// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton< T > {
		//=============================================================================================
		public static T Instance { get; private set; }

		//=============================================================================================
		protected bool InitSingleton() {
			Debug.Assert( Instance == null );
			if ( Instance != null ) {
				Destroy( this );
				return false;
			}
			Instance = ( T )this;
			return true;
		}

		//=============================================================================================
		protected void ShutdownSingleton() {
			if ( Instance == this ) {
				Instance = null;
			}
		}
	}
}
