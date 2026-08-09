using DatabaseTools;
using DatabaseTools.Services;

Console.WriteLine("Hello, World!");

var ts = new ToolsSettings();
await DatabaseHelperScripts.ExtractStringTableAsync(ts, (s) => Console.WriteLine(s));
