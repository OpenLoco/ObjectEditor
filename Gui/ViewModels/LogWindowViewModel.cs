using Common.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;

namespace Gui.ViewModels;

public class DesignLogWindowViewModel : LogWindowViewModel
{
	public DesignLogWindowViewModel() : base([new LogLine(DateTime.Now, LogLevel.Information, "Caller", "Message")])
	{ }
}

public class LogWindowViewModel(ObservableCollection<LogLine> logs) : ViewModelBase
{
	public ObservableCollection<LogLine> Logs { get; init; } = logs;
}
