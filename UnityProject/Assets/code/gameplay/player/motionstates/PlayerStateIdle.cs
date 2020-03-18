// Copyright 2019 J. KIEFFER - All Rights Reserved.
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
				if ( !Game.Player.IsGrounded||
					Game.InputManager[ InputManager.ActionName.Left ].state.state.isPressed ||
					Game.InputManager[ InputManager.ActionName.Right ].state.state.isPressed ||
					Game.InputManager[ InputManager.ActionName.Up ].state.state.isPressed ||
					Game.InputManager[ InputManager.ActionName.Down ].state.state.isPressed ||
					Game.InputManager[ InputManager.ActionName.Jump ].state.state.isPressed ) {
					// There is a motion query, move, climb or jump
					return false;
				}

				Game.Player.SetState( GetStateName() );

				return true;
			}

			//=============================================================================================
			public override void Awake() {
				m_idleAnimationKey = Animator.StringToHash( m_idleAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Game.Player.m_animator.Play( m_idleAnimationKey );
			}

			//=============================================================================================
			public override void Update() {
				// Check state transitions
				if ( Game.Player.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ||
					Game.Player.GetMotionState( MotionState.Climb ).TryTransition( GetStateName() ) ||
					Game.Player.GetMotionState( MotionState.Run ).TryTransition( GetStateName() ) ) {
					return;
				}
			}
		}
	}
}
