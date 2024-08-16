using System.ComponentModel;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Objects
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
	public record ImageAndHeight(
		[property: LocoStructOffset(0x00)] uint8_t ImageOffset,
		[property: LocoStructOffset(0x01)] uint8_t Height
	) : ILocoStruct
	{
		public static ImageAndHeight Read(ReadOnlySpan<byte> data)
			=> new(data[0], data[1]);
		public bool Validate() => true;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	[LocoStructType(ObjectType.Steam)]
	[LocoStringTable("Name")]
	public record SteamObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoPropertyMaybeUnused] uint16_t NumImages, // this is simply the count of images in the graphics table
		[property: LocoStructOffset(0x04)] uint8_t NumStationaryTicks,
		[property: LocoStructOffset(0x05), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteWidth,
		[property: LocoStructOffset(0x06), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteHeightNegative,
		[property: LocoStructOffset(0x07), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteHeightPositive,
		[property: LocoStructOffset(0x08)] SteamObjectFlags Flags,
		[property: LocoStructOffset(0x0A)] uint32_t var_0A,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id BaseImageId,
		[property: LocoStructOffset(0x12), LocoStructVariableLoad, Browsable(false)] uint16_t _TotalNumFramesType0,
		[property: LocoStructOffset(0x14), LocoStructVariableLoad, Browsable(false)] uint16_t _TotalNumFramesType1,
		[property: LocoStructOffset(0x16)] uint32_t _FrameInfoType0Ptr,
		[property: LocoStructOffset(0x1A)] uint32_t _FrameInfoType1Ptr,
		[property: LocoStructOffset(0x1E)] uint8_t NumSoundEffects,
		[property: LocoStructOffset(0x01F), LocoArrayLength(SteamObject.MaxSoundEffects), LocoStructVariableLoad, Browsable(false)] object_id[] _SoundEffects
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int MaxSoundEffects = 9;

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
		public bool Validate() => true;
	}
}
