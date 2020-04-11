// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.Collections;
using UnityEngine;

namespace vzp {
	[Serializable]
	public struct EnemySpawnProbability {
		[Tooltip( "Time at which this probability takes place (s)" )]
		public float time;
		[Tooltip( "Probability of this type of enemy to spawn (points)" )]
		public float probability;
	}

	public struct EnemySpawnProbabilityComparer : IComparer {
		public int Compare( object x, object y ) {
			return ( ( EnemySpawnProbability )x ).time.CompareTo( ( ( EnemySpawnProbability )y ).time );
		}
	}
}
