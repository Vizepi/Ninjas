// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateJump : StateMotion {
			//=============================================================================================
			[SerializeField, Tooltip( "Speed of horizontal motion in air after a jump" )]
			float m_jumpAirSpeed = 3.0f;
			[SerializeField, Tooltip( "Force of grounded jump impulse" )]
			float m_jumpForce = 300.0f;
			[SerializeField, Tooltip( "Force of double jump impulse" )]
			float m_doubleJumpForce = 200.0f;
			[SerializeField, Tooltip( "Force of a jump from a wall" )]
			float m_wallJumpForce = 300.0f;
			[SerializeField, Tooltip( "Angle at which to jump from wall (deg) [0-90]. 0 is vertical, 90 is horizontal" )]
			float m_wallJumpAngle = 40.0f;
			[SerializeField, Tooltip( "Delay after jump when we can look for a double jump or a motion state change (sec)" )]
			float m_afterJumpDelay = 0.1f;
			[Header( "Animation" )]
			[SerializeField, Tooltip( "Name of the jump animation" )]
			string m_jumpAnimationName = "";
			[SerializeField, Tooltip( "Name of the double jump animation" )]
			string m_doubleJumpAnimationName = "";

			int m_jumpAnimationKey = 0;
			int m_doubleJumpAnimationKey = 0;

			bool m_hasExecutedFirstJump = false;
			float m_afterJumpTimer = 0.0f;

			//=============================================================================================
			public override MotionState GetStateName() {
				return MotionState.Jump;
			}

			//=============================================================================================
			public override bool TryTransition( MotionState _fromState ) {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				bool doJump = false;
				Vector2 impulseDirection = Vector2.zero;

				if ( Instance.IsGrounded() ) {
					if ( inputs[InputManager.ActionName.Jump ].state.state.justPressed ) {
						impulseDirection.y = m_jumpForce;
						m_hasExecutedFirstJump = false;
						doJump = true;
					}
				} else if ( _fromState == MotionState.StickWall ) {
					impulseDirection.y = m_wallJumpForce * Mathf.Sin( Mathf.Deg2Rad * m_wallJumpAngle );
					impulseDirection.x = m_wallJumpForce * Mathf.Cos( Mathf.Deg2Rad * m_wallJumpAngle );
					StateStickWall stateStickWall = Instance.GetMotionState( MotionState.StickWall ) as StateStickWall;
					if ( !stateStickWall.IsStickingOnLeftWall() ) {
						impulseDirection.x = -impulseDirection.x;
					}
					m_hasExecutedFirstJump = true;
					doJump = true;
				}

				if ( doJump ) {
					ApplyJump( impulseDirection );
					m_afterJumpTimer = m_afterJumpDelay;
					Instance.SetState( GetStateName() );
				}

				return doJump;
			}

			//=============================================================================================
			public override void Awake() {
				m_jumpAnimationKey = Animator.StringToHash( m_jumpAnimationName );
				m_doubleJumpAnimationKey = Animator.StringToHash( m_doubleJumpAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Instance.m_animator.Play( m_jumpAnimationKey );
			}

			//=============================================================================================
			public override void Update() {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				float motion = 0.0f;
				motion -= inputs[ InputManager.ActionName.Left ].state.state.isPressed ? 1.0f : 0.0f;
				motion += inputs[ InputManager.ActionName.Right ].state.state.isPressed ? 1.0f : 0.0f;

				Instance.SetHorizontalMotion( m_jumpAirSpeed, motion );

				m_afterJumpTimer -= Time.deltaTime;
				if ( m_afterJumpTimer <= 0.0f ) {
					if ( !m_hasExecutedFirstJump ) {
						if ( inputs[ InputManager.ActionName.Jump ].state.state.justPressed ) {
							m_hasExecutedFirstJump = true;
							ApplyJump( Vector2.up * m_doubleJumpForce );
						}
					}

					// Check state transitions
					if ( Instance.GetMotionState( MotionState.Idle ).TryTransition( GetStateName() ) ||
						Instance.GetMotionState( MotionState.Run ).TryTransition( GetStateName() ) ) {
						return;
					}
				}
			}

			//=============================================================================================
			void ApplyJump( Vector2 _force ) {
				Instance.m_rigidbody.velocity = new Vector2( Instance.m_rigidbody.velocity.x, 0.0f );
				Instance.m_rigidbody.AddForce( _force, ForceMode2D.Impulse );
			}
		}
	}
}
