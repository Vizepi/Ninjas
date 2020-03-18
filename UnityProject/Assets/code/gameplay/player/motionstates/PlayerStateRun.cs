// Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateRun : StateMotion {
			//=============================================================================================
			[SerializeField, Tooltip( "Running speed (unit/sec)" )]
			float m_runningSpeed = 4.0f;
			[Header( "Animation" )]
			[SerializeField, Tooltip( "Name of the run animation" )]
			string m_runAnimationName = "";
			[SerializeField, Tooltip( "Name of the brake animation" )]
			string m_brakeAnimationName = "";

			int m_runAnimationKey = 0;
			int m_brakeAnimationKey = 0;
			float m_currentMotion = 0.0f;

			//=============================================================================================
			public override MotionState GetStateName() {
				return MotionState.Run;
			}

			public override bool TryTransition( MotionState _fromState ) {
				if ( Game.Player.IsGrounded&& (
					Game.InputManager[ InputManager.ActionName.Left ].state.state.isPressed ||
					Game.InputManager[ InputManager.ActionName.Right ].state.state.isPressed ) ) {

					Game.Player.SetState( GetStateName() );
					return true;
				}

				return false;
			}

			//=============================================================================================
			public override void Awake() {
				m_runAnimationKey = Animator.StringToHash( m_runAnimationName );
				m_brakeAnimationKey = Animator.StringToHash( m_brakeAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Game.Player.m_animator.Play( m_runAnimationKey );
				m_currentMotion = 1.0f;
			}

			//=============================================================================================
			public override void Update() {
				if ( !Game.Player.IsGrounded) {
					if ( //Instance.GetMotionState( StateName.Climb ).TryTransition() ||
						Game.Player.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ) {
						return;
					}
					return;
				}

				float motion = 0.0f;
				motion -= Game.InputManager[ InputManager.ActionName.Left ].state.state.isPressed ? 1.0f : 0.0f;
				motion += Game.InputManager[ InputManager.ActionName.Right ].state.state.isPressed ? 1.0f : 0.0f;

				Game.Player.SetHorizontalMotion( m_runningSpeed, motion );

				if ( motion == 0.0f ) {
					if ( Mathf.Approximately( Game.Player.m_rigidbody.velocity.x, 0.0f ) ) {
						// Check state transitions
						if ( Game.Player.GetMotionState( MotionState.Idle ).TryTransition( GetStateName() ) ) {
							return;
						}
					} else if ( m_currentMotion != 0.0f ) {
						Game.Player.m_animator.Play( m_brakeAnimationKey );
					}
				}

				// Check if player is jumping or climbing
				if ( Game.Player.GetMotionState( MotionState.Climb ).TryTransition( GetStateName() ) ||
					Game.Player.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ) {
					return;
				}

				m_currentMotion = motion;
			}
		}
	}
}
