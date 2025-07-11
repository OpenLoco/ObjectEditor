using Common.Logging;
using System;
using System.Collections.ObjectModel;

namespace Gui.ViewModels
{
	public class DesignLogWindowViewModel : LogWindowViewModel
	{
		public DesignLogWindowViewModel() : base([new LogLine(DateTime.Now, LogLevel.Info, "Caller", "Message")])
		{ }
	}

	public class LogWindowViewModel : ViewModelBase
	{
		public ObservableCollection<LogLine> Logs { get; init; }

		public LogWindowViewModel(ObservableCollection<LogLine> logs)
			=> Logs = logs;

	}
}
