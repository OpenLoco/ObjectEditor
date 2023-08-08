// See https://aka.ms/new-console-template for more information

using OpenLocoTool;
using OpenLocoToolCommon;

var logger = new Logger();
logger.Level = LogLevel.Debug2;
logger.LogAdded += (s, e) => Console.WriteLine(e.Log);

logger.Log(LogLevel.Info, "=== Welcome to OpenLocoTool ===");

const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";

//var decoder = new DatDecoder(logger);
//decoder.Decode(path);

//var ssr = new SawyerStreamReader(logger);
//var obj = ssr.Load(path);

Console.ReadLine();