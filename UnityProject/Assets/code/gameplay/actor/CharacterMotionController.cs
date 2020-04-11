// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class CharacterMotionController : MonoBehaviour {
		//=============================================================================================
		[Header( "Motion" )]
		[SerializeField, Tooltip( "Time to reach full speed (sec)" )]
		float m_fullSpeedTime = 0.3f;
		[SerializeField, Tooltip( "Time to reach zero speed (sec)" )]
		float m_zeroSpeedTime = 0.15f;

		//=============================================================================================
		public float HorizontalMotion { get; set; }
		public float HorizontalMotionDirection { get; set; }
		public Rigidbody2D Rigidbody { get; set; }

		//=============================================================================================
		public void SetHorizontalMotion( float _motion, float _direction ) {
			HorizontalMotion = _motion;
			HorizontalMotionDirection = _direction;
		}

		//=============================================================================================
		public void ApplyMotion() {
			float currentXMotion = Rigidbody.velocity.x;
			float currentXMotionSign = Mathf.Sign( currentXMotion );
			if ( HorizontalMotionDirection == 0.0f ) {
				// Reduce speed by deceleration factor
				float speedScaleFactor = HorizontalMotion * Time.deltaTime / m_zeroSpeedTime;

				if ( Mathf.Abs( currentXMotion ) < speedScaleFactor ) {
					currentXMotion = 0.0f;
				} else {
					currentXMotion -= currentXMotionSign * speedScaleFactor;
				}

				Rigidbody.velocity = new Vector2( currentXMotion, Rigidbody.velocity.y );

			} else {
				if ( currentXMotion != 0.0f && HorizontalMotionDirection != currentXMotionSign ) {
					float speedScaleFactor = HorizontalMotion * Time.deltaTime / m_zeroSpeedTime;
					currentXMotion -= currentXMotionSign * speedScaleFactor;
				}
				float accelerationFactor = HorizontalMotion * Time.deltaTime / m_fullSpeedTime;
				currentXMotion += HorizontalMotionDirection * accelerationFactor;
				currentXMotion = Mathf.Clamp( currentXMotion, -HorizontalMotion, HorizontalMotion );

				Rigidbody.velocity = new Vector2( currentXMotion, Rigidbody.velocity.y );
			}
			HorizontalMotionDirection = 0.0f;
		}
	}
}
