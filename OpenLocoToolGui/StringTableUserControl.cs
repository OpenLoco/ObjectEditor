using OpenLocoTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace OpenLocoToolGui
{
	public partial class StringTableUserControl : UserControl
	{
		private BindingList<string> blKeys = new BindingList<string>();
		//Dictionary<string, Dictionary<LanguageId, string>> boundData;

		public StringTableUserControl()
		{
			InitializeComponent();

			// Set up data binding for the inner dictionary DataGridView.
			//dgvLanguageSelector.DataSource = innerDataBindingList;
		}

		public void SetDataBinding(Dictionary<string, Dictionary<LanguageId, string>> data)
		{
			//boundData = data;

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

	//public class MyPropertyTypeConverter : TypeConverter
	//{
	//	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	//	{
	//		return true;
	//	}

	//	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	//	{
	//		// Return the PropertyDescriptors for the properties of your custom UserControl.
	//		// You can create custom PropertyDescriptors if needed.
	//		return TypeDescriptor.GetProperties(value, attributes);
	//	}
	//}


	//public class MyPropertyUIEditor : UITypeEditor
	//{
	//	public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
	//	{
	//		return UITypeEditorEditStyle.Modal;
	//	}

	//	public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
	//	{
	//		if (provider != null)
	//		{
	//			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

	//			if (editorService != null)
	//			{
	//				// Create an instance of your custom UserControl.
	//				StringTableUserControl userControl = new StringTableUserControl();
	//				// Set its value based on the current property value.
	//				userControl.PropertyValue = (YourPropertyType)value;

	//				// Show your UserControl in a dialog.
	//				editorService.ShowDialog(userControl);

	//				// Return the updated value from your UserControl.
	//				return userControl.PropertyValue;
	//			}
	//		}

	//		return base.EditValue(context, provider, value);
	//	}
	//}

}
