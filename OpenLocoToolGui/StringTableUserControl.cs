using OpenLocoTool;
using System.ComponentModel;

namespace OpenLocoToolGui
{
	public partial class StringTableUserControl : UserControl
	{
		private readonly BindingList<string> blKeys = [];

		public StringTableUserControl() => InitializeComponent();

		public void SetDataBinding(Dictionary<string, Dictionary<LanguageId, string>> data)
		{
			blKeys.Clear();
			foreach (var key in data.Keys)
			{
				blKeys.Add(key);
			}

			// Set up data binding for the outer dictionary DataGridView.
			lbStringSelector.DataSource = blKeys;

			// Subscribe to the SelectionChanged event to populate the inner DataGridView.
			lbStringSelector.SelectedValueChanged += (sender, e) =>
			{
				if (lbStringSelector.SelectedValue != null)
				{
					dgvLanguageSelector.DataSource = new BindingSource(data[(string)lbStringSelector.SelectedValue], null);
				}
			};
		}
	}
}
