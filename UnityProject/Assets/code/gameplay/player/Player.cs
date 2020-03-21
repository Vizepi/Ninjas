// Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public partial class Player : GameSystemManager {
		//=============================================================================================
		const int kMaxCollidingColliders = 32;
		const int kMaxContactsPerCollider = 32;

		//=============================================================================================
		public enum MotionState {
			Idle,
			Run,
			Jump,
			StickWall,
			Climb,

			// Keep at the end and up to date
			FirstMotionState = Idle,
			LastMotionState = Climb
		}

		//=============================================================================================
		public enum ActionState {
			None,
			Attack,
			Jutsu,
			Hide,
			Teleport,

			// Keep at the end and up to date
			FirstActionState = None,
			LastActionState = Teleport
		}

		//=============================================================================================
		public enum StateTransitionOrder {
			DisableThenEnable,
			EnableThenDisable,
			EnableNoDisable,
			DisableNoEnable,
			NoEnableNoDisable
		}

		//=============================================================================================
		public abstract class State {
			public abstract bool IsMotionState();
			public abstract bool IsActionState();
			public virtual void Awake() { }
			public virtual void Start() { }
			public virtual void OnEnable() { }
			public virtual void OnDisable() { }
			public virtual void Update() { }
			public virtual void LateUpdate() { }
			public virtual void FixedUpdate() { }
			public virtual void OnDrawGizmos() { }
		}

		//=============================================================================================
		public abstract class StateMotion : State {
			public override bool IsMotionState() { return true; }
			public override bool IsActionState() { return false; }
			public abstract MotionState GetStateName();
			// WARNING !!! Always return immediately from caller state if this returns true
			public abstract bool TryTransition( MotionState _fromState );
		}

		//=============================================================================================
		public abstract class StateAction : State {
			public override bool IsMotionState() { return false; }
			public override bool IsActionState() { return true; }
			public abstract ActionState GetStateName();
			public abstract bool TryTransition( ActionState _fromState );
		}

		//=============================================================================================
		[Header( "Attributes" )]
		[SerializeField, Tooltip( "Number of lifes of the player" )]
		int m_lifes = 2;

		[Header( "Motion states" )]
		[SerializeField, Tooltip( "Idle state" )]
		StateIdle m_stateIdle = null;
		[SerializeField, Tooltip( "Run state" )]
		StateRun m_stateRun = null;
		[SerializeField, Tooltip( "Jump state" )]
		StateJump m_stateJump = null;
		[SerializeField, Tooltip( "Stick wall state" )]
		StateStickWall m_stateStickWall = null;
		[SerializeField, Tooltip( "Climb state" )]
		StateClimb m_stateClimb = null;

		[Header( "Action states" )]
		[SerializeField, Tooltip( "None state" )]
		StateNone m_stateNone = null;
		[SerializeField, Tooltip( "Hide state" )]
		StateHide m_stateHide = null;

		[Header( "Motion" )]
		[SerializeField, Tooltip( "Time to reach full speed (sec)" )]
		float m_fullSpeedTime = 0.3f;
		[SerializeField, Tooltip( "Time to reach zero speed (sec)" )]
		float m_zeroSpeedTime = 0.15f;

		[Header( "Physics" )]
		[SerializeField, Tooltip( "Distance from the player's feet where we are considered grounded (unit)" )]
		float m_groundDistance = 0.05f;
		[SerializeField, Tooltip( "Collision layers used for gond detection" )]
		LayerMask m_groundLayer = 0;

		[Header( "View" )]
		[SerializeField, Tooltip( "The camera of the player" )]
		PlayerCamera m_camera = null;

		//=============================================================================================
		StateMotion[] m_motionStates = new StateMotion[ ( int )MotionState.LastMotionState - ( int )MotionState.FirstMotionState + 1 ];
		StateAction[] m_actionStates = new StateAction[ ( int )ActionState.LastActionState - ( int )ActionState.FirstActionState + 1 ];
		MotionState m_currentMotionState = MotionState.Idle;
		ActionState m_currentActionState = ActionState.None;
		Animator m_animator = null;
		Rigidbody2D m_rigidbody = null;
		CapsuleCollider2D m_capsule = null;
		RaycastHit2D[] m_groundHitChecker = new RaycastHit2D[ 1 ];
		bool m_isGrounded = true;
		float m_horizontalMotion = 0.0f;
		float m_horizontalMotionDirection = 0.0f;

		//=============================================================================================
		public MotionState CurrentMotionState {
			get { return m_currentMotionState; }
		}

		//=============================================================================================
		public ActionState CurrentActionState {
			get { return m_currentActionState; }
		}

		//=============================================================================================
		public bool IsGrounded {
			get { return m_isGrounded; }
		}

		//=============================================================================================
		public override void OnAwake() {
			m_animator = GetComponent<Animator>();
			Debug.Assert( m_animator );
			m_rigidbody = GetComponent<Rigidbody2D>();
			Debug.Assert( m_rigidbody );
			m_capsule = GetComponent<CapsuleCollider2D>();
			Debug.Assert( m_capsule );

			m_motionStates[ ( int )MotionState.Idle ] = m_stateIdle;
			m_motionStates[ ( int )MotionState.Run ] = m_stateRun;
			m_motionStates[ ( int )MotionState.Jump ] = m_stateJump;
			m_motionStates[ ( int )MotionState.StickWall ] = m_stateStickWall;
			m_motionStates[ ( int )MotionState.Climb ] = m_stateClimb;
			foreach ( State state in m_motionStates ) {
				Debug.Assert( state != null );
				state.Awake();
			}

			m_actionStates[ ( int )ActionState.None ] = m_stateNone;
			m_actionStates[ ( int )ActionState.Attack ] = null;
			m_actionStates[ ( int )ActionState.Jutsu ] = null;
			m_actionStates[ ( int )ActionState.Hide ] = m_stateHide;
			m_actionStates[ ( int )ActionState.Teleport ] = null;
			foreach ( State state in m_actionStates ) {
				// TODO jkieffer - Enable this assert when states are ready, and remove the ?. from all calls
				// Debug.Assert( state != null );
				state?.Awake();
			}
			SetState( m_currentMotionState, StateTransitionOrder.EnableNoDisable, true );
			SetState( m_currentActionState, StateTransitionOrder.EnableNoDisable, true );

			Debug.Assert( m_camera != null );
			m_camera.Awake();
		}

		//=============================================================================================
		public override void OnStart() {
			foreach ( State state in m_motionStates ) {
				state.Start();
			}
			foreach ( State state in m_actionStates ) {
				state?.Start();
			}
		}

		//=============================================================================================
		public override void OnUpdate() {
			m_motionStates[ ( int )m_currentMotionState ]?.Update();
			m_actionStates[ ( int )m_currentActionState ]?.Update();

			ApplyMotion();
			m_isGrounded = Physics2D.RaycastNonAlloc(
				m_rigidbody.position,
				Vector2.down,
				m_groundHitChecker,
				m_groundDistance,
				m_groundLayer ) != 0;

			m_camera.Update();
		}

		//=============================================================================================
		public override void OnLateUpdate() {
			m_motionStates[ ( int )m_currentMotionState ].LateUpdate();
			m_actionStates[ ( int )m_currentActionState ]?.LateUpdate();
		}

		//=============================================================================================
		public override void OnFixedUpdate() {
			m_motionStates[ ( int )m_currentMotionState ].FixedUpdate();
			m_actionStates[ ( int )m_currentActionState ]?.FixedUpdate();
		}

		//=============================================================================================
		void OnDrawGizmos() {
			m_motionStates[ ( int )m_currentMotionState ]?.OnDrawGizmos();
			m_actionStates[ ( int )m_currentActionState ]?.OnDrawGizmos();

			switch ( m_currentMotionState ) {
			case MotionState.Idle:
				Gizmos.color = Color.blue;
				break;
			case MotionState.Run:
				Gizmos.color = Color.cyan;
				break;
			case MotionState.Jump:
				Gizmos.color = Color.green;
				break;
			case MotionState.StickWall:
				Gizmos.color = Color.red;
				break;
			case MotionState.Climb:
				Gizmos.color = Color.yellow;
				break;
			}
			Gizmos.DrawLine( transform.position, transform.position + Vector3.up );

			switch ( m_currentActionState ) {
			case ActionState.None:
				Gizmos.color = Color.blue;
				break;
			case ActionState.Attack:
				Gizmos.color = Color.cyan;
				break;
			case ActionState.Jutsu:
				Gizmos.color = Color.green;
				break;
			case ActionState.Hide:
				Gizmos.color = Color.red;
				break;
			case ActionState.Teleport:
				Gizmos.color = Color.yellow;
				break;
			}
			Gizmos.DrawLine(
				transform.position + Vector3.up * 0.5f - Vector3.left * 0.25f,
				transform.position + Vector3.up * 0.5f + Vector3.left * 0.25f );
		}

		//=============================================================================================
		void OnDrawGizmosSelected() {
			foreach ( State state in m_motionStates ) {
				state?.OnDrawGizmos();
			}
			foreach ( State state in m_actionStates ) {
				state?.OnDrawGizmos();
			}
		}

		//=============================================================================================
		public void SetState( MotionState _state, StateTransitionOrder _transition = StateTransitionOrder.DisableThenEnable, bool _forceTransition = false ) {
			SetStateGeneric( _state, ref m_currentMotionState, ( int )_state, ( int )m_currentMotionState, m_motionStates, _transition, _forceTransition );
		}

		//=============================================================================================
		public void SetState( ActionState _state, StateTransitionOrder _transition = StateTransitionOrder.DisableThenEnable, bool _forceTransition = false ) {
			SetStateGeneric( _state, ref m_currentActionState, ( int )_state, ( int )m_currentMotionState, m_actionStates, _transition, _forceTransition );
		}

		//=============================================================================================
		void SetStateGeneric<T,U>( T _state, ref T _currentState, int _iState, int _iCurrentState, U[] _states, StateTransitionOrder _transition, bool _forceTransition ) where U : State {
			// TODO jkieffer - Remove this if once all states are implemented
			if ( _states[ _iState ] == null ) { return; }

			if ( _iState != _iCurrentState && !_forceTransition ) {
				switch ( _transition ) {
				case StateTransitionOrder.DisableThenEnable:
					_states[ _iCurrentState ]?.OnDisable();
					_states[ _iState ]?.OnEnable();
					break;
				case StateTransitionOrder.EnableThenDisable:
					_states[ _iState ]?.OnEnable();
					_states[ _iCurrentState ]?.OnDisable();
					break;
				case StateTransitionOrder.EnableNoDisable:
					_states[ _iState ]?.OnEnable();
					break;
				case StateTransitionOrder.DisableNoEnable:
					_states[ _iCurrentState ]?.OnDisable();
					break;
				case StateTransitionOrder.NoEnableNoDisable:
					break;
				}
				_currentState = _state;
			}
		}

		//=============================================================================================
		public StateMotion GetMotionState( MotionState _state ) {
			return m_motionStates[ ( int )_state ];
		}

		//=============================================================================================
		public StateAction GetActionState( ActionState _state ) {
			return m_actionStates[ ( int )_state ];
		}

		//=============================================================================================
		public void SetHorizontalMotion( float _motion, float _direction ) {
			m_horizontalMotion = _motion;
			m_horizontalMotionDirection = _direction;
		}

		//=============================================================================================
		void ApplyMotion() {
			float currentXMotion = m_rigidbody.velocity.x;
			float currentXMotionSign = Mathf.Sign( currentXMotion );
			if ( m_horizontalMotionDirection == 0.0f ) {
				// Reduce speed by deceleration factor
				float speedScaleFactor = m_horizontalMotion * Time.deltaTime / m_zeroSpeedTime;

				if ( Mathf.Abs( currentXMotion ) < speedScaleFactor ) {
					currentXMotion = 0.0f;
				} else {
					currentXMotion -= currentXMotionSign * speedScaleFactor;
				}

				m_rigidbody.velocity = new Vector2( currentXMotion, m_rigidbody.velocity.y );

			} else {
				if ( currentXMotion != 0.0f && m_horizontalMotionDirection != currentXMotionSign ) {
					float speedScaleFactor = m_horizontalMotion * Time.deltaTime / m_zeroSpeedTime;
					currentXMotion -= currentXMotionSign * speedScaleFactor;
				}
				float accelerationFactor = m_horizontalMotion * Time.deltaTime / m_fullSpeedTime;
				currentXMotion += m_horizontalMotionDirection * accelerationFactor;
				currentXMotion = Mathf.Clamp( currentXMotion, -m_horizontalMotion, m_horizontalMotion );

				m_rigidbody.velocity = new Vector2( currentXMotion, m_rigidbody.velocity.y );
			}
			m_horizontalMotionDirection = 0.0f;
		}
	}
}
