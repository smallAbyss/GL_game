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
        int VertexBufferObject;
        int VertexArrayObject;
        int ElementBufferObject;

        int texCoordLocation;
        Shader shader;
        Shader shader1;
        private Stopwatch timer;
        Camera camera;
        Terrain _terrain;
        FlowerField flowerField;

        int width;
        int height;


        Surface flor;
        Flower flower;
        Wall wall;

        private readonly float[] vertices =
{
       //Position            Texture coordinates
       -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
        0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
       -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
       -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

       -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
       -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
       -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

       -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
       -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
       -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
       -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
       -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
       -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

       -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
       -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
       -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

       -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
       -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
       -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,


       0f, 0f, 0f, 0.0f, 1.0f,
       10f, 0f, 0f, 1.0f, 1.0f,
       0f, 0f, 10f, 1.0f, 0.0f,
   };

        private readonly uint[] indices = {  // note that we start from 0!
       0, 1, 2,
       3, 4, 5,

       6, 7, 8,
       9, 10, 11,

       12, 13, 14,
       15, 16, 17,

       18, 19, 20,
       21, 22, 23,

       24, 25, 26,
       27, 28, 29,

       30, 31, 32,
       33, 34, 35
   };



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
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // timers
            timer = new Stopwatch();
            timer.Start();

            // deeptest
            GL.Enable(EnableCap.DepthTest);

            // camera
            camera = new Camera(width, height, new Vector3(0f, 1.5f, 0f));
            CursorState = CursorState.Grabbed;

            // objects
            wall = new Wall(shader, camera, "../../../Textures/moon.png");
            flower = new Flower(shader, camera, "../../../Textures/grass_1.jpg");
            flor = new Surface(shader, camera, "../../../Textures/cat.jpg");
            _terrain = new Terrain(100, 100, shader, camera, "../../../Textures/cat.jpg");


            // После загрузки OpenTK окна
            flowerField = new FlowerField();
            // Вместо flowerField.GenerateTestFlowers(camera, shader1);
            //flowerField.GenerateFlowers(camera, shader1, _terrain, 100, 100, 100); // 100 цветков
            //flowerField.GenerateFlowers( _terrain, 3000, 100, 100);

            flowerField = new FlowerField();
            flowerField.GenerateFlowers(_terrain, 10000,  100, 100);
            //flowerField.GenerateFlowers(camera, shader1, _terrain, 1000, 100, 100);

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
            _terrain.Draw();


            // flor.Draw();
            flowerField.Draw(shader1, camera);
            //flowerField.Draw();
            //flower.Draw();
            //wall.Draw();





            

            // swap
            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

    }
}
