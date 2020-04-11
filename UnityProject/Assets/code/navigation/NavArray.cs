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
		public RectInt ExpandedBroadphase { get; private set; }

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
			ExpandedBroadphase = new RectInt(
				m_broadphase.xMin * m_cellSquareSize,
				m_broadphase.yMin * m_cellSquareSize,
				m_broadphase.width * m_cellSquareSize,
				m_broadphase.height * m_cellSquareSize );

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
		bool LoadFile( byte[] _content, bool _logError = true ) {
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
							broadphase.xMin = reader.ReadInt16();
							broadphase.yMin = reader.ReadInt16();
							broadphase.xMax = reader.ReadInt16();
							broadphase.yMax = reader.ReadInt16();

							// Ensure broadphases match
							if ( broadphase.min != m_broadphase.min || broadphase.max != m_broadphase.max ) {
								throw new Exception( "Broadphase does not match the broadphase built in NavArray - Component: " + m_broadphase.ToString() + ", File: " + broadphase.ToString() );
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
				if ( _logError ) {
#if UNITY_EDITOR
					Debug.LogError( "[NAVARRAY] Failed to read NavArray asset: " + _e.Message + "\n" + _e.StackTrace );
#else
				Debug.LogError( "[NAVARRAY] Failed to read NavArray asset: " + _e.Message );
#endif
				}
				return false;
			}
			return true;
		}

		//=============================================================================================
		public NavArrayCellData? GetCellData( Vector2Int _position ) {
			if ( _position.x < ExpandedBroadphase.xMin ||
				_position.x >= ExpandedBroadphase.xMax ||
				_position.y < ExpandedBroadphase.yMin ||
				_position.y >= ExpandedBroadphase.yMax ) {
				return null;
			}

			NavArrayCellData[,] cells = m_cells[ _position.x / m_cellSquareSize, _position.y / m_cellSquareSize ];

			return cells == null ? NavArrayCellData.Empty : cells[ _position.x % m_cellSquareSize, _position.y % m_cellSquareSize ];
		}

		//=============================================================================================
		public NavArrayCell GetCell( Vector2Int _position ) {
			NavArrayCellData? cellData = GetCellData( _position );
			if ( cellData.HasValue ) {
				return new NavArrayCell( this, _position, cellData.Value, true );
			} else {
				return new NavArrayCell( this, _position, NavArrayCellData.Empty, false );
			}
		}

		//=============================================================================================
		public NavArrayCell GetCell( Vector3 _worldPosition ) {
			Vector2Int position = new Vector2Int( Mathf.FloorToInt( _worldPosition.x ), Mathf.FloorToInt( _worldPosition.y ) );
			NavArrayCellData ? cellData = GetCellData( position );
			if ( cellData.HasValue ) {
				return new NavArrayCell( this, position, cellData.Value, true );
			} else {
				return new NavArrayCell( this, position, NavArrayCellData.Empty, false );
			}
		}

