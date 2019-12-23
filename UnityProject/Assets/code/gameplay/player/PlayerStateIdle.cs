// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateIdle : StateMotion {
			//=============================================================================================
			[Header( "Animation" )]
			[SerializeField, Tooltip( "Name of the idle animation" )]
			string m_idleAnimationName = "";

			int m_idleAnimationKey = 0;

			//=============================================================================================
			public override MotionState GetStateName() {
				return MotionState.Idle;
			}

			//=============================================================================================
			public override bool TryTransition( MotionState _fromState ) {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				if ( !Instance.IsGrounded() ||
					( inputs[ InputManager.ActionName.Left ].state.state.isPressed ) ||
					( inputs[ InputManager.ActionName.Right ].state.state.isPressed ) ||
					( inputs[ InputManager.ActionName.Up ].state.state.isPressed ) ||
					( inputs[ InputManager.ActionName.Down ].state.state.isPressed ) ||
					( inputs[ InputManager.ActionName.Jump ].state.state.isPressed ) ) {
					// There is a motion query, move, climb or jump
					return false;
				}

				Instance.SetState( GetStateName() );

				return true;
			}

			//=============================================================================================
			public override void Awake() {
				m_idleAnimationKey = Animator.StringToHash( m_idleAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Instance.m_animator.Play( m_idleAnimationKey );
			}

			//=============================================================================================
			public override void Update() {
				// Check state transitions
				if ( Instance.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ||
					 //Instance.GetMotionState( StateName.Climb ).TryTransition() ||
					Instance.GetMotionState( MotionState.Run ).TryTransition( GetStateName() ) ) {
					return;
				}
			}
		}
	}
}
