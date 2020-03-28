// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class Game : MonoBehaviour {
		//=============================================================================================
		public static InputManager InputManager { get; private set; }
		public static ScoreManager ScoreManager { get; private set; }
		public static RelicManager RelicManager { get; private set; }
		public static EnemyManager EnemyManager { get; private set; }
		public static CameraManager CameraManager { get; private set; }
		public static Player Player { get; private set; }

		//=============================================================================================
		void Awake() {
			// If this are not null, then the Game is instantiated multiple times
			Debug.Assert( InputManager == null );
			Debug.Assert( ScoreManager == null );
			Debug.Assert( RelicManager == null );
			Debug.Assert( EnemyManager == null );
			Debug.Assert( CameraManager == null );
			Debug.Assert( Player == null );

			InputManager = FindObjectOfType<InputManager>();
			ScoreManager = FindObjectOfType<ScoreManager>();
			RelicManager = FindObjectOfType<RelicManager>();
			EnemyManager = FindObjectOfType<EnemyManager>();
			CameraManager = FindObjectOfType<CameraManager>();
			Player = FindObjectOfType<Player>();

			Debug.Assert( InputManager );
			Debug.Assert( ScoreManager );
			Debug.Assert( RelicManager );
			Debug.Assert( EnemyManager );
			Debug.Assert( CameraManager );
			Debug.Assert( Player );

			InputManager.OnAwake();
			ScoreManager.OnAwake();
			RelicManager.OnAwake();
			EnemyManager.OnAwake();
			CameraManager.OnAwake();
			Player.OnAwake();
		}

		//=============================================================================================
		void Start() {
			InputManager.OnStart();
			ScoreManager.OnStart();
			RelicManager.OnStart();
			EnemyManager.OnStart();
			CameraManager.OnStart();
			Player.OnStart();
		}

		//=============================================================================================
		void Update() {
			InputManager.OnUpdate();
			Player.OnUpdate();
			EnemyManager.OnUpdate();
			RelicManager.OnUpdate();
			CameraManager.OnUpdate();
			ScoreManager.OnUpdate();
		}

		//=============================================================================================
		void LateUpdate() {
			InputManager.OnLateUpdate();
			Player.OnLateUpdate();
			EnemyManager.OnLateUpdate();
			RelicManager.OnLateUpdate();
			CameraManager.OnLateUpdate();
			ScoreManager.OnLateUpdate();
		}

		//=============================================================================================
		void FixedUpdate() {
			InputManager.OnFixedUpdate();
			Player.OnFixedUpdate();
			EnemyManager.OnFixedUpdate();
			RelicManager.OnFixedUpdate();
			CameraManager.OnFixedUpdate();
			ScoreManager.OnFixedUpdate();
		}
	}
}
