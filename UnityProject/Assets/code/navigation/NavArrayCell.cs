// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;

namespace vzp {
	[Flags]
	public enum NavArrayCell : short {
		Empty = 0,
		IsGround = 0b00000001,
		IsCeiling = 0b00000010,
		HasLeftWall = 0b00000100,
		HasRightWall = 0b00001000,
		HasLadder = 0b00010000,
		HasHideout = 0b00100000,
		IsGroundThin = 0b01000000,
		IsCeilingThin = 0b10000000
	}
}
