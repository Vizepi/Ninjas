// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class NavGraph : MonoBehaviour {
		//=============================================================================================
		[System.Flags]
		public enum Cell {
			IsEmpty			= 0b00000001,
			IsGround		= 0b00000010,
			IsCeiling		= 0b00000100,
			HasLeftWall		= 0b00001000,
			HasRightWall	= 0b00010000,
			HasLadder		= 0b00100000,
			IsHideout		= 0b01000000
		}

		//=============================================================================================
		public const int kNodeSize = 10;

		//=============================================================================================
		class Node {
			public const int kBottomLeft = 1;
			public const int kBottomRight = 2;
			public const int kTopLeft = 3;
			public const int kTopRight = 4;

			Vector2Int m_position = Vector2Int.zero;
			Vector2Int m_center = Vector2Int.zero;
			Vector2Int m_size = Vector2Int.zero;
			Node[] m_children = null;
			Cell[,] m_cells = null;

			//=============================================================================================
			public bool IsLeaf {
				get { return m_children == null; }
			}

			//=============================================================================================
			public Node( Vector2Int _position, Vector2Int _center, Vector2Int _size, Node[] _children ) {
				m_position = _position;
				m_children = _children;
				m_center = _center;
				m_size = _size;
			}

			//=============================================================================================
			public Node( Vector2Int _position, Cell[,] _cells ) {
				m_position = _position;
				m_cells = _cells;
			}
		}

		//=============================================================================================
		[SerializeField, Tooltip( "The rectangle containing all the nodes of the graph" )]
		RectInt m_broadphase = new RectInt( -1, -1, 2, 2 );



#if UNITY_EDITOR
		//=============================================================================================
		void OnDrawGizmosCommon() {
			// Broadphase
			Vector3 p1 = new Vector3( m_broadphase.xMin * kNodeSize, m_broadphase.yMin * kNodeSize, 0.0f );
			Vector3 p2 = new Vector3( m_broadphase.xMin * kNodeSize, m_broadphase.yMax * kNodeSize, 0.0f );
			Vector3 p3 = new Vector3( m_broadphase.xMax * kNodeSize, m_broadphase.yMax * kNodeSize, 0.0f );
			Vector3 p4 = new Vector3( m_broadphase.xMax * kNodeSize, m_broadphase.yMin * kNodeSize, 0.0f );
			Gizmos.DrawLine( p1, p2 );
			Gizmos.DrawLine( p2, p3 );
			Gizmos.DrawLine( p3, p4 );
			Gizmos.DrawLine( p4, p1 );
		}

		//=============================================================================================
		void OnDrawGizmos() {
			Color color = Color.red;
			color.a = 0.25f;
			Gizmos.color = color;
			OnDrawGizmosCommon();
		}

		//=============================================================================================
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.red;
			OnDrawGizmosCommon();
		}
#endif // UNITY_EDITOR
	}
}
