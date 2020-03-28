// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public struct NavArrayCell {
		//=============================================================================================
		public NavArray Array { get; private set; }
		public Vector2Int Coordinates { get; private set; }
		public NavArrayCellData Data { get; private set; }

		//=============================================================================================
		public NavArrayCell? Left {
			get { return GetAdjacentCellGeneric( Coordinates + Vector2Int.left ); }
		}

		//=============================================================================================
		public NavArrayCell? Right {
			get { return GetAdjacentCellGeneric( Coordinates + Vector2Int.right ); }
		}

		//=============================================================================================
		public NavArrayCell? Up {
			get { return GetAdjacentCellGeneric( Coordinates + Vector2Int.up ); }
		}

		//=============================================================================================
		public NavArrayCell? Down {
			get { return GetAdjacentCellGeneric( Coordinates + Vector2Int.down ); }
		}

		//=============================================================================================
		public NavArrayCell? UpLeft {
			get { return GetAdjacentCellGeneric( Coordinates + new Vector2Int( -1, 1 ) ); }
		}

		//=============================================================================================
		public NavArrayCell? UpRight {
			get { return GetAdjacentCellGeneric( Coordinates + new Vector2Int( 1, 1 ) ); }
		}

		//=============================================================================================
		public NavArrayCell? DownLeft {
			get { return GetAdjacentCellGeneric( Coordinates + new Vector2Int( -1, -1 ) ); }
		}

		//=============================================================================================
		public NavArrayCell? DownRight {
			get { return GetAdjacentCellGeneric( Coordinates + new Vector2Int( 1, -1 ) ); }
		}

		//=============================================================================================
		public NavArrayCell( NavArray _array, Vector2Int _coordinates, NavArrayCellData _data ) {
			Array = _array;
			Coordinates = _coordinates;
			Data = _data;
		}

		//=============================================================================================
		NavArrayCell? GetAdjacentCellGeneric( Vector2Int _newCoordinates ) {
			NavArrayCellData? adjacent = Array.GetCellData( _newCoordinates );
			return adjacent.HasValue ? new NavArrayCell( Array, _newCoordinates, adjacent.Value ) : ( NavArrayCell? )null;
		}
	}
}
