using NLog;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKExtensions.Components;
using OpenTKExtensions.Filesystem;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using OpenTKExtensions.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HelloWorldGL
{
    public class TestBench : GameWindow
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private const string SHADERPATH = @"../../../Resources/Shaders;Resources/Shaders";

        private GameComponentCollection components = new GameComponentCollection();
        private MultiPathFileSystemPoller shaderUpdatePoller = new MultiPathFileSystemPoller(SHADERPATH.Split(';'));
        //private double lastShaderPollTime = 0.0;
        private Stopwatch timer = new Stopwatch();

        private Font font;
        private RenderTargetBase renderTarget;
        private TestComponent testcomp1;
        private TestComponent2 testcomp2;


        public class RenderData : IFrameRenderData, IFrameUpdateData
        {
            public double Time { get; set; }
            public float Param0 { get; set; }
        }
        public RenderData frameData = new RenderData();


        public TestBench() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Size = new Vector2i(800, 600),
            APIVersion = new Version(4, 6),
            API = OpenTK.Windowing.Common.ContextAPI.OpenGL,
            Profile = ContextProfile.Compatability,
            Flags = ContextFlags.Default
        })
        //base(
        //     800, 600,
        //     GraphicsMode.Default, "OpenTKExtensions TestBench",
        //     GameWindowFlags.Default, DisplayDevice.Default,
        //     4, 5, GraphicsContextFlags.ForwardCompatible
        //    )
        {
            VSync = VSyncMode.Off;

            // set static loader
            ShaderProgram.DefaultLoader = new OpenTKExtensions.Loaders.MultiPathFileSystemLoader(SHADERPATH);


            Load += TestBench_Load;
            Unload += TestBench_Unload;
            UpdateFrame += TestBench_UpdateFrame;
            RenderFrame += TestBench_RenderFrame;
            Resize += TestBench_Resize;

            components.Add(font = new Font("Resources/Fonts/consolab.ttf_sdf_512.png", "Resources/Fonts/consolab.ttf_sdf_512.txt"));

            

            components.Add(renderTarget = new RenderTargetBase(false, false, 512, 512) { DrawOrder = 1 });
            renderTarget.Loading += (s, e) =>
            {
                renderTarget.SetOutput(0, new TextureSlotParam(TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, false,
                    Texture.Params().FilterNearest().Repeat().ToArray()
                    ));
            };
            renderTarget.Add(testcomp2 = new TestComponent2());

            components.Add(testcomp1 = new TestComponent() { DrawOrder = 2 });
            testcomp1.PreRender += (s, e) => { testcomp1.tex2 = renderTarget.GetTexture(0); };
            

            //components.Add(new OperatorComponentTest());

            components.Add(new FrameCounter(font) {LoadOrder = 2, DrawOrder = 3 });

            timer.Start();
        }


        private void TestBench_Resize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            components.Resize(e.Width, e.Height);
        }

        private void TestBench_RenderFrame(FrameEventArgs e)
        {
            if (shaderUpdatePoller.Poll())
            {
                components.Reload();
                shaderUpdatePoller.Reset();
            }

            GL.ClearColor(0.0f, 0.1f, 0.2f, 1.0f);
            GL.ClearDepth(1.0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.Disable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);


            components.Render(frameData);

            SwapBuffers();
            Thread.Sleep(0);

        }

        private void TestBench_UpdateFrame(FrameEventArgs e)
        {
            frameData.Param0 = (KeyboardState[Keys.Space]) ? 1f : 0f;
            frameData.Time = timer.Elapsed.TotalSeconds;
            components.Update(frameData);
        }

        private void TestBench_Unload()
        {
            components.Unload();
        }

        private void TestBench_Load()
        {
            components.Load();
            timer.Start();
        }


        protected void LogTrace(string message, [CallerMemberName] string caller = null)
        {
            log.Trace($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            log.Info($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogWarn(string message, [CallerMemberName] string caller = null)
        {
            log.Warn($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogError(string message, [CallerMemberName] string caller = null)
        {
            log.Error($"{this.GetType().Name}.{caller}: {message}");
        }

    }
}
