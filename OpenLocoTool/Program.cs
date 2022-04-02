// See https://aka.ms/new-console-template for more information

using OpenLocoTool;
using OpenLocoToolCommon;

var logger = new Logger();
logger.Level = LogLevel.Debug2;
logger.LogAdded += (s, e) => Console.WriteLine(e.Log);

logger.Log(LogLevel.Info, "=== Welcome to OpenLocoTool ===");

var decoder = new DatDecoder(logger);
decoder.Decode("Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\P61XX.dat");

Console.ReadLine();