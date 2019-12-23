// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vzp {
	public class vzpColliderMerge : MonoBehaviour {
		const string kScenePath = "Assets/scenes/";
		const string kOutputPath = "Assets/Resources/scenes/";
		const string kSceneExtension = ".unity";
		const string kOutputExtension = ".prefab";

		//=============================================================================================
		[MenuItem( "Ninja/Compilers/ColliderMerge" )]
		public static void Compile() {
			CompileInternal( null );
		}

		//=============================================================================================
		public static bool CompileInternal( string _scenePath ) {
			string sceneBackup = SceneManager.GetActiveScene().name;
			if ( _scenePath == null ) {
				_scenePath = sceneBackup;
			}

			Scene scene = EditorSceneManager.OpenScene( _scenePath );
			if ( !scene.IsValid() ) {
				Debug.LogError( "[COLLIDERMERGE] Failed to open scene '" + _scenePath + "'" );
				return false;
			}

			string outputPath = GetOutputPath( _scenePath );
			if ( outputPath.StartsWith( "[" ) ) {
				Debug.LogError( outputPath );
				return false;
			}

			return true;
		}

		//=============================================================================================
		static string GetOutputPath( string _scenePath ) {
			if ( !_scenePath.StartsWith( kScenePath ) ) {
				return "[COLLIDERMERGE] Scene must be in '" + kScenePath + "...': '" + _scenePath + "'";
			}
			if ( !_scenePath.EndsWith( kSceneExtension ) ) {
				return "[COLLIDERMERGE] Extension must be in '" + kSceneExtension + "': '" + _scenePath + "'";
			}

			return kOutputPath + _scenePath.Substring( kScenePath.Length, _scenePath.Length - kScenePath.Length - kSceneExtension.Length ) + kOutputExtension;
		}
	}
}
