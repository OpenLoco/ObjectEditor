using ReactiveUI;
using System;
using OpenLoco.ObjectEditor.Types;
using OpenLoco.ObjectEditor.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AvaGui.ViewModels
{
	public class StringTableViewModel : ViewModelBase
	{
		public StringTableViewModel(StringTable table)
		{
			_table = table;

			_ = this.WhenAnyValue(o => o.Table)
				.Subscribe(o => this.RaisePropertyChanged(nameof(Keys)));
			_ = this.WhenAnyValue(o => o.Keys)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentlySelectedKey)));
		}

		public StringTable _table;
		public StringTable Table
		{
			get => _table;
			set
			{
				if (value != null)
				{
					_keys = new BindingList<string>(value.Table.Keys.ToList());
					_ = this.RaiseAndSetIfChanged(ref _table, value);
				}
			}
		}

		public BindingList<string> _keys;
		public BindingList<string> Keys
		{
			get => _keys;
			set => this.RaiseAndSetIfChanged(ref _keys, value);
		}

		public string _currentlySelectedKey;
		public string CurrentlySelectedKey
		{
			get => _currentlySelectedKey;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedKey, value);
		}
	}

	public class DesignStringTableViewModel : StringTableViewModel
	{
		public DesignStringTableViewModel() : base(designStringTableData)
		{ }

		static readonly Dictionary<string, Dictionary<LanguageId, string>> designTableData = new()
		{
			{ "Name",
				new Dictionary<LanguageId, string>()
				{
					{ LanguageId.English_UK, "Engine" },
					{ LanguageId.English_US, "Engine" },
					{ LanguageId.Dutch, "Motor" },
					{ LanguageId.French, "Moteur" },
				}
			},
			{ "Type",
				new Dictionary<LanguageId, string>()
				{
					{ LanguageId.English_UK, "Car" },
					{ LanguageId.English_US, "Automobile" },
					{ LanguageId.Dutch, "Auto" },
					{ LanguageId.French, "Voiture" },
				}
			},
		};

		static StringTable designStringTableData = new(designTableData);
	}
}
