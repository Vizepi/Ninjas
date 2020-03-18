// Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class PlayerCamera {
			//=============================================================================================
			[SerializeField, Tooltip( "The camera to link to the player" )]
			Transform m_camera = null;
			[SerializeField, Tooltip( "Acceleration of the camera (unit/sec²" )]
			float m_acceleration = 35.0f;
			[SerializeField, Tooltip( "Check to use custom vector as offset instead of camera's initial offset" )]
			bool m_useCustomOffset = false;
			[SerializeField, Tooltip( "Custom offset from camera. Ignored if Use custom offset is not checked. Offset is from target to camera" )]
			Vector3 m_customOffset = Vector3.zero;
			[SerializeField, Tooltip( "Size of the position history" )]
			int m_historySize = 30;
			[SerializeField, Tooltip( "Rate at which to sample player position (Hz)" )]
			int m_historyRate = 60;

			Vector3 m_offset;
			Vector3 m_position;

			struct Delta {
				public Vector2 position;
				public Vector2 calculatedPosition;
			}

			Vector4[] m_history = null;
			int m_historyIndex = 0;
			float m_historyTimeBetweenSamples = 1.0f;
			float m_historyNextSamplingTime = 0.0f;

			//=============================================================================================
			public void Awake() {
				Debug.Assert( m_camera );

				m_position = Game.Player.transform.position;

				if ( m_useCustomOffset ) {
					m_offset = m_customOffset;
				} else {
					m_offset = m_camera.position - Game.Player.transform.position;
				}

				m_history = new Vector4[ m_historySize ];

				for ( int i = 0; i < m_history.Length; ++i ) {
					m_history[ i ] = new Vector4 (
						m_position.x, m_position.y ,
						m_position.x * m_history.Length, m_position.y * m_history.Length );
				}

				m_historyIndex = m_history.Length;

				m_historyTimeBetweenSamples = 1.0f / m_historyRate;
				m_historyNextSamplingTime = Time.time + m_historyTimeBetweenSamples;
			}

			//=============================================================================================
			public void Update() {
				UpdateHistory();
				Vector3 position = Vector3.Lerp( m_camera.position - m_offset, m_position, m_acceleration * Time.deltaTime );

				m_camera.position = position + m_offset;
				m_camera.LookAt( Game.Player.transform.position, Vector3.up );
			}

			//=============================================================================================
			void UpdateHistory() {
				float time = Time.time;
				if ( time > m_historyNextSamplingTime ) {
					if ( time - m_historyNextSamplingTime > m_historyTimeBetweenSamples * m_history.Length ) {
						// Big hang, reset position
						Vector3 playerPosition = Game.Player.transform.position;

						for ( int i = 0; i < m_history.Length; ++i ) {
							m_history[ i ] = new Vector4(
								playerPosition.x, playerPosition.y,
								playerPosition.x * m_history.Length, playerPosition.y * m_history.Length );
						}

						m_historyIndex = m_history.Length;
						m_historyNextSamplingTime = Time.time + m_historyTimeBetweenSamples;

						m_position.x = playerPosition.x;
						m_position.y = playerPosition.y;

					} else {
						while ( time > m_historyNextSamplingTime ) {
							m_historyNextSamplingTime += m_historyTimeBetweenSamples;

							// Update ring buffer
							Vector3 playerPosition = Game.Player.transform.position;

							Vector4 first = m_history[ m_historyIndex % m_history.Length ];
							Vector4 last = m_history[ ( m_historyIndex - 1 ) % m_history.Length ];

							Vector2 calculatedPosition = new Vector2(
								last.z + playerPosition.x - first.x,
								last.w + playerPosition.y - first.y );

							m_history[ m_historyIndex % m_history.Length ] = new Vector4(
								playerPosition.x,
								playerPosition.y,
								calculatedPosition.x,
								calculatedPosition.y );

							calculatedPosition /= m_history.Length;

							m_position.x = calculatedPosition.x;
							m_position.y = calculatedPosition.y;

							++m_historyIndex;
						}
					}
				}
			}
		}
	}
}