#if UNITY_EDITOR
		//=============================================================================================
		[Header( "Debug" )]
		[SerializeField, Tooltip( "Show NavArray representation" )]
		bool m_debugShowNavArray = false;
		[SerializeField, Tooltip( "Transparency of the NavArray texture" ), Range( 0.0f, 1.0f )]
		float m_debugTextureAlpha = 0.5f;

		Texture2D m_debugTexture = null;
		int m_debugTextureIsBuilt = 0;

		public bool RefreshTexture { private get; set; }

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
			--m_debugTextureIsBuilt;

			if ( m_debugShowNavArray ) {
				int w = m_broadphase.width * m_cellSquareSize;
				int h = m_broadphase.height * m_cellSquareSize;

				if ( m_debugTextureIsBuilt <= 0 || RefreshTexture || !Mathf.Approximately( m_debugTexture.GetPixel( 0, 0 ).a, m_debugTextureAlpha ) ) {
					RefreshTexture = false;

					NavArrayCellData[,][,] cellsBackup = m_cells;

					try {
						string sceneName = ScenePathToNavPath( SceneManager.GetActiveScene().path );
						TextAsset asset = Resources.Load<TextAsset>( sceneName );
						LoadFile( asset.bytes, false );

						m_debugTexture = new Texture2D( w * 5, h * 5, TextureFormat.ARGB32, false );
						m_debugTexture.filterMode = FilterMode.Point;
						m_debugTextureIsBuilt = 1000;

						Color[] redBlock = new Color[ 25 ];
						Color[] emptyBlock = new Color[ 25 ];
						for ( int i = 0; i < 25; ++i ) {
							redBlock[ i ] = new Color( 1.0f, 0.0f, 0.0f, m_debugTextureAlpha / 2.0f );
							emptyBlock[ i ] = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
						}

						for ( int j = 0; j < h; ++j ) {
							int aj = j / CellSquareSize;
							int bj = j % CellSquareSize;
							for ( int i = 0; i < w; ++i ) {
								int ai = i / CellSquareSize;
								NavArrayCellData[,] dataArray = m_cells[ ai, aj ];
								if ( dataArray == null ) {
									m_debugTexture.SetPixels( i * 5, j * 5, 5, 5, redBlock );
								} else {
									int bi = i % CellSquareSize;
									NavArrayCellData data = dataArray[ bi, bj ];
									m_debugTexture.SetPixels( i * 5, j * 5, 5, 5, emptyBlock );
									if ( data.HasLadder() ) {
										Color c = new Color( 163 / 255.0f, 73 / 255.0f, 164 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 0, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 1, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 2, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 3, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 4, c );
									}
									if ( data.HasHideout() ) {
										Color c = new Color( 255 / 255.0f, 174 / 255.0f, 201 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 0, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 1, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 2, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 3, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 4, c );

										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 0, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 1, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 2, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 3, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 4, c );
									}
									if ( data.HasLeftWall() ) {
										Color c = new Color( 255 / 255.0f, 127 / 255.0f, 39 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5, j * 5 + 0, c );
										m_debugTexture.SetPixel( i * 5, j * 5 + 1, c );
										m_debugTexture.SetPixel( i * 5, j * 5 + 2, c );
										m_debugTexture.SetPixel( i * 5, j * 5 + 3, c );
										m_debugTexture.SetPixel( i * 5, j * 5 + 4, c );
									}
									if ( data.HasRightWall() ) {
										Color c = new Color( 255 / 255.0f, 127 / 255.0f, 39 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 0, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 1, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 2, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 3, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 4, c );
									}
									if ( data.HasCeiling() ) {
										Color c = new Color( 34 / 255.0f, 177 / 255.0f, 76 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 0, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5 + 4, c );
									}
									if ( data.HasThinCeiling() ) {
										Color c = new Color( 181 / 255.0f, 230 / 255.0f, 29 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5 + 4, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5 + 4, c );
									}
									if ( data.HasGround() ) {
										Color c = new Color( 0 / 255.0f, 162 / 255.0f, 232 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 0, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 4, j * 5, c );
									}
									if ( data.HasThinGround() ) {
										Color c = new Color( 153 / 255.0f, 217 / 255.0f, 234 / 255.0f, m_debugTextureAlpha );
										m_debugTexture.SetPixel( i * 5 + 1, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 2, j * 5, c );
										m_debugTexture.SetPixel( i * 5 + 3, j * 5, c );
									}
								}
							}
						}
						m_debugTexture.Apply();
					} catch( Exception _e ) {}

					m_cells = cellsBackup;
				}

				if ( m_debugTextureIsBuilt > 0 ) {
					int x = m_broadphase.xMin * m_cellSquareSize;
					int y = m_broadphase.yMin * m_cellSquareSize;

					Gizmos.DrawGUITexture( new Rect( x, y + h, w, -h ), m_debugTexture );
				}
			}
		}
#endif // UNITY_EDITOR
	}
}
