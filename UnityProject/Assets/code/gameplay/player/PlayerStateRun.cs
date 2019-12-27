// © Copyright 2019 J. KIEFFER - All Rights Reserved.
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
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				if ( Instance.IsGrounded() && (
					inputs[ InputManager.ActionName.Left ].state.state.isPressed ||
					inputs[ InputManager.ActionName.Right ].state.state.isPressed ) ) {

					Instance.SetState( GetStateName() );
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
				Instance.m_animator.Play( m_runAnimationKey );
				m_currentMotion = 1.0f;
			}

			//=============================================================================================
			public override void Update() {
				if ( !Instance.IsGrounded() ) {
					if ( //Instance.GetMotionState( StateName.Climb ).TryTransition() ||
						Instance.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ) {
						return;
					}
					return;
				}

				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				float motion = 0.0f;
				motion -= inputs[ InputManager.ActionName.Left ].state.state.isPressed ? 1.0f : 0.0f;
				motion += inputs[ InputManager.ActionName.Right ].state.state.isPressed ? 1.0f : 0.0f;

				Instance.SetHorizontalMotion( m_runningSpeed, motion );

				if ( motion == 0.0f ) {
					if ( Mathf.Approximately( Instance.m_rigidbody.velocity.x, 0.0f ) ) {
						// Check state transitions
						if ( Instance.GetMotionState( MotionState.Idle ).TryTransition( GetStateName() ) ) {
							return;
						}
					} else if ( m_currentMotion != 0.0f ) {
						Instance.m_animator.Play( m_brakeAnimationKey );
					}
				}

				// Check if player is jumping or climbing
				if ( Instance.GetMotionState( MotionState.Climb ).TryTransition( GetStateName() ) ||
					Instance.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ) {
					return;
				}

				m_currentMotion = motion;
			}
		}
	}
}
