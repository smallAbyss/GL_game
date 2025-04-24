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
        private Stopwatch timer;
        Camera camera; 

        int width;
        int height;


        Surface flor;

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
            //working with VBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // working with VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // working with EBO
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // creating a shader
            shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
            shader.Use();

            // vertex attributes: position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // textures idk why here
            Texture texture = new Texture("../../../Textures/moon.png");

            // vertex attributes: texture position
            texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            // set backroundColor
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // timers
            timer = new Stopwatch();
            timer.Start();

            // deeptest
            GL.Enable(EnableCap.DepthTest);

            // camera
            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;

            flor = new Surface(shader, camera, "../../../Textures/cat.jpg");
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shader.Dispose();

            // Deleting VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

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
            
            /*
            // gen transform matrix
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45.0f));
            Matrix4 scale = Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            Matrix4 trans = rotation * scale;
            */

            double timeValue = timer.Elapsed.TotalSeconds;
            shader.SetInt("textr", 0);

            Matrix4 model = Matrix4.Identity; // to world
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            shader.Use(); /// почему перестановка этой штуки вниз ничего не ломает

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);


            /// binding VAO
            GL.BindVertexArray(VertexArrayObject);

            // drawing
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // unbinding VAO
            GL.BindVertexArray(0);


            flor.Draw();

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
