// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateStickWall : StateMotion {
			//=============================================================================================
			[SerializeField, Tooltip( "Distance from the player's side where we are considered sticked (unit)" )]
			float m_stickDistance = 0.05f;
			[SerializeField, Tooltip( "Offsets of left sticking raycasts" )]
			Vector2[] m_leftStickOffset = null;
			[SerializeField, Tooltip( "Offsets of right sticking raycasts" )]
			Vector2[] m_rightStickOffset = null;
			[Header( "Animation" )]
			[SerializeField, Tooltip( "Name of the stick on the wall animation" )]
			string m_stickAnimationName = "";

			int m_stickAnimationKey = 0;
			bool m_stickingOnLeftWall = false;

			//=============================================================================================
			public override MotionState GetStateName() {
				return MotionState.StickWall;
			}

			//=============================================================================================
			public override bool TryTransition( MotionState _fromState ) {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				// Left wall
				bool stick = true;
				foreach ( Vector2 offset in m_leftStickOffset ) {
					if ( !CastWall( offset, Vector2.left ) ) {
						stick = false;
						break;
					}
				}
				if ( stick ) {
					m_stickingOnLeftWall = true;
					Instance.SetState( GetStateName() );
					return true;
				}

				// Right wall
				stick = true;
				foreach ( Vector2 offset in m_rightStickOffset ) {
					if ( !CastWall( offset, Vector2.right ) ) {
						stick = false;
						break;
					}
				}
				if ( stick ) {
					m_stickingOnLeftWall = false;
					Instance.SetState( GetStateName() );
					return true;
				}

				return false;
			}

			//=============================================================================================
			public override void Awake() {
				Debug.Assert( m_leftStickOffset != null && m_leftStickOffset.Length != 0 );
				Debug.Assert( m_rightStickOffset != null && m_rightStickOffset.Length != 0 );

				m_stickAnimationKey = Animator.StringToHash( m_stickAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Instance.m_animator.Play( m_stickAnimationKey );
				Instance.m_rigidbody.isKinematic = true;
			}

			//=============================================================================================
			public override void Update() {
				// Check state transitions
				Instance.m_rigidbody.isKinematic = false;
				if ( Instance.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ||
					Instance.GetMotionState( MotionState.Fall ).TryTransition( GetStateName() ) ) {
					return;
				}
				Instance.m_rigidbody.isKinematic = true;
			}

			//=============================================================================================
			public override void OnDrawGizmos() {
				Gizmos.color = Color.cyan;
				foreach ( Vector2 offset in m_leftStickOffset ) {
					Gizmos.DrawLine(
						Instance.transform.position + new Vector3( offset.x, offset.y, 0.0f ),
						Instance.transform.position + new Vector3( offset.x - m_stickDistance, offset.y, 0.0f ) );
				}
				foreach ( Vector2 offset in m_rightStickOffset ) {
					Gizmos.DrawLine(
						Instance.transform.position + new Vector3( offset.x, offset.y, 0.0f ),
						Instance.transform.position + new Vector3( offset.x + m_stickDistance, offset.y, 0.0f ) );
				}
			}

			//=============================================================================================
			public bool IsStickingOnLeftWall() {
				return m_stickingOnLeftWall;
			}

			//=============================================================================================
			bool CastWall( Vector2 _offset, Vector2 _direction ) {
				return Physics2D.RaycastNonAlloc(
					Instance.transform.position + new Vector3( _offset.x, _offset.y, 0.0f ),
					_direction,
					Instance.m_groundHitChecker,
					m_stickDistance,
					Physics2D.GetLayerCollisionMask( Instance.gameObject.layer ) ) != 0;
			}
		}
	}
}
