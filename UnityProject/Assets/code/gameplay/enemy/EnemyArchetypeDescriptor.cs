// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	[CreateAssetMenu( fileName = "EnemyArchetype", menuName = "Ninja/Enemy archetype" )]
	public class EnemyArchetypeDescriptor : ScriptableObject {
		//=============================================================================================
		[SerializeField, Tooltip( "Prefab of the ingame enemy" )]
		GameObject m_prefab = null;
		[SerializeField, Tooltip( "Probabilities of spawn over time" )]
		EnemySpawnProbability[] m_probabilities = new EnemySpawnProbability[ 0 ];

		//=============================================================================================
		public GameObject Prefab {
			get { return m_prefab; }
		}

		//=============================================================================================
		void Awake() {
			Array.Sort( m_probabilities, new EnemySpawnProbabilityComparer() );
		}

		//=============================================================================================
		public float GetLerpedProbabilities( float _time ) {
			if ( _time >= m_probabilities[ m_probabilities.Length - 1 ].time ) {
				return m_probabilities[ m_probabilities.Length - 1 ].probability;
			}

			for ( int i = m_probabilities.Length - 1; i >= 0; --i ) {
				if ( m_probabilities[ i ].time >= _time ) {
					float t = ( _time - m_probabilities[ i ].time ) / ( m_probabilities[ i + 1 ].time - m_probabilities[ i ].time );
					return Mathf.Lerp( m_probabilities[ i ].probability, m_probabilities[ i + 1 ].probability, t );
				}
			}

			return m_probabilities[ 0 ].probability;
		}
	}
}
