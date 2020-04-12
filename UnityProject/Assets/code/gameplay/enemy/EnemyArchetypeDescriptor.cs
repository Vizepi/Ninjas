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
		AnimationCurve m_probabilities = new AnimationCurve();

		//=============================================================================================
		public GameObject Prefab {
			get { return m_prefab; }
		}

		//=============================================================================================
		public float GetLerpedProbabilities( float _time ) {
			return m_probabilities.Evaluate( _time );
		}
	}
}
