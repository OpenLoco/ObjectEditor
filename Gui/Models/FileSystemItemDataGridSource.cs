using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace Gui.Models;

// Centralised column definitions for the FileSystemItem tree.
// HierarchicalTreeDataGridSource<T> is sealed in TreeDataGrid v12, so we cannot
// subclass it; this helper builds a source with the standard columns instead.
public static class FileSystemItemDataGridSource
{
	public static HierarchicalTreeDataGridSource<FileSystemItem> Create(
		IEnumerable<FileSystemItem> items,
		IDataTemplate? cellTemplate,
		IDataTemplate? cellEditingTemplate)
		=> new(items ?? [])
		{
			Columns =
			{
				new TreeDataGridHierarchicalExpanderColumn
				{
					Inner = new TreeDataGridTemplateColumn
					{
						Header = string.Empty,
						CellTemplate = cellTemplate,
						CellEditingTemplate = cellEditingTemplate,
						Width = new GridLength(1, GridUnitType.Auto),
					},
					ChildrenBinding = new Binding(nameof(FileSystemItem.SubNodes)),
				},
				new TreeDataGridTextColumn { Header = "Source", Binding = new Binding(nameof(FileSystemItem.NiceObjectSource)) },
				new TreeDataGridTextColumn { Header = "Origin", Binding = new Binding(nameof(FileSystemItem.FileLocation)) },
				new TreeDataGridTextColumn { Header = "Created", Binding = new Binding(nameof(FileSystemItem.CreatedDate)) },
				new TreeDataGridTextColumn { Header = "Modified", Binding = new Binding(nameof(FileSystemItem.ModifiedDate)) },
				new TreeDataGridTextColumn { Header = "Type", Binding = new Binding(nameof(FileSystemItem.ObjectType)) },
			},
		};
}
