using System.Text;

namespace DatabaseTools.Services;

// TextWriter that calls a callback for each logical line written via Console.Write/WriteLine.
// Used to redirect stdout from helper scripts into the UI log pane while a script is running.
public sealed class CallbackTextWriter : TextWriter
{
	readonly Action<string> _onLine;
	readonly StringBuilder _buffer = new();

	public CallbackTextWriter(Action<string> onLine)
	{
		_onLine = onLine;
	}

	public override Encoding Encoding => Encoding.UTF8;

	public override void Write(char value)
	{
		if (value == '\n')
		{
			Flush();
			return;
		}

		if (value == '\r')
		{
			return;
		}

		_ = _buffer.Append(value);
	}

	public override void Write(string? value)
	{
		if (value is null)
		{
			return;
		}

		foreach (var c in value)
		{
			Write(c);
		}
	}

	public override void WriteLine()
		=> Flush();

	public override void WriteLine(string? value)
	{
		Write(value);
		Flush();
	}

	public override void Flush()
	{
		if (_buffer.Length == 0)
		{
			return;
		}

		var line = _buffer.ToString();
		_buffer.Clear();
		_onLine(line);
	}
}
