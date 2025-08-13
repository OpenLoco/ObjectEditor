namespace Definitions.ObjectModels.Types;

public enum Colour : uint8_t // copy DatColour for now
{
	Black = 0,
	Grey = 1,
	White = 2,
	MutedDarkPurple = 3,
	MutedPurple = 4,
	Purple = 5,
	DarkBlue = 6,
	Blue = 7,
	MutedDarkTeal = 8,
	MutedTeal = 9,
	DarkGreen = 10,
	MutedSeaGreen = 11,
	MutedGrassGreen = 12,
	Green = 13,
	MutedAvocadoGreen = 14,
	MutedOliveGreen = 15,
	Yellow = 16,
	DarkYellow = 17,
	Orange = 18,
	Amber = 19,
	DarkOrange = 20,
	MutedDarkYellow = 21,
	MutedYellow = 22,
	Brown = 23,
	MutedOrange = 24,
	MutedDarkRed = 25,
	DarkRed = 26,
	Red = 27,
	DarkPink = 28,
	Pink = 29,
	MutedRed = 30,
	MAX,
}

public enum ExtColour : uint8_t // copy DatExtColour for now
{
	Black = 0,
	Grey = 1,
	White = 2,
	MutedDarkPurple = 3,
	MutedPurple = 4,
	Purple = 5,
	DarkBlue = 6,
	Blue = 7,
	MutedDarkTeal = 8,
	MutedTeal = 9,
	DarkGreen = 10,
	MutedSeaGreen = 11,
	MutedGrassGreen = 12,
	Green = 13,
	MutedAvocadoGreen = 14,
	MutedOliveGreen = 15,
	Yellow = 16,
	DarkYellow = 17,
	Orange = 18,
	Amber = 19,
	DarkOrange = 20,
	MutedDarkYellow = 21,
	MutedYellow = 22,
	Brown = 23,
	MutedOrange = 24,
	MutedDarkRed = 25,
	DarkRed = 26,
	Red = 27,
	DarkPink = 28,
	Pink = 29,
	MutedRed = 30,
	// First 30 are inherited from Colour
	clear = 31, // No colour
	NULL = 32,  // Does not represent any palette
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
	TranslucentGrey1, // 0-1 black, grey
	TranslucentGrey2,
	TranslucentGrey0,
	TranslucentBlue1, // 6-7 darkBlue, blue
	TranslucentBlue2,
	TranslucentBlue0,
	TranslucentMutedDarkRed1, // 25 mutedDarkRed
	TranslucentMutedDarkRed2,
	TranslucentMutedDarkRed0,
	TranslucentMutedSeaGreen1, // 11 mutedSeaGreen
	TranslucentMutedSeaGreen2,
	TranslucentMutedSeaGreen0,
	TranslucentMutedPurple1, // 3-4 mutedDarkPurple, mutedPurple
	TranslucentMutedPurple2,
	TranslucentMutedPurple0,
	TranslucentMutedOliveGreen1, // 15 mutedOliveGreen
	TranslucentMutedOliveGreen2,
	TranslucentMutedOliveGreen0,
	TranslucentMutedYellow1, // 21-22 mutedDarkYellow, mutedYellow
	TranslucentMutedYellow2,
	TranslucentMutedYellow0,
	TranslucentYellow1, // 16-17 yellow, darkYellow
	TranslucentYellow2,
	TranslucentYellow0,
	TranslucentMutedGrassGreen1, // 12 mutedGrassGreen
	TranslucentMutedGrassGreen2,
	TranslucentMutedGrassGreen0,
	TranslucentMutedAvocadoGreen1, // 14 mutedAvocadoGreen
	TranslucentMutedAvocadoGreen2,
	TranslucentMutedAvocadoGreen0,
	TranslucentGreen1, // 10, 13 darkGreen, green
	TranslucentGreen2,
	TranslucentGreen0,
	TranslucentMutedOrange1, // 24 mutedOrange
	TranslucentMutedOrange2,
	TranslucentMutedOrange0,
	TranslucentPurple1, // 5 purple
	TranslucentPurple2,
	TranslucentPurple0,
	TranslucentRed1, // 26-27 darkRed, red
	TranslucentRed2,
	TranslucentRed0,
	TranslucentOrange1, // 18, 20 orange, darkOrange
	TranslucentOrange2,
	TranslucentOrange0,
	TranslucentMutedTeal1, // 8-9 mutedDarkTeal, mutedTeal
	TranslucentMutedTeal2,
	TranslucentMutedTeal0,
	TranslucentPink1, // 28-29 pink, darkPink
	TranslucentPink2,
	TranslucentPink0,
	TranslucentBrown1, // 23 brown
	TranslucentBrown2,
	TranslucentBrown0,
	TranslucentMutedRed1, // 30 mutedRed
	TranslucentMutedRed2,
	TranslucentMutedRed0,
	TranslucentWhite1, // 2 white
	TranslucentWhite2,
	TranslucentWhite0,
	TranslucentAmber1, // 19 amber
	TranslucentAmber2,
	TranslucentAmber0,
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
	MAX,
}
