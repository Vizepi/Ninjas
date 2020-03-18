// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class Game : MonoBehaviour {
		static InputManager m_inputManager = null;
		static ScoreManager m_scoreManager = null;
		static RelicManager m_relicManager = null;
		static EnemyManager m_enemyManager = null;
		static CameraManager m_cameraManager = null;
		static Player m_player = null;

		//=============================================================================================
		public static InputManager InputManager {
			get { return m_inputManager; }
		}

		//=============================================================================================
		public static ScoreManager ScoreManager {
			get { return m_scoreManager; }
		}

		//=============================================================================================
		public static RelicManager RelicManager {
			get { return m_relicManager; }
		}

		//=============================================================================================
		public static EnemyManager EnemyManager {
			get { return m_enemyManager; }
		}

		//=============================================================================================
		public static CameraManager CameraManager {
			get { return m_cameraManager; }
		}

		//=============================================================================================
		public static Player Player {
			get { return m_player; }
		}

		//=============================================================================================
		void Awake() {
			// If this are not null, then the Game is instantiated multiple times
			Debug.Assert( m_inputManager == null );
			Debug.Assert( m_scoreManager == null );
			Debug.Assert( m_relicManager == null );
			Debug.Assert( m_enemyManager == null );
			Debug.Assert( m_cameraManager == null );
			Debug.Assert( m_player == null );

			m_inputManager = FindObjectOfType<InputManager>();
			m_scoreManager = FindObjectOfType<ScoreManager>();
			m_relicManager = FindObjectOfType<RelicManager>();
			m_enemyManager = FindObjectOfType<EnemyManager>();
			m_cameraManager = FindObjectOfType<CameraManager>();
			m_player = FindObjectOfType<Player>();

			Debug.Assert( m_inputManager );
			Debug.Assert( m_scoreManager );
			Debug.Assert( m_relicManager );
			Debug.Assert( m_enemyManager );
			Debug.Assert( m_cameraManager );
			Debug.Assert( m_player );

			m_inputManager.OnAwake();
			m_scoreManager.OnAwake();
			m_relicManager.OnAwake();
			m_enemyManager.OnAwake();
			m_cameraManager.OnAwake();
			m_player.OnAwake();
		}

		//=============================================================================================
		void Start() {
			m_inputManager.OnStart();
			m_scoreManager.OnStart();
			m_relicManager.OnStart();
			m_enemyManager.OnStart();
			m_cameraManager.OnStart();
			m_player.OnStart();
		}

		//=============================================================================================
		void Update() {
			m_inputManager.OnUpdate();
			m_player.OnUpdate();
			m_enemyManager.OnUpdate();
			m_relicManager.OnUpdate();
			m_cameraManager.OnUpdate();
			m_scoreManager.OnUpdate();
		}

		//=============================================================================================
		void LateUpdate() {
			m_inputManager.OnLateUpdate();
			m_player.OnLateUpdate();
			m_enemyManager.OnLateUpdate();
			m_relicManager.OnLateUpdate();
			m_cameraManager.OnLateUpdate();
			m_scoreManager.OnLateUpdate();
		}

		//=============================================================================================
		void FixedUpdate() {
			m_inputManager.OnFixedUpdate();
			m_player.OnFixedUpdate();
			m_enemyManager.OnFixedUpdate();
			m_relicManager.OnFixedUpdate();
			m_cameraManager.OnFixedUpdate();
			m_scoreManager.OnFixedUpdate();
		}
	}
}
