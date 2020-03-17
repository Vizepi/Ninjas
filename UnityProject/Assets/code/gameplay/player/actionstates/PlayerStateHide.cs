// © Copyright 2019 J. KIEFFER - All Rights Reserved.
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
				Out
			}

			//=============================================================================================
			[SerializeField, Tooltip( "Collision mask of the hideout" )]
			LayerMask m_collisionMask = 0;
			[SerializeField, Tooltip( "Animation to play when entering the hideout" )]
			string m_enterAnimation;
			[SerializeField, Tooltip( "Time for the player to hide" )]
			float m_hidingTime = 0.1f;

			Collider2D[] m_colliders = new Collider2D[ 1 ];
			State m_hideState = State.Out;
			float m_timer = 0.0f;

			//=============================================================================================
			public override ActionState GetStateName() {
				return ActionState.Hide;
			}

			//=============================================================================================
			public override void Awake() {
				Debug.Assert( !string.IsNullOrWhiteSpace( m_enterAnimation ) );
			}

			//=============================================================================================
			public override bool TryTransition( ActionState _fromState ) {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				if ( inputs[ InputManager.ActionName.Hide ].state.state.justPressed ) {
					if ( CastHideout() ) {
						Instance.SetState( GetStateName() );
						return true;
					}
				}

				return false;
			}

			//=============================================================================================
			public override void OnEnable() {
				Instance.m_animator.Play( m_enterAnimation );
				Instance.m_rigidbody.isKinematic = true;
				Instance.m_rigidbody.velocity = Vector2.zero;
				m_timer = m_hidingTime;
				m_hideState = State.Hiding;
			}

			//=============================================================================================
			public override void Update() {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				switch ( m_hideState ) {
				case State.Hiding:
					m_timer -= Time.deltaTime;
					if ( m_timer <= 0.0f ) {
						m_hideState = State.Hidden;
					}
					break;
				case State.Hidden:
					Instance.m_rigidbody.isKinematic = false;
					if ( Instance.GetActionState( ActionState.Attack ).TryTransition( GetStateName() ) ||
						Instance.GetActionState( ActionState.Jutsu ).TryTransition( GetStateName() ) ) {
						m_hideState = State.Out;
						return;
					}
					if ( Instance.GetMotionState( MotionState.Run ).TryTransition( MotionState.Idle ) ||
						Instance.GetMotionState( MotionState.Jump ).TryTransition( MotionState.Idle ) ||
						Instance.GetMotionState( MotionState.Climb ).TryTransition( MotionState.Idle ) ) {
						Instance.SetState( ActionState.None );
						m_hideState = State.Out;
						return;
					}
					Instance.m_rigidbody.isKinematic = true;
					break;
				case State.Out:
					Instance.SetState( ActionState.None );
					break;
				}
			}

			//=============================================================================================
			bool CastHideout() {
				bool wasCastingTriggers = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;

				return ( Physics2D.OverlapPointNonAlloc(
							VectorConverter.ToVector2( Instance.transform.position ) + Instance.m_capsule.offset,
							m_colliders,
							m_collisionMask ) != 0 );
			}
		}
	}
}
