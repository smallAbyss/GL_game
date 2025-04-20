using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_2
{
    internal class Game :GameWindow
    {
        int VertexBufferObject;

        Shader shader;

        float[] vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
             0.5f, -0.5f, 0.0f, //Bottom-right vertex
             0.0f,  0.5f, 0.0f  //Top vertex
        };

        public Game(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { } // Vs жаловался на просто Size

        int VertexArrayObject;
        protected override void OnLoad()
        {
            base.OnLoad();


            //working with VBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // creating & working with VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

           
            // creating a shader
            shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
            shader.Use();

            // set backroundColor
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

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
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here.
            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

    }
}
