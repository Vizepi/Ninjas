// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vzp {
	public class NavArray : MonoBehaviour {
		//=============================================================================================
		public const long kFileMask = 1684235984;
		public const string kAssetPath = "generated/navigation/";
		public const string kAssetExtension = ".nav";
		const string kScenesPath = "Assets/scenes/";

		//=============================================================================================
		[SerializeField, Tooltip( "The rectangle containing all the nodes of the graph" )]
		RectInt m_broadphase = new RectInt( -1, -1, 2, 2 );
		[SerializeField, Tooltip( "Size of a cell square" )]
		int m_cellSquareSize = 10;

		NavArrayCellData[,][,] m_cells = null;

		//=============================================================================================
		public RectInt Broadphase {
			get { return m_broadphase; }
		}

		//=============================================================================================
		public int CellSquareSize {
			get { return m_cellSquareSize; }
		}

		//=============================================================================================
		public static string ScenePathToNavPath( string _scenePath ) {
			Debug.Assert( _scenePath.StartsWith( kScenesPath ) && _scenePath.EndsWith( ".unity" ) );
			return Path.ChangeExtension( kAssetPath + _scenePath.Substring( kScenesPath.Length ), kAssetExtension );
		}

		//=============================================================================================
		void Awake() {
			string sceneName = ScenePathToNavPath( SceneManager.GetActiveScene().path );
			TextAsset asset = Resources.Load<TextAsset>( sceneName );

			if ( asset == null ) {
				Debug.LogWarning( "[NAVARRAY] NavArray not found for current scene. Maybe a rebuild should be done." );
			} else if ( !LoadFile( asset.bytes ) ) {
				Debug.LogWarning( "[NAVARRAY] Failed to load NavArray file." );
			}
			Resources.UnloadAsset( asset );
		}

		//=============================================================================================
		public static int NeededBytesForSize( int _broadphaseWidth, int _broadphaseHeight ) {
			return ( ( ( _broadphaseWidth * _broadphaseHeight ) + 7 ) & ~7 ) >> 3;
		}

		//=============================================================================================
		public static int ByteIndexInBlockBitsForBitIndex( int _bitIndex ) {
			return _bitIndex >> 3;
		}

		//=============================================================================================
		public static byte BitIndexInBlockBitForBitIndex( int _bitIndex ) {
			return ( byte )( 1 << ( _bitIndex & 7 ) );
		}

		//=============================================================================================
		bool LoadFile( byte[] _content ) {
			try {
				using ( MemoryStream memory = new MemoryStream( _content ) ) {
					using ( MaskedStream masked = new MaskedStream( memory, kFileMask ) ) {
						using ( BinaryReader reader = new BinaryReader( masked ) ) {
							// Check version
							short version = reader.ReadInt16();
							if ( version > 1 || version <= 0 ) {
								throw new Exception( "Version not supported. Supports version up to 1" );
							}

							// Get cell square size
							int cellSquareSize = reader.ReadByte();

							// Ensure cell square match
							if ( cellSquareSize != m_cellSquareSize ) {
								throw new Exception( "Cell square size does not match the size built in NavArray" );
							}

							// Get broadphase
							RectInt broadphase = new RectInt();
							broadphase.xMin = reader.ReadByte();
							broadphase.yMin = reader.ReadByte();
							broadphase.xMax = reader.ReadByte();
							broadphase.yMax = reader.ReadByte();

							// Ensure broadphases match
							if ( broadphase.min != m_broadphase.min || broadphase.max != broadphase.max ) {
								throw new Exception( "Broadphase does not match the broadphase built in NavArray" );
							}

							// Initialize NavArray							
							m_cells = new NavArrayCellData[ broadphase.width, broadphase.height ][,];

							// Get block bits
							int blockBitsByteCount = NeededBytesForSize( broadphase.width, broadphase.height );
							byte[] blockBits = reader.ReadBytes( blockBitsByteCount );

							// Read blocks
							int blockSize = sizeof( short ) * cellSquareSize * cellSquareSize;
							for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
								for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
									// Select only blocks for wich the block bit is set
									int byteIndex = ByteIndexInBlockBitsForBitIndex( bit );
									byte bitFlag = BitIndexInBlockBitForBitIndex( bit );

									if ( ( blockBits[ byteIndex ] & bitFlag ) != 0 ) {
										byte[] block = reader.ReadBytes( blockSize );
										m_cells[ x, y ] = new NavArrayCellData[ cellSquareSize, cellSquareSize ];

										for ( int cy = 0; cy < cellSquareSize; ++cy ) {
											int sy = cy * cellSquareSize;
											for ( int cx = 0; cx < cellSquareSize; ++cx ) {
												int sx = ( sy + cx ) * sizeof( short );

												m_cells[ x, y ][ cx, cy ] = ( NavArrayCellData )( ( block[ sx ] << 8 ) | ( block[ sx + 1 ] ) );
											}
										}
									}
								}
							}
						}
					}
				}
			} catch ( Exception _e ) {
#if UNITY_EDITOR
				Debug.LogError( "[NAVARRAY] Failed to read NavArray asset: " + _e.Message + "\n" + _e.StackTrace );
#else
				Debug.LogError( "[NAVARRAY] Failed to read NavArray asset: " + _e.Message );
#endif
				return false;
			}
			return true;
		}

#if UNITY_EDITOR
		//=============================================================================================
		void OnDrawGizmosCommon() {
			// Broadphase
			Vector3 p1 = new Vector3( m_broadphase.xMin * m_cellSquareSize, m_broadphase.yMin * m_cellSquareSize, 0.0f );
			Vector3 p2 = new Vector3( m_broadphase.xMin * m_cellSquareSize, m_broadphase.yMax * m_cellSquareSize, 0.0f );
			Vector3 p3 = new Vector3( m_broadphase.xMax * m_cellSquareSize, m_broadphase.yMax * m_cellSquareSize, 0.0f );
			Vector3 p4 = new Vector3( m_broadphase.xMax * m_cellSquareSize, m_broadphase.yMin * m_cellSquareSize, 0.0f );
			Gizmos.DrawLine( p1, p2 );
			Gizmos.DrawLine( p2, p3 );
			Gizmos.DrawLine( p3, p4 );
			Gizmos.DrawLine( p4, p1 );
		}

		//=============================================================================================
		void OnDrawGizmos() {
			Color color = Color.red;
			color.a = 0.25f;
			Gizmos.color = color;
			OnDrawGizmosCommon();
		}

		//=============================================================================================
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.red;
			OnDrawGizmosCommon();
		}
#endif // UNITY_EDITOR
	}
}
