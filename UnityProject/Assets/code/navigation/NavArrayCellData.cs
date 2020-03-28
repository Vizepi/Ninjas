// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;

namespace vzp {
	//=============================================================================================
	[Flags]
	public enum NavArrayCellData : short {
		Empty = 0,
		GroundFlag = 0b00000001,
		CeilingFlag = 0b00000010,
		LeftWallFlag = 0b00000100,
		RightWallFlag = 0b00001000,
		LadderFlag = 0b00010000,
		HideoutFlag = 0b00100000,
		ThinGroundFlag = 0b01000000,
		ThinCeilingFlag = 0b10000000
	}

	public static class NavArrayCellDataExtension {
		//=============================================================================================
		public static bool HasGround( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.GroundFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasCeiling( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.CeilingFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasLeftWall( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.LeftWallFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasRightWall( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.RightWallFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasLadder( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.LadderFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasHideout( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.HideoutFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasThinGround( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.ThinGroundFlag ) != NavArrayCellData.Empty;
		}

		//=============================================================================================
		public static bool HasThinCeiling( this NavArrayCellData data ) {
			return ( data & NavArrayCellData.ThinCeilingFlag ) != NavArrayCellData.Empty;
		}
	}
}
