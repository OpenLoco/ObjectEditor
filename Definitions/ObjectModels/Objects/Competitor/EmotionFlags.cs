namespace Definitions.ObjectModels.Objects.Competitor;

[Flags]
public enum EmotionFlags
{
	Neutral = 1 << 0,
	Happy = 1 << 1,
	Worried = 1 << 2,
	Thinking = 1 << 3,
	Dejected = 1 << 4,
	Surprised = 1 << 5,
	Scared = 1 << 6,
	Angry = 1 << 7,
	Disgusted = 1 << 8,
}
