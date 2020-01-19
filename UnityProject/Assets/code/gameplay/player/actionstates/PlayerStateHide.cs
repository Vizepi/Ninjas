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
				Leaving,
				Out
			}

			//=============================================================================================
			[SerializeField, Tooltip( "Collision mask of the hideout" )]
			LayerMask m_collisionMask = 0;
			[SerializeField, Tooltip( "Time for the player to hide" )]
			float m_hidingTime = 0.1f;
			[SerializeField, Tooltip( "Time for the player to leave the hideout" )]
			float m_leavingTime = 0.1f;

			Collider2D[] m_colliders = new Collider2D[ 1 ];
			Hideout m_currentHideout = null;
			State m_hideState = State.Out;
			float m_timer = 0.0f;

			//=============================================================================================
			public override ActionState GetStateName() {
				return ActionState.Hide;
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
				Instance.m_animator.Play( m_currentHideout.EnterAnimation );
				Instance.m_rigidbody.isKinematic = true;
				Instance.m_rigidbody.velocity = Vector2.zero;
				m_timer = m_hidingTime;
			}

			//=============================================================================================
			public override void Update() {
				InputManager inputs = InputManager.Instance;
				Debug.Assert( inputs );

				switch ( m_hideState ) {
				case State.Hiding:
					break;
				case State.Hidden:
					Debug.Assert( m_currentHideout );
					Instance.m_animator.Play( m_currentHideout.LeaveAnimation );
					break;
				case State.Leaving:
					break;
				case State.Out:
					if ( Instance.GetActionState( ActionState.None ).TryTransition( GetStateName() ) ) {
						return;
					}
					break;
				}
			}

			//=============================================================================================
			bool CastHideout() {
				bool wasCastingTriggers = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;

				if ( Physics2D.OverlapCapsuleNonAlloc(
					VectorConverter.ToVector2( Instance.transform.position ) + Instance.m_capsule.offset,
					Instance.m_capsule.size,
					CapsuleDirection2D.Vertical,
					0.0f,
					m_colliders,
					m_collisionMask ) != 0 ) {

					m_currentHideout = m_colliders[ 0 ].GetComponent<Hideout>();
					Debug.Assert( m_currentHideout );

					return m_currentHideout != null;
				}

				return false;
			}
		}
	}
}
