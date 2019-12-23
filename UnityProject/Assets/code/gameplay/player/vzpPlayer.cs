// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

#pragma warning disable 649

namespace vzp {
	public partial class Player : MonoBehaviour {
		//=============================================================================================
		public enum StateName : int {
			Idle,
			Run,
			Jump,
			DoubleJump,
			AttackKatana,
			AttackShuriken,
			Hide,
			Teleport
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
			public abstract StateName GetStateName();

			public abstract void Awake();
			public abstract void Start();
			public abstract void OnEnable();
			public abstract void OnDisable();
			public abstract void Update();
			public abstract void LateUpdate();
			public abstract void FixedUpdate();
		}

		//=============================================================================================
		[Header( "States" )]
		[SerializeField, Tooltip( "Idle state" )]
		State m_stateIdle = null;

		//=============================================================================================
		State[] m_states = new State[ Enum.GetNames( typeof( StateName ) ).Length ];
		StateName m_currentState = StateName.Idle;
		Animator m_animator = null;

		//=============================================================================================
		public static Player PlayerInstance {
			get; private set;
		}

		//=============================================================================================
		private void Awake() {
			Debug.Assert( PlayerInstance == null );
			PlayerInstance = this;

			m_animator = GetComponent<Animator>();
			Debug.Assert( m_animator );

			m_states[ ( int )StateName.Idle ] = m_stateIdle;
			foreach ( State state in m_states ) {
				Debug.Assert( state != null );
				state.Awake();
			}
		}

		//=============================================================================================
		void Start() {
			foreach ( State state in m_states ) {
				state?.Start();
			}
		}

		//=============================================================================================
		void OnDestroy() {
			if ( PlayerInstance == this ) {
				PlayerInstance = null;
			}
		}

		//=============================================================================================
		void Update() {
			m_states[ ( int )m_currentState ]?.Update();
		}

		//=============================================================================================
		void LateUpdate() {
			m_states[ ( int )m_currentState ]?.LateUpdate();
		}

		//=============================================================================================
		void FixedUpdate() {
			m_states[ ( int )m_currentState ]?.FixedUpdate();
		}

		//=============================================================================================
		public void SetState( StateName _state, StateTransitionOrder _transition = StateTransitionOrder.DisableThenEnable, bool _forceTransition = false ) {
			// TODO jkieffer - Remove this if once all states are implemented
			if ( m_states[ ( int )_state ] == null ) { return; }

			if ( _state != m_currentState && !_forceTransition ) {
				switch ( _transition ) {
				case StateTransitionOrder.DisableThenEnable:
					m_states[ ( int )m_currentState ]?.OnDisable();
					m_states[ ( int )_state ]?.OnEnable();
					break;
				case StateTransitionOrder.EnableThenDisable:
					m_states[ ( int )_state ]?.OnEnable();
					m_states[ ( int )m_currentState ]?.OnDisable();
					break;
				case StateTransitionOrder.EnableNoDisable:
					m_states[ ( int )_state ]?.OnEnable();
					break;
				case StateTransitionOrder.DisableNoEnable:
					m_states[ ( int )m_currentState ]?.OnDisable();
					break;
				case StateTransitionOrder.NoEnableNoDisable:
					break;
				}
				m_currentState = _state;
			}
		}

		//=============================================================================================
		public State GetState( StateName _state ) {
			return m_states[ ( int )_state ];
		}

		//=============================================================================================
		public StateName GetCurrentState() {
			return m_currentState;
		}
	}
}

#pragma warning restore 649
