// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public abstract partial class Enemy : MonoBehaviour {
		public enum State {
			Patrol,
			Detect,
			Chase,
			Attack
		}

		//=============================================================================================
		[Header( "Movement" )]
		[SerializeField, Tooltip( "The speed of the enemy when patroling (m/s)" )]
		float m_patrolSpeed = 1.0f;
		[SerializeField, Tooltip( "The speed of the enemy when chasing (m/s)" )]
		float m_chasingSpeed = 2.0f;
		[SerializeField, Tooltip( "The time after which the enemy give up when they don't see the player (s)" )]
		float m_giveUpTime = 3.0f;

		[Header( "Detection" )]
		[SerializeField, Tooltip( "The range of detection of the enemy (m)" )]
		float m_detectionRange = 3.0f;
		[SerializeField, Tooltip( "Time for the enemy to detect the player when player is in sight (s)" )]
		float m_detectionTime = 1.5f;

		[Header( "Attack" )]
		[SerializeField, Tooltip( "Time before the enemy attack when in range of the player (s)" )]
		float m_preAttackDelay = 0.5f;
		[SerializeField, Tooltip( "Time after the enemy attack before returning to chasing (s)" )]
		float m_postAttackDelay = 0.5f;
		[SerializeField, Tooltip( "Damage of the attack of the enemy (% of player's life)" )]
		float m_attackDamage = 0.5f;
		[SerializeField, Tooltip( "Range at which the enemy freezes before attacking (m)" )]
		float m_attackRange = 20.0f;

		[Header( "Physics" )]
		[SerializeField, Tooltip( "Distance from the player's feet where we are considered grounded (unit)" )]
		float m_groundDistance = 0.05f;
		[SerializeField, Tooltip( "Collision layers used for gond detection" )]
		LayerMask m_groundLayer = 0;

		[Header( "Scoring" )]
		[SerializeField, Tooltip( "Score obtained when killed" )]
		int m_killScore = 100;

		protected State m_currentState = State.Patrol;
		protected Rigidbody2D m_rigidbody = null;
		RaycastHit2D[] m_groundHitChecker = new RaycastHit2D[ 1 ];

		//=============================================================================================
		public bool IsFacingLeft { get; protected set; }
		public bool IsGrounded { get; private set; }

		#region Virtuals
		//=============================================================================================
		protected virtual void OnPatrolUpdate() { }
		protected virtual void OnPatrolLateUpdate() { }
		protected virtual void OnPatrolFixedUpdate() { }

		//=============================================================================================
		protected virtual void OnDetectUpdate() { }
		protected virtual void OnDetectLateUpdate() { }
		protected virtual void OnDetectFixedUpdate() { }

		//=============================================================================================
		protected virtual void OnChaseUpdate() { }
		protected virtual void OnChaseLateUpdate() { }
		protected virtual void OnChaseFixedUpdate() { }

		//=============================================================================================
		protected virtual void OnAttackUpdate() { }
		protected virtual void OnAttackLateUpdate() { }
		protected virtual void OnAttackFixedUpdate() { }

		//=============================================================================================
		protected virtual void OnDetectionStart() { }
		protected virtual void OnDetectionEnd() { }

		//=============================================================================================
		protected virtual void OnChasingStart() { }
		protected virtual void OnChasingEnd() { }

		//=============================================================================================
		protected virtual void OnFreezeBeforeAttack() { }
		protected virtual void OnAttack() { }
		protected virtual void OnFreezeAfterAttack() { }
		protected virtual void OnResumeChasing() { }

		//=============================================================================================
		protected virtual void OnLosingSight() { }
		protected virtual void OnGivingUp() { }

		//=============================================================================================
		protected virtual void OnSpawned() { }
		protected virtual void OnKillingPlayer() { }
		protected virtual void OnKilled() { }
		#endregion

		//=============================================================================================
		public void OnUpdate() {
			switch ( m_currentState ) {
			case State.Patrol:
				OnPatrolUpdate();
				break;
			case State.Detect:
				OnDetectUpdate();
				break;
			case State.Chase:
				OnChaseUpdate();
				break;
			case State.Attack:
				OnAttackUpdate();
				break;
			}


			IsGrounded = Physics2D.RaycastNonAlloc(
				m_rigidbody.position,
				Vector2.down,
				m_groundHitChecker,
				m_groundDistance,
				m_groundLayer ) != 0;
		}

		//=============================================================================================
		public void OnLateUpdate() {
			switch ( m_currentState ) {
			case State.Patrol:
				OnPatrolLateUpdate();
				break;
			case State.Detect:
				OnDetectLateUpdate();
				break;
			case State.Chase:
				OnChaseLateUpdate();
				break;
			case State.Attack:
				OnAttackLateUpdate();
				break;
			}
		}

		//=============================================================================================
		public void OnFixedUpdate() {
			switch ( m_currentState ) {
			case State.Patrol:
				OnPatrolFixedUpdate();
				break;
			case State.Detect:
				OnDetectFixedUpdate();
				break;
			case State.Chase:
				OnChaseFixedUpdate();
				break;
			case State.Attack:
				OnAttackFixedUpdate();
				break;
			}
		}

		//=============================================================================================
		protected NavArrayCell GetCurrentCell() {
			NavArrayCell? currentCell = Game.EnemyManager.NavArray.GetCell( transform.position );
			Debug.Assert( currentCell.HasValue, "[ENEMY] Enemy is outside the NavArray broadphase." );
			return currentCell.Value;
		}

#if UNITY_EDITOR
		//=============================================================================================
		protected virtual void OnDrawGizmosCustom( bool _selected ) { }

		//=============================================================================================
		void OnDrawGizmosCommon() {
		}

		//=============================================================================================
		void OnDrawGizmos() {
			Color color = Color.magenta;
			color.a = 0.25f;
			Gizmos.color = color;
			OnDrawGizmosCommon();
			OnDrawGizmosCustom( false );
		}

		//=============================================================================================
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.magenta;
			OnDrawGizmosCommon();
			OnDrawGizmosCustom( true );
		}
#endif // UNITY_EDITOR
	}
}
