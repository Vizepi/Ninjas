// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vzp {
	public class NavArrayBuilder : MonoBehaviour {
		public const short kVersion = 1;

		//=============================================================================================
		[MenuItem( "Ninja/Build Navigation" )]
		public static void Build() {
			string filePath = NavArray.ScenePathToNavPath( SceneManager.GetActiveScene().path );
			string fullFilePath = EditorPaths.GetResourcePath( filePath ) + ".bytes";
			EditorPaths.EnsurePathExists( EditorPaths.GetResourceDirectory( filePath ) );

			NavArray nav = null;
			{
				NavArray[] navs = FindObjectsOfType<NavArray>();
				if ( navs.Length != 1 ) {
					if ( navs.Length == 0 ) {
						Debug.LogError( "[NAVARRAY] Cannot build navigation: add a NavArray component in the scene." );
					} else {
						Debug.LogError( "[NAVARRAY] Cannot build navigation: there is more than one NavArray component in the scene." );
					}
					return;
				}
				nav = navs[ 0 ];
			}

			try {
				using ( FileStream file = new FileStream( fullFilePath, FileMode.Create, FileAccess.Write ) ) {
					using ( MaskedStream masked = new MaskedStream( file, NavArray.kFileMask ) ) {
						using ( BinaryWriter writer = new BinaryWriter( masked ) ) {
							// Write version
							writer.Write( kVersion );

							// Cell square size
							writer.Write( ( byte )( nav.CellSquareSize ) );

							// Broadphase
							RectInt broadphase = nav.Broadphase;
							writer.Write( ( byte )broadphase.xMin );
							writer.Write( ( byte )broadphase.yMin );
							writer.Write( ( byte )broadphase.xMax );
							writer.Write( ( byte )broadphase.yMax );

							// Allocate blocks
							int numBlocks = broadphase.width * broadphase.height;
							NavArray.Cell[,][,] cells = new NavArray.Cell[ broadphase.width, broadphase.height ][,];

							// Compute
							for ( int y = 0, bit = 0; y < broadphase.height; ++y ) {
								for ( int x = 0; x < broadphase.width; ++x, ++bit ) {
									cells[ x, y ] = new NavArray.Cell[ nav.CellSquareSize, nav.CellSquareSize ];
								}
							}

							// Allocate bits
							byte[] blockBits = new byte[ NavArray.NeededBytesForSize( broadphase.width, broadphase.height ) ];

							// Check null blocks

							// Write block bits

							// Write blocks
						}
					}
				}
			} catch ( Exception _e ) {
				Debug.LogError( "[NAVARRAY] Failed to build navigation: " + _e.Message );
				return;
			}

			AssetDatabase.ImportAsset( EditorPaths.kResourcePath + filePath + ".bytes" );
		}
	}
}
