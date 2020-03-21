// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace vzp {
	public static class EditorPaths {
		public const string kAssetPathName = "Assets";
		public const string kResourcePath = "Assets/Resources/";

		public static void EnsureResourcePath( string _subPath = "" ) {
			string dataPath = Application.dataPath;
			if ( dataPath.EndsWith( kAssetPathName ) ) {
				dataPath = dataPath.Substring( 0, dataPath.Length - kAssetPathName.Length );
			}
			EnsurePathExists( dataPath + kResourcePath + _subPath );
		}

		public static void EnsurePathExists( string _fullPath ) {
			Directory.CreateDirectory( _fullPath );
		}

		public static string GetResourcePath( string _relativePath ) {
			string dataPath = Application.dataPath;
			if ( dataPath.EndsWith( kAssetPathName ) ) {
				dataPath = dataPath.Substring( 0, dataPath.Length - kAssetPathName.Length );
			}
			return dataPath + kResourcePath + _relativePath;
		}

		public static string GetResourceDirectory( string _relativePath ) {
			string dataPath = Application.dataPath;
			if ( dataPath.EndsWith( kAssetPathName ) ) {
				dataPath = dataPath.Substring( 0, dataPath.Length - kAssetPathName.Length );
			}
			dataPath += kResourcePath + _relativePath;
			string fileName = Path.GetFileName( dataPath );
			return dataPath.Substring( 0, dataPath.Length - fileName.Length );
		}
	}
}
#endif
