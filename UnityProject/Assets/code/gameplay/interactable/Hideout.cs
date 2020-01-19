// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class Hideout : MonoBehaviour {
		//=============================================================================================
		[SerializeField, Tooltip( "Relative position where the character goes when he is hidden" )]
		Vector2 m_hiddenPosition;
		[SerializeField, Tooltip( "Relative position of the player after he goes out of the hideout" )]
		Vector2 m_outPosition;
		[SerializeField, Tooltip( "Animation to play when entering the hideout" )]
		string m_enterAnimation;
		[SerializeField, Tooltip( "Animation to play when levaing the hideout" )]
		string m_leaveAnimation;

		//=============================================================================================
		public Vector2 HiddenPosition {
			get {
				return m_hiddenPosition;
			}
		}

		//=============================================================================================
		public Vector2 OutPosition {
			get {
				return m_outPosition;
			}
		}

		//=============================================================================================
		public string EnterAnimation {
			get {
				return m_enterAnimation;
			}
		}

		//=============================================================================================
		public string LeaveAnimation {
			get {
				return m_leaveAnimation;
			}
		}

		//=============================================================================================
		private void Awake() {
			Debug.Assert( !string.IsNullOrWhiteSpace( m_enterAnimation ) );
			Debug.Assert( !string.IsNullOrWhiteSpace( m_leaveAnimation ) );
		}

		//=============================================================================================
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere( transform.position + VectorConverter.ToVector3( m_hiddenPosition ), 0.05f );
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere( transform.position + VectorConverter.ToVector3( m_outPosition ), 0.05f );
		}
	}
}
