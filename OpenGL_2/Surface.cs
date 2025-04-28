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

    

public struct Vertex
    {
        public Vector3 Position;
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
        float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        public Terrain(int width, int length, Shader sh, Camera cam, string texture_path)
        {
            texture = new Texture(texture_path);
            shader = sh;
            camera = cam;

            _width = width;
            _length = length;
            _heights = new float[width, length];

            GenerateHills(); // Генерация высот
            SetupMesh();     // Создание буферов
        }

        public Vector3 GetTerrainNormal(float x, float z)
        {
            // Простейший способ - через соседние точки
            float offset = 0.1f;
            float y1 = GetTerrainHeight(x - offset, z);
            float y2 = GetTerrainHeight(x + offset, z);
            float y3 = GetTerrainHeight(x, z - offset);
            float y4 = GetTerrainHeight(x, z + offset);

            Vector3 tangent = new Vector3(2 * offset, y2 - y1, 0);
            Vector3 bitangent = new Vector3(0, y4 - y3, 2 * offset);

            return Vector3.Cross(tangent, bitangent).Normalized();
        }

        private void GenerateHills()
        {
            float heightScale = 10.0f; // Множитель высоты 

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                {
                   
                    // Нормализованные координаты для плавности
                    float nx = x / (float)_width * 4f; // Увеличиваем частоту
                    float nz = z / (float)_length * 4f;

                    float noise = (float)(
                       Math.Sin(nx * 1.5f) * Math.Cos(nz * 1.5f) * 0.8f + 
                        Math.Sin(nx * 0.7f) * Math.Cos(nz * 0.7f) * 1.2f   
                        
                    );

                    _heights[x, z] = noise * heightScale * 1;
                }
            }
        }

        private void SetupMesh()
        {
            // Создаём вершины
            Vertex[] vertices = new Vertex[_width * _length]; /// а как двойные индексы в indices для EBO передавать? -  а никак
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                {
                    vertices[x * _length + z] = new Vertex
                    {
                        Position = new Vector3(x, _heights[x, z], z),
                    };
                }
            }

            Vector2[] texture_vertices = new Vector2[_width * _length]; 
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _length; z++)
                { 
                int texIdx = (x * _length + z);
                texture_vertices[x * _length + z] = new Vector2(x / (float)(_width - 1) * TEXTURE_COUNT, z / (float)(_length - 1) * TEXTURE_COUNT);
                }
            }
            ////// ***

            // Создаём индексы для треугольников
            List<uint> indices = new List<uint>();
            for (int x = 0; x < _width - 1; x++)
            {
                for (int z = 0; z < _length - 1; z++)
                { 
                    uint topLeft = (uint)(x * _length + z);
                    uint topRight = (uint)(x * _length + z + 1);
                    uint bottomLeft = (uint)((x + 1) * _length + z);
                    uint bottomRight = (uint)((x + 1) * _length + z + 1);

                    // Первый треугольник (topLeft - bottomLeft - topRight)
                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);

                    // Второй треугольник (topRight - bottomLeft - bottomRight)
                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }
            _indexCount = indices.Count;

            // Создаём VAO, VBO и EBO
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


            //  Указываем атрибуты вершин (позиция)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            // Указываем атрибуты вершин (текстура)
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

        /// ** sin
        public float GetTerrainHeight(float x, float z)
        {
            // Переводим мировые координаты в координаты сетки
            float gridX = x;
            float gridZ = z;

            int x0 = (int)MathF.Floor(gridX);
            int z0 = (int)MathF.Floor(gridZ);
            int x1 = x0 + 1;
            int z1 = z0 + 1;

            // Краевые случаи: за пределами карты
            x0 = Math.Clamp(x0, 0, _width - 1);
            x1 = Math.Clamp(x1, 0, _width - 1);
            z0 = Math.Clamp(z0, 0, _length - 1);
            z1 = Math.Clamp(z1, 0, _length - 1);

            // Дробные части (насколько мы смещены внутри квадратика)
            float tx = gridX - x0;
            float tz = gridZ - z0;

            // Высоты четырёх углов квадрата
            float h00 = _heights[x0, z0]; // top-left
            float h10 = _heights[x1, z0]; // top-right
            float h01 = _heights[x0, z1]; // bottom-left
            float h11 = _heights[x1, z1]; // bottom-right

            // Билинейная интерполяция
            float height = Lerp(
                Lerp(h00, h10, tx),
                Lerp(h01, h11, tx),
                tz
            );

            return height;
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
