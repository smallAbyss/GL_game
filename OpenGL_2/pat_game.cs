using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_2
{
    internal class Game_pat : GameWindow
    {
        Shader shader;
        Shader shader1;
        private Stopwatch timer;
        Camera camera;
        Terrain _terrain;
        FlowerField flowerField;


        int width;
        int height;


        Flower flower;

     


        public Game_pat(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {
            this.width = width;
            this.height = height;
        } // Vs жаловался на просто Size

        protected override void OnLoad()
        {
            base.OnLoad();
            shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
            shader1 = new Shader("../../../Shaders/fl.vert", "../../../Shaders/fl.frag");

            // set backroundColor
            GL.ClearColor(66.0f / 255.0f, 170.0f / 255.0f, 255.0f / 255.0f, 1.0f);

            // timers
            timer = new Stopwatch();
            timer.Start();

            // deeptest
            GL.Enable(EnableCap.DepthTest);

            // camera
            camera = new Camera(width, height, new Vector3(0f, 1.5f, 0f));
            CursorState = CursorState.Grabbed;

            // objects
            flower = new Flower(shader, camera, "../../../Textures/box.jpg");
            _terrain = new Terrain(100, 100, shader, camera, "../../../Textures/grass_1.jpg");



            flowerField = new FlowerField();
            flowerField.GenerateFlowers(_terrain, 500000,  100, 100, "../../../Textures/fle.png");

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            base.OnUpdateFrame(args);
            camera.Update(input, mouse, args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            flower.Draw();
            _terrain.Draw();
            flowerField.Draw(shader1, camera);
            

            // swap
            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
            camera.UpdateDim(this.width, this.height);
        }

    }
}
