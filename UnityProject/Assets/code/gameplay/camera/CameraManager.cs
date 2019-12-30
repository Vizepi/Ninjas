// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public class CameraManager : Singleton<CameraManager> {
		//=============================================================================================
		[Serializable]
		public class CameraSlot {
			[SerializeField, Tooltip( "The camera" )]
			public Camera camera = null;
			[SerializeField, Tooltip( "Position of the camera" )]
			public Transform transform = null;
			[SerializeField, Tooltip( "Acceleration of the camera ( unit/sec²)" )]
			public float acceleration = 60.0f;
		}

		//=============================================================================================
		[SerializeField, Tooltip( "Main camera" )]
		CameraSlot m_mainCamera = null;
		[SerializeField, Tooltip( "Additional cameras" )]
		CameraSlot[] m_otherCameras = null;

		//=============================================================================================
		void Awake() {
			if ( !InitSingleton() ) {
				return;
			}

			Debug.Assert( m_mainCamera.camera );
			Debug.Assert( m_mainCamera.transform );
			foreach ( CameraSlot camera in m_otherCameras ) {
				Debug.Assert( camera.camera );
				Debug.Assert( camera.transform );
			}
		}

		//=============================================================================================
		void OnDestroy() {
			ShutdownSingleton();
		}

		//=============================================================================================
		void Update() {
			UpdateSlot( m_mainCamera );
			foreach ( CameraSlot slot in m_otherCameras) {
				UpdateSlot( slot );
			}
		}

		//=============================================================================================
		void UpdateSlot( CameraSlot _slot ) {
			if ( _slot.camera.transform != _slot.transform ) {
				float lerpFactor = _slot.acceleration * Time.deltaTime;
				_slot.camera.transform.position = Vector3.Lerp(
					_slot.camera.transform.position,
					_slot.transform.position,
					lerpFactor );
				_slot.camera.transform.rotation = Quaternion.Slerp(
					_slot.camera.transform.rotation,
					_slot.transform.rotation,
					lerpFactor );
			}
		}

		//=============================================================================================
		public void SetMainCameraTransform( Transform _transform, bool _instant = false ) {
			SetCameraTransformInternal( m_mainCamera, _transform, _instant );
		}

		//=============================================================================================
		public void SetMainCameraAcceleration( float _acceleration ) {
			SetCameraAccelerationInternal( m_mainCamera, _acceleration );
		}

		//=============================================================================================
		public void SetCameraTransform( int _index, Transform _transform, bool _instant = false ) {
			if ( _index >= 0 && _index < m_otherCameras.Length ) {
				SetCameraTransformInternal( m_otherCameras[ _index ], _transform, _instant );
			}
		}

		//=============================================================================================
		public void SetCameraAcceleration( int _index, float _acceleration ) {
			if ( _index >= 0 && _index < m_otherCameras.Length ) {
				SetCameraAccelerationInternal( m_otherCameras[ _index ], _acceleration );
			}
		}

		//=============================================================================================
		public void SetCameraTransformInternal( CameraSlot _slot, Transform _transform, bool _instant = false ) {
			_slot.transform = _transform;
			if ( _instant ) {
				_slot.camera.transform.position = _transform.position;
				_slot.camera.transform.rotation = _transform.rotation;
			}
		}

		//=============================================================================================
		public void SetCameraAccelerationInternal( CameraSlot _slot, float _acceleration ) {
			_slot.acceleration = Mathf.Clamp01( _acceleration );
		}
	}
}
