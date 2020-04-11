// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class EnemyHenchman : Enemy {
		//=============================================================================================
		[Header("Henchamn patrol")]
		[SerializeField, Tooltip( "Time during which the henchman waits when they reach the edge of a platform (s)" )]
		float m_stopTimeOnPlatformEdge = 1.0f;
		[SerializeField, Tooltip( "Distance from the edge of the platform at which to stop walking (m)" )]
		float m_stopDistanceFromPlatformEdge = 0.2f;

		float m_patrolStopTimer = 0.0f;

		//=============================================================================================
		protected override void OnPatrolUpdate() {
			if ( !IsGrounded ) {
				// Let enemy fall
				return;
			}

			// Enemy is walking
			switch ( m_currentPatrolState ) {
			case EnemyPatrolState.Walking:
				OnPatrolWalkingState();
				break;
			case EnemyPatrolState.Waiting:
				OnPatrolWaitingState();
				break;
			}
		}

		//=============================================================================================
		void OnPatrolWalkingState() {
			NavArrayCell currentCell = GetCurrentCell();
			NavArrayCell nextCell;
			bool canContinueWalking = true;

			if ( IsFacingLeft ) {
				nextCell = currentCell.Left;
				canContinueWalking = !currentCell.Data.HasRightWall();
			} else {
				nextCell = currentCell.Right;
				canContinueWalking = !currentCell.Data.HasLeftWall();
			}

			float distanceFromNextCell = nextCell.Distance( transform.position );
			canContinueWalking &= nextCell.Data.HasGround();

			if ( canContinueWalking && distanceFromNextCell > m_stopDistanceFromPlatformEdge ) {
				m_motionController.SetHorizontalMotion( m_patrolSpeed, IsFacingLeft ? -1.0f : 1.0f );
			} else {
				m_patrolStopTimer = m_stopTimeOnPlatformEdge;
				m_currentPatrolState = EnemyPatrolState.Waiting;
			}
		}

		//=============================================================================================
		void OnPatrolWaitingState() {
			NavArrayCell currentCell = GetCurrentCell();

		}
	}
}
