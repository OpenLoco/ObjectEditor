using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoco.Gui.ViewModels
{
	public class S5HeaderViewModel : ReactiveObject
	{
		S5Header s5Header { get; init; }

		public S5HeaderViewModel(S5Header s5)
		{
			s5Header = s5;
			Name = s5.Name;
			SourceGame = s5.SourceGame;
			ObjectType = s5.ObjectType;
			//Checksum = s5.Checksum;
			//Flags = s5.Flags;

			_ = this.WhenAnyValue(o => o.Name)
				.Subscribe(_ => s5Header.Name = Name);

			_ = this.WhenAnyValue(o => o.SourceGame)
				.Subscribe(_ =>
				{
					s5Header.SourceGame = SourceGame;
					this.RaisePropertyChanged(nameof(Flags));
				});

			_ = this.WhenAnyValue(o => o.ObjectType)
				.Subscribe(_ =>
				{
					s5Header.ObjectType = ObjectType;
					this.RaisePropertyChanged(nameof(Flags));
				});
		}

		[Reactive, MaxLength(8)]
		public string Name { get; set; }

		[Reactive]
		public SourceGame SourceGame { get; set; }

		[Reactive]
		public ObjectType ObjectType { get; set; }

		[Editable(false)]
		public uint32_t Checksum => s5Header.Checksum;

		[Editable(false)]
		public uint32_t Flags => s5Header.Flags;
	}
}
