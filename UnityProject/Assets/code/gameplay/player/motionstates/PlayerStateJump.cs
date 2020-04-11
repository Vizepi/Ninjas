// Copyright 2019 J. KIEFFER - All Rights Reserved.
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
				bool doJump = false;
				Vector2 impulseDirection = Vector2.zero;

				if ( Game.Player.IsGrounded) {
					if ( Game.InputManager[ InputManager.ActionName.Jump ].state.state.justPressed ) {
						impulseDirection.y = m_jumpForce;
						m_hasExecutedFirstJump = false;
						doJump = true;
					}
				} else if ( _fromState == MotionState.StickWall ) {
					StateStickWall stateStickWall = Game.Player.GetMotionState( MotionState.StickWall ) as StateStickWall;

					if ( Game.InputManager[ InputManager.ActionName.Jump ].state.state.justPressed ) {
						impulseDirection.y = m_wallJumpForce * Mathf.Sin( Mathf.Deg2Rad * m_wallJumpAngle );
						impulseDirection.x = m_wallJumpForce * Mathf.Cos( Mathf.Deg2Rad * m_wallJumpAngle );
						doJump = true;
						m_hasExecutedFirstJump = false;
					} else if ( stateStickWall.IsStickingOnLeftWall() ) {
						if ( Game.InputManager[ InputManager.ActionName.Right ].state.state.justPressed ) {
							doJump = true;
							m_hasExecutedFirstJump = false;
						}
					} else {
						if ( Game.InputManager[ InputManager.ActionName.Left ].state.state.justPressed ) {
							doJump = true;
							m_hasExecutedFirstJump = false;
						}
					}

					if ( !stateStickWall.IsStickingOnLeftWall() ) {
						impulseDirection.x = -impulseDirection.x;
					}
				} else if ( _fromState == MotionState.Run || _fromState == MotionState.Idle ) {
					m_hasExecutedFirstJump = false;
					doJump = true;
				} else if ( _fromState == MotionState.Climb ) {
					if ( Game.InputManager[ InputManager.ActionName.Jump ].state.state.justPressed ) {
						impulseDirection.y = m_jumpForce;
						m_hasExecutedFirstJump = false;
						doJump = true;
					} else if ( Game.InputManager[ InputManager.ActionName.Left ].state.state.justPressed ||
						Game.InputManager[ InputManager.ActionName.Right ].state.state.justPressed ) {
						m_hasExecutedFirstJump = false;
						doJump = true;
					}
				}

				if ( doJump ) {
					ApplyJump( impulseDirection );
					Game.Player.SetState( GetStateName() );
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
				Game.Player.m_animator.Play( m_jumpAnimationKey );
			}

			//=============================================================================================
			public override void Update() {
				float motion = 0.0f;
				motion -= Game.InputManager[ InputManager.ActionName.Left ].state.state.isPressed ? 1.0f : 0.0f;
				motion += Game.InputManager[ InputManager.ActionName.Right ].state.state.isPressed ? 1.0f : 0.0f;

				Game.Player.m_motionController.SetHorizontalMotion( m_jumpAirSpeed, motion );

				m_afterJumpTimer -= Time.deltaTime;
				if ( m_afterJumpTimer <= 0.0f ) {
					if ( !m_hasExecutedFirstJump ) {
						if ( Game.InputManager[ InputManager.ActionName.Jump ].state.state.justPressed ) {
							m_hasExecutedFirstJump = true;
							ApplyJump( Vector2.up * m_doubleJumpForce );
						}
					}

					// Check state transitions
					if ( Game.Player.GetMotionState( MotionState.Idle ).TryTransition( GetStateName() ) ||
						Game.Player.GetMotionState( MotionState.Run ).TryTransition( GetStateName() ) ||
						Game.Player.GetMotionState( MotionState.StickWall ).TryTransition( GetStateName() ) ||
						Game.Player.GetMotionState( MotionState.Climb ).TryTransition( GetStateName() ) ) {
						return;
					}
				}
			}

			//=============================================================================================
			void ApplyJump( Vector2 _force ) {
				Game.Player.m_rigidbody.velocity = new Vector2( Game.Player.m_rigidbody.velocity.x, 0.0f );
				Game.Player.m_rigidbody.AddForce( _force, ForceMode2D.Impulse );
				m_afterJumpTimer = m_afterJumpDelay;
			}
		}
	}
}
