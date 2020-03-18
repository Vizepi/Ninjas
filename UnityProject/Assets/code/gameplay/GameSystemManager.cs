// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public abstract class GameSystemManager : MonoBehaviour {
		public virtual void OnAwake() { }
		public virtual void OnStart() { }
		public virtual void OnUpdate() { }
		public virtual void OnLateUpdate() { }
		public virtual void OnFixedUpdate() { }
	}
}
