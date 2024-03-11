using Veldrid.StartupUtilities;
using Veldrid;
using System.Diagnostics;
using Veldrid.Sdl2;
using Veldrid.ImageSharp;

namespace VelgridGui
{
	internal class Program
	{
		static void Main(string[] args)
		{
			new Demo().Run();
		}
	}

	class Demo
	{
		private Sdl2Window _window;
		private GraphicsDevice _gd;
		//private Scene _scene;
		private readonly ImGuiRenderable _igRenderable;
		//private readonly SceneContext _sc = new SceneContext();
		private bool _windowResized;
		//private RenderOrderKeyComparer _renderOrderKeyComparer = new RenderOrderKeyComparer();
		private bool _recreateWindow = true;

		private static double _desiredFrameLengthSeconds = 1.0 / 60.0;
		private static bool _limitFrameRate = false;
		//private static FrameTimeAverager _fta = new FrameTimeAverager(0.666);
		private CommandList _frameCommands;

		private event Action<int, int> _resizeHandled;

		private readonly Dictionary<string, ImageSharpTexture> _textures = new Dictionary<string, ImageSharpTexture>();
		//private Sdl2ControllerTracker _controllerTracker;
		private bool _colorSrgb = true;
		//private FullScreenQuad _fsq;
		//public static RenderDoc _renderDoc;
		private bool _controllerDebugMenu;
		private bool _showImguiDemo;

		public Demo()
		{
			WindowCreateInfo windowCI = new WindowCreateInfo
			{
				X = 50,
				Y = 50,
				WindowWidth = 960,
				WindowHeight = 540,
				WindowInitialState = WindowState.Normal,
				WindowTitle = "Veldrid NeoDemo"
			};
			GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved, true, true, _colorSrgb);
			gdOptions.Debug = true;
		}

		public void Run()
		{
			long previousFrameTicks = 0;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			while (_window.Exists)
			{
				long currentFrameTicks = sw.ElapsedTicks;
				double deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

				while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds)
				{
					currentFrameTicks = sw.ElapsedTicks;
					deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
				}

				previousFrameTicks = currentFrameTicks;

				InputSnapshot snapshot = null;
				Sdl2Events.ProcessEvents();
				snapshot = _window.PumpEvents();
				//InputTracker.UpdateFrameInput(snapshot, _window);
				//Update((float)deltaSeconds);
				if (!_window.Exists)
				{
					break;
				}

				//Draw();
			}

			DestroyAllObjects();
			_gd.Dispose();
		}

		private void DestroyAllObjects()
		{
			_gd.WaitForIdle();
			_frameCommands.Dispose();
			//_sc.DestroyDeviceObjects();
			//_scene.DestroyAllDeviceObjects();
			//CommonMaterials.DestroyAllDeviceObjects();
			//StaticResourceCache.DestroyAllDeviceObjects();
			_gd.WaitForIdle();
		}
	}
}
