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
    internal class Surface
    {
        int VBO, VAO, EBO;

        int texCoordLocation;

        Shader shader;
        Camera camera;
        Texture texture;

        private float[] vertices =
        {
            //Position      Texture coordinates
             -20f, 0f, -20f,  0.0f, 0.0f,
             -20f, 0f,  20f,  1.0f, 0.0f,
              20f, 0f, -20f,  0.0f, 1.0f,
              20f, 0f,  20f,  1.0f, 1.0f,

        };

        private readonly uint[] indices = 
        { 
            0, 1, 2,
            1, 2, 3
        };

        public Surface(Shader sh, Camera cam, string texture_path) 
        {
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
            shader.SetInt("textr", texture.GetActiveTextureSocketNumber() - 1);


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



        ~Surface() /// а он и не вызывается блин
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

public struct Vertex
    {
        public Vector3 Position;
        // Можно добавить нормали и текстурные координаты:
        // public Vector3 Normal;
        // public Vector2 TexCoord;
    }

    public class Terrain
    {
        private int _vao; // Vertex Array Object
        private int _vbo; // Vertex Buffer Object
        private int _ebo; // Element Buffer Object
        private int _indexCount;
        int _texture_vbo;

        private float[,] _heights;
        private int _width, _length;

        private Camera camera;
        private Shader shader;
        private Texture texture;

        private const int TEXTURE_COUNT = 25;

        public Terrain(int width, int length, Shader sh, Camera cam, string texture_path)
        {
            texture = new Texture(texture_path);
            shader = sh;
            camera = cam;

            _width = width;
            _length = length;
            _heights = new float[width, length];

            GenerateHills(); // Генерация высот
            SetupMesh();     // Создание меша и буферов
        }

        private void GenerateHills()
        {
            float heightScale = 10.0f; // Множитель высоты 

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                {
                    //// ???
                    // Нормализованные координаты для плавности
                    float nx = x / (float)_width * 4f; // Увеличиваем частоту
                    float nz = z / (float)_length * 4f;

                    // Комбинируем несколько слоёв шума для реалистичности
                    float noise = (float)(
                       Math.Sin(nx * 1.5f) * Math.Cos(nz * 1.5f) * 0.8f + // Основные холмы
                       // Math.Sin(nx * 3.0f) * Math.Cos(nz * 3.0f) * 0.3f +  // Детализация
                        Math.Sin(nx * 0.7f) * Math.Cos(nz * 0.7f) * 1.2f   // Крупные формы
                        
                    );

                    _heights[x, z] = noise * heightScale;
                }
            }
        }

        private void SetupMesh()
        {
            // 1. Создаём вершины
            Vertex[] vertices = new Vertex[_width * _length]; /// а как двойные индексы в indices для EBO передавать?
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                {
                    vertices[x * _length + z] = new Vertex
                    {
                        Position = new Vector3(x, _heights[x, z], z),
                        // Normal = Vector3.UnitY, // Можно добавить нормали
                    };
                }
            }

            Vector2[] texture_vertices = new Vector2[_width * _length]; /// а как двойные индексы в indices для EBO передавать?
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                { 
                int texIdx = (x * _length + z);
                texture_vertices[x * _length + z] = new Vector2(x / (float)(_width - 1) * TEXTURE_COUNT, z / (float)(_length - 1) * TEXTURE_COUNT);
                }
            }

            // 2. Создаём индексы для треугольников
            List<uint> indices = new List<uint>();
            for (int x = 0; x < _width - 1; x++)
            {
                for (int z = 0; z < _length - 1; z++)
                { 
                    uint topLeft = (uint)(x * _length + z);
                    uint topRight = (uint)(x * _length + z + 1);
                    uint bottomLeft = (uint)((x + 1) * _length + z);
                    uint bottomRight = (uint)((x + 1) * _length + z + 1);

                    // Первый треугольник (topLeft → bottomLeft → topRight)
                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);

                    // Второй треугольник (topRight → bottomLeft → bottomRight)
                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }
            _indexCount = indices.Count;

            // 3. Создаём VAO, VBO и EBO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);

            _texture_vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _texture_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, texture_vertices.Length * Vector2.SizeInBytes, texture_vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexCount * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);


            // 4. Указываем атрибуты вершин (позиция)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            // 5. Указываем атрибуты вершин (текстура)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _texture_vbo);
            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(texCoordLocation);

            // Отвязываем буферы
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); 
            GL.BindVertexArray(0);
        }

        public void Draw()
        {

            shader.Use();
            shader.SetInt("textr", 1);


            // matrixes
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);



            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
