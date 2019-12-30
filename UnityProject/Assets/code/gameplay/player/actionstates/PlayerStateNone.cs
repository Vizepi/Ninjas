// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;

namespace vzp {
	public partial class Player {
		[Serializable]
		public class StateNone : StateAction {
			//=============================================================================================
			public override ActionState GetStateName() {
				return ActionState.None;
			}

			//=============================================================================================
			public override bool TryTransition( ActionState _fromState ) {
				Instance.SetState( GetStateName() );
				return true;
			}

			//=============================================================================================
			public override void Update() {
				if ( Instance.GetActionState( ActionState.Attack ).TryTransition( GetStateName() ) ||
					Instance.GetActionState( ActionState.Jutsu ).TryTransition( GetStateName() ) ||
					Instance.GetActionState( ActionState.Hide ).TryTransition( GetStateName() ) ||
					Instance.GetActionState( ActionState.Teleport ).TryTransition( GetStateName() ) ) {
					return;
				}
			}
		}
	}
}
