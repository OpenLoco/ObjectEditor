using OpenLocoTool.Types;
using System.ComponentModel;

namespace OpenLocoToolGui
{
	public partial class StringTableUserControl : UserControl
	{
		private readonly BindingList<string> blKeys = [];

		public StringTableUserControl() => InitializeComponent();

		StringTable _data;
		StringTable Data
		{
			get => _data;
			set
			{
				_data = value;
				BindingSource.DataSource = _data;
			}
		}
		BindingSource BindingSource = new();

		public void SetDataBinding(StringTable data)
		{
			Data = data;
			//dgvLanguageSelector.DataSource = BindingSource;

			//DataContext = Data;

			blKeys.Clear();
			foreach (var key in Data.Keys)
			{
				blKeys.Add(key);
			}

			// Set up data binding for the outer dictionary DataGridView.
			lbStringSelector.DataSource = blKeys;

			// Subscribe to the SelectionChanged event to populate the inner DataGridView.
			lbStringSelector.SelectedValueChanged += (sender, e) => UpdateDGVSource();

			// update the DGV now as well
			UpdateDGVSource();

		}

		void UpdateDGVSource()
		{
			if (lbStringSelector.SelectedValue != null && Data.table.ContainsKey((string)lbStringSelector.SelectedValue))
			{
				dgvLanguageSelector.DataSource = new BindingSource(Data[(string)lbStringSelector.SelectedValue], null);
			}
		}
	}
}
