﻿// Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateHide : StateAction {
			//=============================================================================================
			enum State {
				Hiding,
				Hidden,
				Unhiding,
				Out
			}

			//=============================================================================================
			[SerializeField, Tooltip( "Collision mask of the hideout" )]
			LayerMask m_collisionMask = 0;
			[SerializeField, Tooltip( "Animation to play when entering the hideout" )]
			string m_enterAnimation;
			[SerializeField, Tooltip( "Animation to play when leaving the hideout" )]
			string m_leaveAnimation;
			[SerializeField, Tooltip( "Time for the player to hide" )]
			float m_hidingTime = 0.1f;
			[SerializeField, Tooltip( "Time for the player to un-hide" )]
			float m_unhidingTime = 0.0f;

			Collider2D[] m_colliders = new Collider2D[ 1 ];
			State m_hideState = State.Out;
			float m_timer = 0.0f;

			//=============================================================================================
			public override ActionState GetStateName() {
				return ActionState.Hide;
			}

			//=============================================================================================
			public override void Awake() {
				//Debug.Assert( !string.IsNullOrWhiteSpace( m_enterAnimation ) );
			}

			//=============================================================================================
			public override bool TryTransition( ActionState _fromState ) {
				if ( Game.InputManager[ InputManager.ActionName.Hide ].state.state.justPressed ) {
					if ( CastHideout() ) {
						Game.Player.SetState( GetStateName() );
						return true;
					}
				}

				return false;
			}

			//=============================================================================================
			public override void OnEnable() {
				Game.Player.m_rigidbody.isKinematic = true;
				Game.Player.m_rigidbody.velocity = Vector2.zero;
				StartHiding();
			}

			//=============================================================================================
			public override void Update() {
				switch ( m_hideState ) {
				case State.Hiding:
					m_timer -= Time.deltaTime; // TODO jkieffer - Use GameTime
					if ( m_timer <= 0.0f ) {
						m_hideState = State.Hidden;
					}
					break;
				case State.Hidden:
					Game.Player.m_rigidbody.isKinematic = false;
					/*if ( Game.Player.GetActionState( ActionState.Attack ).TryTransition( GetStateName() ) ||
						Game.Player.GetActionState( ActionState.Jutsu ).TryTransition( GetStateName() ) ) {
						StartUnhiding();
						return;
					}*/
					if ( Game.Player.GetMotionState( MotionState.Run ).TryTransition( MotionState.Idle ) ||
						Game.Player.GetMotionState( MotionState.Jump ).TryTransition( MotionState.Idle ) ||
						Game.Player.GetMotionState( MotionState.Climb ).TryTransition( MotionState.Idle ) ) {
						Game.Player.SetState( ActionState.None );
						StartUnhiding();
						return;
					}
					Game.Player.m_rigidbody.isKinematic = true;
					break;
				case State.Unhiding:
					m_timer -= Time.deltaTime; // TODO jkieffer - Use GameTime
					if ( m_timer <= 0.0f ) {
						m_hideState = State.Out;
					}
					break;
				case State.Out:
					Game.Player.SetState( ActionState.None );
					break;
				}
			}

			//=============================================================================================
			void StartHiding() {
				//Game.Player.m_animator.Play( m_enterAnimation );
				m_hideState = State.Hiding;
				m_timer = m_hidingTime;
			}

			//=============================================================================================
			void StartUnhiding() {
				//Game.Player.m_animator.Play( m_leaveAnimation );
				m_hideState = State.Unhiding;
				m_timer = m_unhidingTime;
			}

			//=============================================================================================
			bool CastHideout() {
				bool wasCastingTriggers = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;

				return ( Physics2D.OverlapPointNonAlloc(
							VectorConverter.ToVector2( Game.Player.transform.position ) + Game.Player.m_capsule.offset,
							m_colliders,
							m_collisionMask ) != 0 );
			}
		}
	}
}
