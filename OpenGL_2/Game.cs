using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_2
{
    internal class Game :GameWindow
    {
        int VertexBufferObject;
        int VertexArrayObject;
        int ElementBufferObject;

        int texCoordLocation;
        Shader shader;
        private Stopwatch timer;

        int width;
        int height;

        private readonly float[] vertices_color =
        {
            // positions         // colors
             0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // bottom right
            -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // bottom left
             0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // top 
        };

        private readonly float[] vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, -1.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, -1.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, -1.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] indices = {  // note that we start from 0!
            0, 1, 2   /// мне было лень убирать индексы, а ещё интересная штука: если здесь указать элементы > vertices.size, то оно не ломается
                       /// ну точнее как.. проверь в общем
        };


        public Game(int width, int height, string title) :  base(GameWindowSettings.Default,
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

            // creating & working with VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

           // GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
           // GL.EnableVertexAttribArray(1);

            // textures idk why here
            Texture texture = new Texture("C:/Users/labyss/Downloads/vozdushnyj_shar_aerostat_art_128614_1920x1080.jpg"); /// AND THIS FIX
            texCoordLocation = 1;//shader.GetAttribLocation("aTexCoord"); /// FIX THIS 
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            // working with EBO
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // creating a shader
            shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
            shader.Use();

            // set backroundColor
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // timers
            timer = new Stopwatch();
            timer.Start();

        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shader.Dispose();
            
            // Deleting VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteTexture(texCoordLocation);

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

            /*
            // gen transform matrix
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45.0f));
            Matrix4 scale = Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            Matrix4 trans = rotation * scale;
            */

            double timeValue = timer.Elapsed.TotalSeconds;


            Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians((float)(15.0f*timeValue*20))); // to world
            Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f); // moving scene
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);


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
