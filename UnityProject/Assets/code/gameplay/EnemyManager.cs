// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class EnemyManager : GameSystemManager {
		//=============================================================================================
		[SerializeField, Tooltip( "All the archetypes supported by the manager" )]
		EnemyArchetypeDescriptor[] m_enemyArchetypes = null;

		//=============================================================================================
		public const int kMaxEnemies = 100;

		//=============================================================================================
		public NavArray NavArray { get; private set; }

		//=============================================================================================
		Enemy[] m_enemies = new Enemy[ kMaxEnemies ];
		EnemySpawn[] m_spawns = null;
		int m_enemyCount = 0;

		//=============================================================================================
		public override void OnAwake() {
			NavArray[] navs = FindObjectsOfType<NavArray>();
			Debug.Assert( navs.Length == 1 );
			NavArray = navs[ 0 ];

			m_spawns = FindObjectsOfType<EnemySpawn>();
		}

		//=============================================================================================
		public override void OnUpdate() {
			for ( int i = 0; i < m_enemyCount; ++i ) {
				Debug.Assert( m_enemies[ i ] != null );
				m_enemies[ i ].OnUpdate();
			}
		}

		//=============================================================================================
		public override void OnLateUpdate() {
			for ( int i = 0; i < m_enemyCount; ++i ) {
				Debug.Assert( m_enemies[ i ] != null );
				m_enemies[ i ].OnLateUpdate();
			}
		}

		//=============================================================================================
		public override void OnFixedUpdate() {
			for ( int i = 0; i < m_enemyCount; ++i ) {
				Debug.Assert( m_enemies[ i ] != null );
				m_enemies[ i ].OnFixedUpdate();
			}
		}
	}
}
