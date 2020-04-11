// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vzp {
	public class NavArrayBuilder : MonoBehaviour {
		//=============================================================================================
		public const short kVersion = 1;

		static Collider2D[] s_hits = null;

		static int s_groundLayerMask = 0;
		static int s_thinGroundLayerMask = 0;
		static int s_wallLayerMask = 0;
		static int s_ladderLayerMask = 0;
		static int s_hideoutLayerMask = 0;

		static Tuple<int, NavArrayCellData>[] s_groundLayerToCellMask = null;
		static Tuple<int, NavArrayCellData>[] s_ceilingLayerToCellMask = null;

		static string s_log;

		//=============================================================================================
		static void Log( string _msg ) {
			s_log += "[NAVARRAY] " + _msg + "\n";
		}

		//=============================================================================================
		static void EmitLog() {
			Debug.Log( s_log );
			s_log = "";
		}

		//=============================================================================================
		[MenuItem( "Ninja/Build Navigation" )]
		public static void Build() {
			Log( "Initializing build" );
			string filePath = NavArray.ScenePathToNavPath( SceneManager.GetActiveScene().path );
			string fullFilePath = EditorPaths.GetResourcePath( filePath ) + ".bytes";
			EditorPaths.EnsurePathExists( EditorPaths.GetResourceDirectory( filePath ) );

			Init();

			NavArray nav = FindNavArray();
			if ( nav == null ) {
				return;
			}

			Log( "Starting computation" );

			try {
				using ( FileStream file = new FileStream( fullFilePath, FileMode.Create, FileAccess.Write ) ) {
					using ( MaskedStream masked = new MaskedStream( file, NavArray.kFileMask ) ) {
						using ( BinaryWriter writer = new BinaryWriter( masked ) ) {
							Build( nav, writer );
						}
					}
				}
			} catch ( Exception _e ) {
				EmitLog();
				Debug.LogError( "[NAVARRAY] Failed to build navigation: " + _e.Message + "\n" + _e.StackTrace );
				return;
			}

			Log( "Build complete. Importing asset" );

			AssetDatabase.ImportAsset( EditorPaths.kResourcePath + filePath + ".bytes" );

			EmitLog();
		}

		//=============================================================================================
		static void Init() {
			s_hits = new Collider2D[ 16 ];

			s_groundLayerMask = LayerMask.GetMask( "Ground" );
			s_thinGroundLayerMask = LayerMask.GetMask( "ThinGround" );
			s_wallLayerMask = LayerMask.GetMask( "Wall" );
			s_ladderLayerMask = LayerMask.GetMask( "Ladder" );
			s_hideoutLayerMask = LayerMask.GetMask( "Hideout" );

			s_groundLayerToCellMask = new Tuple<int, NavArrayCellData>[] {
				new Tuple<int, NavArrayCellData>( s_groundLayerMask, NavArrayCellData.GroundFlag ),
				new Tuple<int, NavArrayCellData>( s_thinGroundLayerMask, NavArrayCellData.GroundFlag | NavArrayCellData.ThinGroundFlag )
			};

			s_ceilingLayerToCellMask = new Tuple<int, NavArrayCellData>[] {
				new Tuple<int, NavArrayCellData>( s_groundLayerMask, NavArrayCellData.CeilingFlag ),
				new Tuple<int, NavArrayCellData>( s_thinGroundLayerMask, NavArrayCellData.CeilingFlag | NavArrayCellData.ThinCeilingFlag )
			};

			s_log = "";
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
			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;
			int blockBitCount = NavArray.NeededBytesForSize( broadphase.width, broadphase.height );

			Log( "Building " + broadphase.width + " x " + broadphase.height + " entries" );

			WriteHeader( _nav, _writer );

			Log( "Allocating memory (" + ( broadphase.width * broadphase.height ) + " entries, " + blockBitCount + " bit bytes)" );

			// Allocate blocks
			NavArrayCellData[,][,] cells = new NavArrayCellData[ broadphase.width, broadphase.height ][,];

			// Allocate bits
			byte[] blockBits = new byte[ blockBitCount ];

			// Compute
			ComputeCells( _nav, cells, blockBits );

			// Write block bits
			_writer.Write( blockBits );

			// Write blocks
			WriteBlocks( _nav, cells, blockBits, _writer );
		}

		//=============================================================================================
		static void WriteHeader( NavArray _nav, BinaryWriter _writer ) {
			Log( "Writing header" );
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
		static void ComputeCells( NavArray _nav, NavArrayCellData[,][,] _cells, byte[] _blockBits ) {
			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;

			Log( "Computing cells" );

			// For each entry in the array
			for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
				for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
					_cells[ x, y ] = new NavArrayCellData[ cellSquareSize, cellSquareSize ];

					// Compute each cell
					bool hasNonEmpty = false;
					for ( int cy = 0; cy < cellSquareSize; ++cy ) {
						for ( int cx = 0; cx < cellSquareSize; ++cx ) {
							Vector2 cellCenter = new Vector2( ( x + broadphase.xMin ) * cellSquareSize + cx + 0.5f, ( y + broadphase.yMin ) * cellSquareSize + cy + 0.5f );
							NavArrayCellData cell = CastCell( cellCenter );
							_cells[ x, y ][ cx, cy ] = cell;
							hasNonEmpty |= ( cell != NavArrayCellData.Empty );
						}
					}

					// Set block bit
					if ( hasNonEmpty ) {
						int byteIndex = NavArray.ByteIndexInBlockBitsForBitIndex( bit );
						byte bitFlag = NavArray.BitIndexInBlockBitForBitIndex( bit );
						_blockBits[ byteIndex ] |= bitFlag;
					}
				}
			}
		}

		//=============================================================================================
		static NavArrayCellData CastCell( Vector2 _center ) {
			NavArrayCellData cell = NavArrayCellData.Empty;

			cell |= CastGround( _center );
			cell |= CastCeiling( _center );
			cell |= CastWall( _center, true );
			cell |= CastWall( _center, false );
			cell |= CastLadder( _center );
			cell |= CastHideout( _center );

			return cell;
		}

		//=============================================================================================
		static NavArrayCellData GenericBoxCast( Vector2 _position, Vector2 _size, int _layerMask, Tuple<int, NavArrayCellData>[] _layerToCellMask ) {
			int castCount = Physics2D.OverlapBoxNonAlloc( _position, _size, 0.0f, s_hits, _layerMask );

			if ( castCount > s_hits.Length ) {
				Debug.LogWarning( "[NAVARRAY] Collider cast limit reached. Consider increasing the number of colliders that can be cast." );
				castCount = s_hits.Length;
			}

			NavArrayCellData mask = NavArrayCellData.Empty;

			for ( int i = 0; i < castCount; ++i ) {
				int objectLayerMask = 1 << s_hits[ i ].gameObject.layer;

				foreach ( Tuple<int, NavArrayCellData> layerToCellMaks in _layerToCellMask ) {
					if ( ( objectLayerMask & layerToCellMaks.Item1 ) != 0 ) {
						mask |= layerToCellMaks.Item2;
					}
				}
			}

			return mask;
		}

		//=============================================================================================
		static bool GenericBoxCast( Vector2 _position, Vector2 _size, int _layerMask ) {
			int castCount = Physics2D.OverlapBoxNonAlloc( _position, _size, 0.0f, s_hits, _layerMask );

			if ( castCount > s_hits.Length ) {
				Debug.LogWarning( "[NAVARRAY] Collider cast limit reached. Consider increasing the number of colliders that can be cast." );
				castCount = s_hits.Length;
			}

			return castCount > 0;
		}

		//=============================================================================================
		static NavArrayCellData CastGround( Vector2 _center ) {
			int layerMask = s_groundLayerMask | s_thinGroundLayerMask;

			// Cast ground and thin ground
			return GenericBoxCast(
				_center + new Vector2( -0.475f, -0.595f ),
				new Vector2( 0.95f, 0.09f ),
				layerMask,
				s_groundLayerToCellMask );
		}

		//=============================================================================================
		static NavArrayCellData CastCeiling( Vector2 _center ) {
			int layerMask = s_groundLayerMask | s_thinGroundLayerMask;

			// Cast ceiling and thin ceiling
			return GenericBoxCast(
				_center + new Vector2( -0.475f, 0.405f ),
				new Vector2( 0.95f, 0.09f ),
				layerMask,
				s_ceilingLayerToCellMask );
		}

		//=============================================================================================
		static NavArrayCellData CastWall( Vector2 _center, bool _left ) {
			Vector2 position = _center + ( _left ? Vector2.left : Vector2.right ) * 0.5f;

			// Cast wall
			if ( GenericBoxCast( position, new Vector2( 0.95f, 0.95f ), s_wallLayerMask ) ) {
				return _left ? NavArrayCellData.LeftWallFlag : NavArrayCellData.RightWallFlag;
			}

			return NavArrayCellData.Empty;
		}

		//=============================================================================================
		static NavArrayCellData CastLadder( Vector2 _center ) {
			if ( GenericBoxCast( _center, new Vector2( 0.95f, 0.09f ), s_ladderLayerMask ) ) {
				return NavArrayCellData.LadderFlag;
			}
			return NavArrayCellData.Empty;
		}

		//=============================================================================================
		static NavArrayCellData CastHideout( Vector2 _center ) {
			if ( GenericBoxCast( _center, new Vector2( 0.95f, 0.09f ), s_hideoutLayerMask ) ) {
				return NavArrayCellData.HideoutFlag;
			}
			return NavArrayCellData.Empty;
		}

		//=============================================================================================
		static void WriteBlocks( NavArray _nav, NavArrayCellData[,][,] _cells, byte[] _blockBits, BinaryWriter _writer ) {
			int cellSquareSize = _nav.CellSquareSize;
			RectInt broadphase = _nav.Broadphase;
			int numBlocks = broadphase.width * broadphase.height;
			int blockSize = sizeof( short ) * cellSquareSize * cellSquareSize;

			Log( "Writing blocks" );

			for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
				for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
					int byteIndex = NavArray.ByteIndexInBlockBitsForBitIndex( bit );
					byte bitFlag = NavArray.BitIndexInBlockBitForBitIndex( bit );

					if ( ( _blockBits[ byteIndex ] & bitFlag ) != 0 ) {
						NavArrayCellData[,] array = _cells[ x, y ];
						byte[] block = new byte[ blockSize ];

						for ( int cy = 0; cy < cellSquareSize; ++cy ) {
							int sy = cy * cellSquareSize;
							for ( int cx = 0; cx < cellSquareSize; ++cx ) {
								int sx = ( sy + cx ) * sizeof( short );
								NavArrayCellData cell = array[ cx, cy ];

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
