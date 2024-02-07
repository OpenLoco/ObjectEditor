using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum SteamObjectFlags : uint16_t
	{
		None = 0,
		ApplyWind = 1 << 0,
		DisperseOnCollision = 1 << 1,
		unk2 = 1 << 2,
		unk3 = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]
	public class ImageAndHeight(
		uint8_t imageOffset,
		uint8_t height)
		: ILocoStruct
	{
		[LocoStructOffset(0x00)] public uint8_t ImageOffset { get; set; } = imageOffset;
		[LocoStructOffset(0x01)] public uint8_t Height { get; set; } = height;

		public static ImageAndHeight Read(ReadOnlySpan<byte> data)
			=> new(data[0], data[1]);
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	[LocoStructType(ObjectType.Steam)]
	[LocoStringTable("Name")]
	public class SteamObject(
		uint16_t numImages,
		uint8_t numStationaryTicks,
		SteamObjectFlags flags,
		uint32_t var_0A,
		uint8_t numSoundEffects)
		: ILocoStruct, ILocoStructVariableData
	{
		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02), LocoPropertyMaybeUnused] public uint16_t NumImages { get; set; } = numImages; // this is simply the count of images in the graphics table
		[LocoStructOffset(0x04)] public uint8_t NumStationaryTicks { get; set; } = numStationaryTicks;
		//[LocoStructOffset(0x05)] public uint8_t SpriteWidth { get; set; } = spriteWidth;
		//[LocoStructOffset(0x06)] public uint8_t SpriteHeightNegative { get; set; } = spriteHeightNegative;
		//[LocoStructOffset(0x07)] public uint8_t SpriteHeightPositive { get; set; } = spriteHeightPositive;
		[LocoStructOffset(0x08)] public SteamObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x0A)] public uint32_t var_0A { get; set; } = var_0A;
		//[LocoStructOffset(0x0E)] public image_id BaseImageId,
		[LocoStructOffset(0x12), LocoStructSkipRead] public uint16_t TotalNumFramesType0 => (uint16_t)FrameInfoType0.Count;
		[LocoStructOffset(0x14), LocoStructSkipRead] public uint16_t TotalNumFramesType1 => (uint16_t)FrameInfoType1.Count;
		//[property: LocoStructProperty(0x16)] public const ImageAndHeight* FrameInfoType0,
		//[property: LocoStructProperty(0x1A)] public const ImageAndHeight* FrameInfoType1,
		[LocoStructOffset(0x1E)] public uint8_t NumSoundEffects { get; set; } = numSoundEffects;
		//[LocoStructOffset(0x01F), LocoArrayLength(9)] public object_index[] SoundEffects { get; set; } = soundEffects;

		public List<ImageAndHeight> FrameInfoType0 { get; set; } = [];
		public List<ImageAndHeight> FrameInfoType1 { get; set; } = [];

		public List<S5Header> SoundEffects { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// frameInfoType0
			FrameInfoType0.Clear();
			while (remainingData[0] != 0xFF)
			{
				FrameInfoType0.Add(ImageAndHeight.Read(remainingData[..ObjectAttributes.StructSize<ImageAndHeight>()]));
				remainingData = remainingData[ObjectAttributes.StructSize<ImageAndHeight>()..];
			}

			remainingData = remainingData[1..];

			// frameInfoType1
			FrameInfoType1.Clear();
			while (remainingData[0] != 0xFF)
			{
				FrameInfoType1.Add(ImageAndHeight.Read(remainingData[..ObjectAttributes.StructSize<ImageAndHeight>()]));
				remainingData = remainingData[ObjectAttributes.StructSize<ImageAndHeight>()..];
			}

			remainingData = remainingData[1..];

			// sounds effects
			SoundEffects.Clear();
			for (var i = 0; i < NumSoundEffects; ++i)
			{
				SoundEffects.Add(S5Header.Read(remainingData[..S5Header.StructLength]));
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData;
		}

		// todo: optimise this with streams - this is quite slow as is
		public ReadOnlySpan<byte> Save()
			=> FrameInfoType0.SelectMany(x => new byte[] { x.ImageOffset, x.Height })
			.Concat(new byte[] { 0xFF })
			.Concat(FrameInfoType1.SelectMany(x => new byte[] { x.ImageOffset, x.Height }))
			.Concat(new byte[] { 0xFF })
			.Concat(SoundEffects.SelectMany(sfx => sfx.Write().ToArray()))
			.ToArray();
	}
}
