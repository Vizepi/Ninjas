// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public static class VectorConverter {
		public static Vector2 ToVector2( Vector3 _v ) { return new Vector2( _v.x, _v.y ); }
		public static Vector2 ToVector2( Vector4 _v ) { return new Vector2( _v.x, _v.y ); }
		public static Vector3 ToVector3( Vector2 _v ) { return new Vector3( _v.x, _v.y, 0.0f ); }
		public static Vector3 ToVector3( Vector4 _v ) { return new Vector3( _v.x, _v.y, _v.z ); }
		public static Vector4 ToVector4( Vector2 _v ) { return new Vector4( _v.x, _v.y, 0.0f, 0.0f ); }
		public static Vector4 ToVector4( Vector3 _v ) { return new Vector4( _v.x, _v.y, _v.z, 0.0f ); }

		public static void Square( ref Vector2 _v ) {
			_v.x *= _v.x;
			_v.y *= _v.y;
		}

		public static void Square( ref Vector3 _v ) {
			_v.x *= _v.x;
			_v.y *= _v.y;
			_v.z *= _v.z;
		}

		public static void Square( ref Vector4 _v ) {
			_v.x *= _v.x;
			_v.y *= _v.y;
			_v.z *= _v.w;
			_v.w *= _v.w;
		}
	}
}
