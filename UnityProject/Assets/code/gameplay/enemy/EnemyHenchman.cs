// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class EnemyHenchman : Enemy {
		protected override void OnPatrolUpdate() {
			NavArrayCell currentCell = GetCurrentCell();
			if ( !currentCell.Data.HasGround() ) {
				// Let enemy fall
				return;
			}
		}
	}
}
