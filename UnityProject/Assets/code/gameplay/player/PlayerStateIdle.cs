// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateIdle : State {
			//=============================================================================================
			[SerializeField, Tooltip( "Name of the idle animation" )]
			string m_idleAnimationName;

			int m_idleAnimationKey;

			//=============================================================================================
			public override StateName GetStateName() {
				return StateName.Idle;
			}

			//=============================================================================================
			public override void Awake() {
				m_idleAnimationKey = Animator.StringToHash( m_idleAnimationName );
			}

			//=============================================================================================
			public override void Start() { }

			//=============================================================================================
			public override void OnEnable() {
				PlayerInstance.m_animator.Play( m_idleAnimationKey );
			}

			//=============================================================================================
			public override void OnDisable() { }

			//=============================================================================================
			public override void Update() { }

			//=============================================================================================
			public override void FixedUpdate() { }

			//=============================================================================================
			public override void LateUpdate() { }
		}
	}
}
