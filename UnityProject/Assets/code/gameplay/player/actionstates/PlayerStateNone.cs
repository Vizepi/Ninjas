// Copyright 2019 J. KIEFFER - All Rights Reserved.
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
				Game.Player.SetState( GetStateName() );
				return true;
			}

			//=============================================================================================
			public override void Update() {
				if ( //Game.Player.GetActionState( ActionState.Attack ).TryTransition( GetStateName() ) ||
					 //Game.Player.GetActionState( ActionState.Jutsu ).TryTransition( GetStateName() ) ||
					Game.Player.GetActionState( ActionState.Hide ).TryTransition( GetStateName() ) /*||
					Game.Player.GetActionState( ActionState.Teleport ).TryTransition( GetStateName() )*/ ) {
					return;
				}
			}
		}
	}
}
