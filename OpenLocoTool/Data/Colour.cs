namespace OpenLocoTool.Data
{
	public enum Colour : uint8_t
	{
		black = 0,
		grey = 1,
		white = 2,
		mutedDarkPurple = 3,
		mutedPurple = 4,
		purple = 5,
		darkBlue = 6,
		blue = 7,
		mutedDarkTeal = 8,
		mutedTeal = 9,
		darkGreen = 10,
		mutedSeaGreen = 11,
		mutedGrassGreen = 12,
		green = 13,
		mutedAvocadoGreen = 14,
		mutedOliveGreen = 15,
		yellow = 16,
		darkYellow = 17,
		orange = 18,
		amber = 19,
		darkOrange = 20,
		mutedDarkYellow = 21,
		mutedYellow = 22,
		brown = 23,
		mutedOrange = 24,
		mutedDarkRed = 25,
		darkRed = 26,
		red = 27,
		darkPink = 28,
		pink = 29,
		mutedRed = 30,
		max,
	};

	public enum ExtColour : uint8_t
	{
		black = 0,
		grey = 1,
		white = 2,
		mutedDarkPurple = 3,
		mutedPurple = 4,
		purple = 5,
		darkBlue = 6,
		blue = 7,
		mutedDarkTeal = 8,
		mutedTeal = 9,
		darkGreen = 10,
		mutedSeaGreen = 11,
		mutedGrassGreen = 12,
		green = 13,
		mutedAvocadoGreen = 14,
		mutedOliveGreen = 15,
		yellow = 16,
		darkYellow = 17,
		orange = 18,
		amber = 19,
		darkOrange = 20,
		mutedDarkYellow = 21,
		mutedYellow = 22,
		brown = 23,
		mutedOrange = 24,
		mutedDarkRed = 25,
		darkRed = 26,
		red = 27,
		darkPink = 28,
		pink = 29,
		mutedRed = 30,
		// First 30 are inherited from Colour
		clear = 31, // No colour
		@null = 32,  // Does not represent any palette
		unk21,
		unk22,
		unk23,
		unk24,
		unk25,
		unk26,
		unk27,
		unk28,
		unk29,
		unk2A,
		unk2B,
		unk2C, // ghost
		unk2D,
		unk2E, // translucentGlass1
		unk2F, // translucentGlass2
		unk30, // translucentGlass0
		unk31, // translucentGhost
		unk32, // shadow
		unk33,
		unk34,
		translucentGrey1, // 0-1 black, grey
		translucentGrey2,
		translucentGrey0,
		translucentBlue1, // 6-7 darkBlue, blue
		translucentBlue2,
		translucentBlue0,
		translucentMutedDarkRed1, // 25 mutedDarkRed
		translucentMutedDarkRed2,
		translucentMutedDarkRed0,
		translucentMutedSeaGreen1, // 11 mutedSeaGreen
		translucentMutedSeaGreen2,
		translucentMutedSeaGreen0,
		translucentMutedPurple1, // 3-4 mutedDarkPurple, mutedPurple
		translucentMutedPurple2,
		translucentMutedPurple0,
		translucentMutedOliveGreen1, // 15 mutedOliveGreen
		translucentMutedOliveGreen2,
		translucentMutedOliveGreen0,
		translucentMutedYellow1, // 21-22 mutedDarkYellow, mutedYellow
		translucentMutedYellow2,
		translucentMutedYellow0,
		translucentYellow1, // 16-17 yellow, darkYellow
		translucentYellow2,
		translucentYellow0,
		translucentMutedGrassGreen1, // 12 mutedGrassGreen
		translucentMutedGrassGreen2,
		translucentMutedGrassGreen0,
		translucentMutedAvocadoGreen1, // 14 mutedAvocadoGreen
		translucentMutedAvocadoGreen2,
		translucentMutedAvocadoGreen0,
		translucentGreen1, // 10, 13 darkGreen, green
		translucentGreen2,
		translucentGreen0,
		translucentMutedOrange1, // 24 mutedOrange
		translucentMutedOrange2,
		translucentMutedOrange0,
		translucentPurple1, // 5 purple
		translucentPurple2,
		translucentPurple0,
		translucentRed1, // 26-27 darkRed, red
		translucentRed2,
		translucentRed0,
		translucentOrange1, // 18, 20 orange, darkOrange
		translucentOrange2,
		translucentOrange0,
		translucentMutedTeal1, // 8-9 mutedDarkTeal, mutedTeal
		translucentMutedTeal2,
		translucentMutedTeal0,
		translucentPink1, // 28-29 pink, darkPink
		translucentPink2,
		translucentPink0,
		translucentBrown1, // 23 brown
		translucentBrown2,
		translucentBrown0,
		translucentMutedRed1, // 30 mutedRed
		translucentMutedRed2,
		translucentMutedRed0,
		translucentWhite1, // 2 white
		translucentWhite2,
		translucentWhite0,
		translucentAmber1, // 19 amber
		translucentAmber2,
		translucentAmber0,
		unk74,
		unk75,
		unk76,
		unk77,
		unk78,
		unk79,
		unk7A,
		unk7B,
		unk7C,
		unk7D,
		unk7E,
		unk7F,
		unk80,
		unk81,
		unk82,
		unk83,
		unk84,
		unk85,
		unk86,
		unk87,
		unk88,
		unk89,
		unk8A,
		unk8B,
		unk8C,
		unk8D,
		unk8E,
		unk8F,
		unk90,
		unk91,
		unk92,
		max,
	};
}
