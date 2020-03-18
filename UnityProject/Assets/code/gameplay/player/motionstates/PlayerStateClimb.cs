// Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateClimb : StateMotion {
			//=============================================================================================
			[SerializeField, Tooltip( "Collision mask of the ladders" )]
			LayerMask m_ladderCollisionMask = 0;
			[SerializeField, Tooltip( "Speed to move up (unit/sec)" )]
			float m_climbUpSpeed = 2.0f;
			[SerializeField, Tooltip( "Speed to move down (unit/sec)" )]
			float m_climbDownSpeed = 3.0f;
			[SerializeField, Tooltip( "Speed to reach ladder center (unit/sec)" )]
			float m_alignSpeed = 5.0f;
			[SerializeField, Tooltip( "Collision mask of platforms" )]
			LayerMask m_platformsCollisionMask = 0;
			[Header( "Animation" )]
			[SerializeField, Tooltip( "Name of the animation of player standing on ladder" )]
			string m_idleAnimationName = "";
			[SerializeField, Tooltip( "Name of the animation of player climbing up a ladder" )]
			string m_climbUpAnimationName = "";
			[SerializeField, Tooltip( "Name of the animation of player climbing down a ladder" )]
			string m_climbDownAnimationName = "";

			int m_idleAnimationKey = 0;
			int m_climbUpAnimationKey = 0;
			int m_climbDownAnimationKey = 0;
			Collider2D[] m_ladderCasts = new Collider2D[ 4 ];
			Collider2D[] m_platformCast = new Collider2D[ 1 ];
			int m_castCount = 0;
			bool m_canClimbUp = false;
			bool m_canClimbDown = false;

			//=============================================================================================
			public override MotionState GetStateName() {
				return MotionState.Climb;
			}

			//=============================================================================================
			public override bool TryTransition( MotionState _fromState ) {
				if ( Game.InputManager[ InputManager.ActionName.Up ].state.state.justPressed ||
					Game.InputManager[ InputManager.ActionName.Down ].state.state.justPressed ) {

					if ( CastLadder() ) {
						Game.Player.SetState( GetStateName() );
						return true;
					}
				}

				return false;
			}

			//=============================================================================================
			public override void Awake() {
				m_idleAnimationKey = Animator.StringToHash( m_idleAnimationName );
				m_climbUpAnimationKey = Animator.StringToHash( m_climbUpAnimationName );
				m_climbDownAnimationKey = Animator.StringToHash( m_climbDownAnimationName );
			}

			//=============================================================================================
			public override void OnEnable() {
				Game.Player.m_animator.Play( m_idleAnimationKey );
				Game.Player.m_rigidbody.isKinematic = true;
				Game.Player.m_rigidbody.velocity = Vector2.zero;
			}

			//=============================================================================================
			public override void Update() {
				// Check state transitions
				if ( !CastPlatforms() ) {
					Game.Player.m_rigidbody.isKinematic = false;
					if ( Game.Player.GetMotionState( MotionState.Jump ).TryTransition( GetStateName() ) ||
						Game.Player.GetMotionState( MotionState.Run ).TryTransition( GetStateName() ) ) {
						return;
					}
					Game.Player.m_rigidbody.isKinematic = true;
				}

				if ( !CastLadder() ) {
					Game.Player.m_rigidbody.isKinematic = false;
					Game.Player.SetState( MotionState.Idle );
					return;
				}

				// Compute closest ladder
				Collider2D currentLadder = m_ladderCasts[ 0 ];
				Vector2 sqrDistFromLadder = Get2DSquareDistanceFrom3DBounds(
					currentLadder.bounds,
					Game.Player.transform.position );

				for ( int i = 1; i < m_castCount; ++i ) {
					Vector2 newSqrDist = Get2DSquareDistanceFrom3DBounds(
					m_ladderCasts[ i ].bounds,
					Game.Player.transform.position );

					if ( newSqrDist.x + newSqrDist.y < sqrDistFromLadder.x + sqrDistFromLadder.y ) {
						sqrDistFromLadder = newSqrDist;
						currentLadder = m_ladderCasts[ i ];
					}
				}

				float squareDistanceFromCenter = Game.Player.transform.position.x - currentLadder.bounds.center.x;
				squareDistanceFromCenter *= squareDistanceFromCenter;

				// Move toward the ladder center
				if ( !Mathf.Approximately( squareDistanceFromCenter, 0.0f ) ) {
					float motion = Time.deltaTime * m_alignSpeed;
					if ( motion * motion >= squareDistanceFromCenter ) {
						motion = Mathf.Sqrt( squareDistanceFromCenter );
					}
					Vector3 position = Game.Player.transform.position;
					if ( position.x > currentLadder.bounds.center.x ) {
						position.x -= motion;
					} else {
						position.x += motion;
					}
					Game.Player.transform.position = position;
					squareDistanceFromCenter = position.x - currentLadder.bounds.center.x;
					squareDistanceFromCenter *= squareDistanceFromCenter;
				}

				// Move up or down (or not)
				if ( Mathf.Approximately( squareDistanceFromCenter, 0.0f ) ) {
					float motion = 0.0f;

					if ( Game.InputManager[ InputManager.ActionName.Up ].state.state.isPressed && m_canClimbUp ) {
						if ( !Game.InputManager[ InputManager.ActionName.Down ].state.state.isPressed ) {
							motion += m_climbUpSpeed;
						}
					} else if ( Game.InputManager[ InputManager.ActionName.Down ].state.state.isPressed && m_canClimbDown ) {
						if ( !Game.InputManager[ InputManager.ActionName.Up ].state.state.isPressed ) {
							motion -= m_climbDownSpeed;
						}
					}

					if ( motion != 0.0f ) {
						Vector3 position = Game.Player.transform.position;
						position.y += motion * Time.deltaTime;
						Game.Player.transform.position = position;
					}
				}
			}

			//=============================================================================================
			bool CastLadder() {
				bool wasCastingTriggers = Physics2D.queriesHitTriggers;
				Physics2D.queriesHitTriggers = true;

				m_castCount = Physics2D.OverlapPointNonAlloc(
					VectorConverter.ToVector2( Game.Player.transform.position ) + new Vector2( 0.0f, -0.1f ),
					m_ladderCasts,
					m_ladderCollisionMask );

				m_canClimbDown = m_castCount != 0;

				int count = Physics2D.OverlapCapsuleNonAlloc(
					VectorConverter.ToVector2( Game.Player.transform.position ) + Game.Player.m_capsule.offset,
					Game.Player.m_capsule.size,
					Game.Player.m_capsule.direction,
					0.0f,
					m_ladderCasts,
					m_ladderCollisionMask );

				m_canClimbUp = count != 0;

				if ( count != 0 ) {
					m_castCount = count;
				}

				Physics2D.queriesHitTriggers = wasCastingTriggers;
				return m_canClimbUp || m_canClimbDown;
			}

			//=============================================================================================
			Vector2 Get2DSquareDistanceFrom3DBounds( Bounds _bounds, Vector3 _position ) {
				Vector3 boundsCenter = _bounds.center;
				float px = _position.x - boundsCenter.x;
				float py = _position.y - boundsCenter.y;
				px *= px;
				py *= py;

				Vector3 boundsSize = _bounds.size;
				float dx = px - boundsSize.x;
				float dy = py - boundsSize.y;

				return new Vector2( dx > 0.0f ? dx : 0.0f, dy > 0.0f ? dy : 0.0f );
			}

			//=============================================================================================
			bool CastPlatforms() {
				return Physics2D.OverlapCapsuleNonAlloc(
					VectorConverter.ToVector2( Game.Player.transform.position ) + Game.Player.m_capsule.offset,
					Game.Player.m_capsule.size,
					Game.Player.m_capsule.direction,
					0.0f,
					m_platformCast,
					m_platformsCollisionMask ) != 0;
			}
		}
	}
}
