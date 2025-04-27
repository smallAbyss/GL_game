using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace OpenGL_2
{
    /*
    internal class FlowerField
    {
        private List<FlowerInstance> flowers = new List<FlowerInstance>();

        public void GenerateTestFlowers(Camera camera, Shader shader)
        {
            // Один тестовый цветок по центру
            FlowerInstance fl = new FlowerInstance
            {
                Position = new Vector3(2f, -5f, 2f),
                Scale = new Vector3(10f, 10f, 10f),
                Rotation = 80.0f,
                Camera = camera,
                Shader = shader
            };
            flowers.Add(fl);
        }

        public void GenerateFlowers(Camera camera, Shader shader, Terrain terrain, int count, float terrainWidth, float terrainLength)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                float x = (float)(random.NextDouble() * terrainWidth);
                float z = (float)(random.NextDouble() * terrainLength);

                // Можем у террейна узнать высоту в этой точке, если есть
                float y = terrain.GetTerrainHeight(x, z); // пока что пусть просто 0, потом можно заменить на 

                flowers.Add(new FlowerInstance
                {
                    Position = new Vector3(x, y, z),
                    Scale = new Vector3(0.5f, 0.5f, 0.5f), // чуть уменьшенные
                    Rotation = (float)(random.NextDouble() * 360f),
                    Camera = camera,
                    Shader = shader
                });
            }
        }

        public void Draw()
        {
            foreach (var flower in flowers)
            {
                flower.Draw();
            }
        }
    }

    internal class FlowerInstance
    {
        public Vector3 Position;
        public Vector3 Scale;
        public float Rotation;
        public Camera Camera;
        public Shader Shader;

        private   readonly float[] vertices = {
            // Плоский квадрат (два треугольника)
            -0.5f, 0f, -0.5f,
             0.5f, 0f, -0.5f,
             0.5f, 0f,  0.5f,
            -0.5f, 0f,  0.5f
        };

        private readonly float[] vertices2 = {
            // Плоский квадрат (два треугольника)
            0f, 0f, 0f,
             0.5f, 0f, 0f,
             0f, 0f,  0.5f,
            0.5f, 0f,  0.5f
        };

        private   readonly uint[] indices = {
            0, 1, 2,
            2, 3, 0
        };

        private   int vao;
        private   int vbo;
        private   int ebo;
        private   bool initialized = false;

        private   void Initialize()
        {
            if (initialized) return;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribDivisor(1, 1);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribDivisor(2, 1);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 6 * sizeof(float));
            GL.VertexAttribDivisor(3, 1);


            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL error: " + error);
            }


            initialized = true;
        }


        public void Draw()
        {
            Initialize();

            Shader.Use();



            Matrix4 model = Matrix4.Identity;
            // Правильный порядок преобразований:
            model *= Matrix4.CreateScale(Scale); // И наконец масштабируем
            //model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation)); // Затем поворачиваем
            model *= Matrix4.CreateTranslation(Position); // Сначала перемещаем

            Shader.SetMatrix4("model", model);
            Shader.SetMatrix4("view", Camera.GetViewMatrix());
            Shader.SetMatrix4("projection", Camera.GetProjection());

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        }
    }
    */
}

