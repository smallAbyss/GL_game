using System.Collections.Generic;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Model
{
    private List<Mesh> _meshes = new List<Mesh>();

    public void LoadModel(string path)
    {
        var importer = new AssimpContext();
        var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

        foreach (var mesh in scene.Meshes)
        {
            var vertices = new List<Vector3>();
            var indices = new List<uint>();

            // Загрузка вершин
            foreach (var vertex in mesh.Vertices)
            {
                vertices.Add(new Vector3(vertex.X, vertex.Y, vertex.Z));
            }

            // Загрузка индексов
            foreach (var face in mesh.Faces)
            {
                //indices.AddRange(face.Indices);
                indices.AddRange(face.Indices.Select(index => (uint)index));
            }

            _meshes.Add(new Mesh(vertices, indices));
        }
    }

    public void Draw()
    {
        foreach (var mesh in _meshes)
        {
            mesh.Draw();
        }
    }
}

public class Mesh
{
    private int _vao, _vbo, _ebo;
    private List<Vector3> _vertices;
    private List<uint> _indices;

    public Mesh(List<Vector3> vertices, List<uint> indices)
    {
        _vertices = vertices;
        _indices = indices;

        SetupMesh();
    }

    private void SetupMesh()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
}