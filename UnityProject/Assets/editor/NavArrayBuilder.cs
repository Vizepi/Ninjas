// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vzp {
	public class NavArrayBuilder : MonoBehaviour {
		public const short kVersion = 1;

		static Collider2D[] s_hits = null;

		static int s_groundLayerMask = 0;
		static int s_thinGroundLayerMask = 0;

		static Tuple<int, NavArray.Cell>[] s_groundLayerToCellMask = null;

		//=============================================================================================
		[ MenuItem( "Ninja/Build Navigation" )]
		public static void Build() {
			string filePath = NavArray.ScenePathToNavPath( SceneManager.GetActiveScene().path );
			string fullFilePath = EditorPaths.GetResourcePath( filePath ) + ".bytes";
			EditorPaths.EnsurePathExists( EditorPaths.GetResourceDirectory( filePath ) );

			Init();

			NavArray nav = FindNavArray();
			if ( nav == null ) {
				return;
			}

			try {
				using ( FileStream file = new FileStream( fullFilePath, FileMode.Create, FileAccess.Write ) ) {
					using ( MaskedStream masked = new MaskedStream( file, NavArray.kFileMask ) ) {
						using ( BinaryWriter writer = new BinaryWriter( masked ) ) {
							Build( nav, writer );
						}
					}
				}
			} catch ( Exception _e ) {
				Debug.LogError( "[NAVARRAY] Failed to build navigation: " + _e.Message );
				return;
			}

			AssetDatabase.ImportAsset( EditorPaths.kResourcePath + filePath + ".bytes" );
		}

		//=============================================================================================
		static void Init() {
			s_hits = new Collider2D[ 16 ];

			s_groundLayerMask = LayerMask.GetMask( "Ground" );
			s_thinGroundLayerMask = LayerMask.GetMask( "ThinGround" );

			s_groundLayerToCellMask = new Tuple<int, NavArray.Cell>[] {
				new Tuple<int, NavArray.Cell>( s_groundLayerMask, NavArray.Cell.IsGround ),
				new Tuple<int, NavArray.Cell>( s_thinGroundLayerMask, NavArray.Cell.IsGround | NavArray.Cell.IsGroundThin )
			};
		}

		//=============================================================================================
		static NavArray FindNavArray() {
			NavArray[] navs = FindObjectsOfType<NavArray>();
			if ( navs.Length != 1 ) {
				if ( navs.Length == 0 ) {
					Debug.LogError( "[NAVARRAY] Cannot build navigation: add a NavArray component in the scene." );
				} else {
					Debug.LogError( "[NAVARRAY] Cannot build navigation: there is more than one NavArray component in the scene." );
				}
				return null;
			}
			return navs[ 0 ];
		}

		//=============================================================================================
		static void Build( NavArray _nav, BinaryWriter _writer ) {
			WriteHeader( _nav, _writer );

			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;

			// Allocate blocks
			NavArray.Cell[,][,] cells = new NavArray.Cell[ broadphase.width, broadphase.height ][,];

			// Allocate bits
			byte[] blockBits = new byte[ NavArray.NeededBytesForSize( broadphase.width, broadphase.height ) ];

			// Compute
			ComputeCells( _nav, cells, blockBits );

			// Write block bits
			_writer.Write( blockBits );

			// Write blocks
			WriteBlocks( _nav, cells, blockBits, _writer );
		}

		//=============================================================================================
		static void WriteHeader( NavArray _nav, BinaryWriter _writer ) {
			// Write version
			_writer.Write( kVersion );

			// Cell square size
			_writer.Write( ( byte )_nav.CellSquareSize );

			// Broadphase
			RectInt broadphase = _nav.Broadphase;
			_writer.Write( ( byte )broadphase.xMin );
			_writer.Write( ( byte )broadphase.yMin );
			_writer.Write( ( byte )broadphase.xMax );
			_writer.Write( ( byte )broadphase.yMax );
		}

		//=============================================================================================
		static void ComputeCells( NavArray _nav, NavArray.Cell[,][,] _cells, byte[] _blockBits ) {
			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;

			// For each entry in the array
			for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
				for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
					_cells[ x, y ] = new NavArray.Cell[ cellSquareSize, cellSquareSize ];

					// Compute each cell
					bool hasNonEmpty = false;
					for ( int cy = 0; cy < cellSquareSize; ++cy ) {
						for ( int cx = 0; cx < cellSquareSize; ++cx ) {
							Vector2 cellCenter = new Vector2( x * cellSquareSize + cx + 0.5f, y * cellSquareSize + cy + 0.5f );
							NavArray.Cell cell = CastCell( cellCenter );
							_cells[ x, y ][ cx, cy ] = cell;
							hasNonEmpty |= ( cell != NavArray.Cell.Empty );
						}
					}

					// Write block bit
					if ( hasNonEmpty ) {
						_blockBits[ NavArray.ByteIndexInBlockBitsForBitIndex( bit ) ] |= NavArray.BitIndexInBlockBitForBitIndex( bit );
					}
				}
			}
		}

		//=============================================================================================
		static NavArray.Cell CastCell( Vector2 _center ) {
			NavArray.Cell cell = NavArray.Cell.Empty;

			cell |= CastGround( _center );
			cell |= CastCeiling( _center );

			// Cast left wall
			// Cast right wall
			// Cast ladder
			// Cast hideout


			return cell;
		}

		//=============================================================================================
		static int GenericBoxCast( Vector2 _position, Vector2 _size, int _layerMask ) {
			int castCount = Physics2D.OverlapBoxNonAlloc( _position, _size, 0.0f, s_hits, _layerMask );

			if ( castCount > s_hits.Length ) {
				Debug.LogWarning( "[NAVARRAY] Collider cast limit reached. Consider increasing the number of colliders that can be cast." );
				castCount = s_hits.Length;
			}

			return castCount;
		}

		//=============================================================================================
		static NavArray.Cell GenericDeduceMask( int _castCount, Tuple<int, NavArray.Cell>[] _layerToCellMask ) {
			NavArray.Cell mask = NavArray.Cell.Empty;

			for ( int i = 0; i < _castCount; ++i ) {
				int objectLayer = s_hits[ i ].gameObject.layer;

				foreach ( Tuple<int, NavArray.Cell> layerToCellMaks in _layerToCellMask ) {
					if ( ( objectLayer & layerToCellMaks.Item1 ) != 0 ) {
						mask |= layerToCellMaks.Item2;
					}
				}
			}

			return mask;
		}

		//=============================================================================================
		static NavArray.Cell CastGround( Vector2 _center ) {
			int layerMask = s_groundLayerMask | s_thinGroundLayerMask;

			// Cast ground and thin ground
			int castCount = GenericBoxCast( _center + new Vector2( -0.475f, -0.595f ), new Vector2( 0.95f, 0.09f ), layerMask );
			return GenericDeduceMask( castCount, s_groundLayerToCellMask );
		}

		//=============================================================================================
		static NavArray.Cell CastCeiling( Vector2 _center ) {
			int layerMask = s_groundLayerMask | s_thinGroundLayerMask;

			// Cast ceiling and thin ceiling
			int castCount = GenericBoxCast( _center + new Vector2( -0.475f, 0.405f ), new Vector2( 0.95f, 0.09f ), layerMask );
			return GenericDeduceMask( castCount, s_groundLayerToCellMask );
		}

		//=============================================================================================
		static void WriteBlocks( NavArray _nav, NavArray.Cell[,][,] _cells, byte[] _blockBits, BinaryWriter _writer ) {
			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;

			int numBlocks = broadphase.width * broadphase.height;
			byte[] block = new byte[ cellSquareSize * cellSquareSize ];
			for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
				for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
					if ( ( _blockBits[ NavArray.ByteIndexInBlockBitsForBitIndex( bit ) ] & NavArray.BitIndexInBlockBitForBitIndex( bit ) ) != 0 ) {
						NavArray.Cell[,] array = _cells[ x, y ];
						for ( int cy = 0; cy < cellSquareSize; ++cy ) {
							int sy = cy * cellSquareSize;
							for ( int cx = 0; cx < cellSquareSize; ++cx ) {
								int sx = ( sy + cx ) * 2;
								NavArray.Cell cell = array[ cx, cy ];
								block[ sx ] = ( byte )( ( int )cell >> 8 );
								block[ sx + 1 ] = ( byte )( ( int )cell & 0xff );
							}
						}
						_writer.Write( block );
					}
				}
			}
		}
	}
}
