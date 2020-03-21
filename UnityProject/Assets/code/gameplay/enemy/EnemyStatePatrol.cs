// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public abstract partial class Enemy {
		//=============================================================================================
		void PatrolUpdate() {
			OnPatrolUpdate();
		}

		//=============================================================================================
		void PatrolLateUpdate() {

		}

		//=============================================================================================
		void PatrolFixedUpdate() {

		}
	}
}
