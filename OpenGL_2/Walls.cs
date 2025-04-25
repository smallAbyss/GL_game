using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace OpenGL_2
{
    internal class Wall
    {
        int VBO, VAO, EBO;

        int texCoordLocation;

        Shader shader;
        Camera camera;
        Texture texture;

        private float[] vertices =
        {
            //Position      Texture coordinates
             -20f, 0f,  -20f,  0.0f, 0.0f, // 0
             -20f, 10f, -20f,  0.0f, 0.0f, // 1

             -20f, 0f,   20f,  1.0f, 0.0f, // 2
             -20f, 10f,  20f,  1.0f, 0.0f, // 3


              20f, 0f,  -20f,  0.0f, 1.0f, // 4
              20f, 10f, -20f,  0.0f, 1.0f, // 5

              20f, 0f,   20f,  1.0f, 1.0f, // 6
              20f, 10f,  20f,  1.0f, 1.0f, // 7

        };

        private readonly uint[] indices =
        {
            0, 1, 2,
            1, 2, 3
        };

        public Wall(Shader sh, Camera cam, string texture_path)
        {

            for (int i = 1; i < vertices.Length; i += 5)
            {
                vertices[i] += 0.25f;
            }
            shader = sh;
            camera = cam;

            shader.Use();

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // vertex attributes: position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // vertex attributes: texture position
            texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            // unbinding
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //VBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // EBO
            GL.BindVertexArray(0); // VAO

            // textures 
            //texture = new Texture("../../../Textures/cat.jpg");
            texture = new Texture(texture_path);
        }

        public void Draw()
        {
            shader.Use();
            shader.SetInt("textr", texture.GetActiveTextureSocketNumber() - 0);


            // matrixes
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);


            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


            //shader.Dispose();
        }



        ~Wall() /// а он и не вызывается блин
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(VAO);

            shader.Dispose();
        }

    }
}
