using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace OpenGL_2
{
    internal class FlowerField
    {
        private List<FlowerInstance> flowers = new List<FlowerInstance>();

        private int vao;
        private int vbo;
        private int ebo;
        private int instanceVbo;

        private readonly float[] vertices = {
            -0.5f, 0f, -0.5f,
             0.5f, 0f, -0.5f,
             0.5f, 0f,  0.5f,
            -0.5f, 0f,  0.5f
        };

        private readonly uint[] indices = {
            0, 1, 2,
            2, 3, 0
        };

        private bool initialized = false;

        
        public void GenerateFlowers(Terrain terrain, int count, float terrainWidth, float terrainLength)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {

                float x = (float)(random.NextDouble() * terrainWidth);
                float z = (float)(random.NextDouble() * terrainLength);
                float y = terrain.GetTerrainHeight(x, z);
                
                float rotation = (float)(random.NextDouble() * 360f);
                float scale = 0.5f;
               // Console.WriteLine($"Flower at X: {x}, Y: {y}, Z: {z}");

                Matrix4 model = Matrix4.CreateScale(scale) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation)) * Matrix4.CreateTranslation(new Vector3(x, y, z));

                flowers.Add(new FlowerInstance
                {
                    ModelMatrix = model,
                    //Color = new Vector3(1.0f, 0.8f, 0.2f)
                    Color = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble())
                });
            }
            Initialize();
        }

        private void Initialize()
        {
            if (initialized) return;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            instanceVbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //UpdateInstanceBuffer();


            GL.BindVertexArray(vao);
            List<float> instanceData = new List<float>();

            foreach (var flower in flowers)
            {
                // Матрица model
                instanceData.Add(flower.ModelMatrix.M11); instanceData.Add(flower.ModelMatrix.M21); instanceData.Add(flower.ModelMatrix.M31); instanceData.Add(flower.ModelMatrix.M41);
                instanceData.Add(flower.ModelMatrix.M12); instanceData.Add(flower.ModelMatrix.M22); instanceData.Add(flower.ModelMatrix.M32); instanceData.Add(flower.ModelMatrix.M42);
                instanceData.Add(flower.ModelMatrix.M13); instanceData.Add(flower.ModelMatrix.M23); instanceData.Add(flower.ModelMatrix.M33); instanceData.Add(flower.ModelMatrix.M43);
                instanceData.Add(flower.ModelMatrix.M14); instanceData.Add(flower.ModelMatrix.M14); instanceData.Add(flower.ModelMatrix.M34); instanceData.Add(flower.ModelMatrix.M44);

                /*
                Console.WriteLine("///////////////////////////////");
                Console.WriteLine($"{flower.ModelMatrix.M11}\t{flower.ModelMatrix.M12}\t{flower.ModelMatrix.M13}\t{flower.ModelMatrix.M14}");
                Console.WriteLine($"{flower.ModelMatrix.M21}\t{flower.ModelMatrix.M22}\t{flower.ModelMatrix.M23}\t{flower.ModelMatrix.M24}");
                Console.WriteLine($"{flower.ModelMatrix.M31}\t{flower.ModelMatrix.M32}\t{flower.ModelMatrix.M33}\t{flower.ModelMatrix.M34}");
                Console.WriteLine($"{flower.ModelMatrix.M41}\t{flower.ModelMatrix.M42}\t{flower.ModelMatrix.M43}\t{flower.ModelMatrix.M44}");
                Console.WriteLine("///////////////////////////////");
                */



                instanceData.Add(flower.Color.X);
                instanceData.Add(flower.Color.Y);
                instanceData.Add(flower.Color.Z);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, instanceData.Count * sizeof(float), instanceData.ToArray(), BufferUsageHint.DynamicDraw);


            int stride = (16 + 3) * sizeof(float);
            // Матрица instanceModel
            for (int i = 0; i < 4; i++)
            {
                GL.EnableVertexAttribArray(1 + i);
                GL.VertexAttribPointer(1 + i, 4, VertexAttribPointerType.Float, false, stride, i * 4 * sizeof(float));
                GL.VertexAttribDivisor(1 + i, 1);
            }

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, 4 * 4 * sizeof(float));
            GL.VertexAttribDivisor(5, 1);


            initialized = true;
        }

        

        public void Draw(Shader shader, Camera camera)
        {

            shader.Use();
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjection());

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, flowers.Count);
        }
    }

    internal class FlowerInstance
    {
        public Matrix4 ModelMatrix;
        public Vector3 Color;
    }
}
