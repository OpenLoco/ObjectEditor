using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenLoco.Gui.ViewModels
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class S5HeaderViewModel : ReactiveObject
	{
		// todo: instead of setting ObjectType to a default here, instead prompt user to select a type if appropriate
		public S5HeaderViewModel()
			: this(string.Empty, 0, SourceGame.Custom, ObjectType.InterfaceSkin)
		{ }

		public S5HeaderViewModel(string name, uint checksum, SourceGame sourceGame, ObjectType objectType)
		{
			Name = name;
			Checksum = checksum;
			SourceGame = sourceGame;
			ObjectType = objectType;
		}

		public S5HeaderViewModel(S5Header s5Header)
		{
			Name = s5Header.Name;
			Checksum = s5Header.Checksum;
			SourceGame = s5Header.SourceGame;
			ObjectType = s5Header.ObjectType;

			_ = this.WhenAnyValue(o => o.Checksum)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(ChecksumHex)));
		}

		[Reactive, MaxLength(8)]
		public string Name { get; set; }

		[Reactive]
		public uint32_t Checksum { get; set; }

		public string ChecksumHex => string.Format($"{Checksum:X}");

		[Reactive]
		public SourceGame SourceGame { get; set; }

		[Reactive]
		public ObjectType ObjectType { get; set; }

		public S5Header GetAsUnderlyingType()
			=> new(Name, Checksum)
			{
				ObjectType = ObjectType,
				SourceGame = SourceGame
			};
	}
}
