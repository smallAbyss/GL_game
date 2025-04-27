using System.Collections.Generic;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
public class Mesh
{
    private int _vao, _vbo, _ebo;
    private List<Vector3> _vertices;
    private List<uint> _indices;

    // Дополнительно: для инстансинга
    private int _instanceVbo;
    private bool _instancingEnabled = false;
    private int _instanceCount = 0;

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

    // Новый метод — для инстансинга
    public void SetupInstancing(float[] instanceData, int instanceCount)
    {
        _instanceCount = instanceCount;
        _instancingEnabled = true;

        _instanceVbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, instanceData.Length * sizeof(float), instanceData, BufferUsageHint.StaticDraw);

        int stride = (16 + 3) * sizeof(float); // матрица + цвет

        for (int i = 0; i < 4; i++)
        {
            GL.EnableVertexAttribArray(1 + i);
            GL.VertexAttribPointer(1 + i, 4, VertexAttribPointerType.Float, false, stride, i * 4 * sizeof(float));
            GL.VertexAttribDivisor(1 + i, 1);
        }

        GL.EnableVertexAttribArray(5);
        GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, 16 * sizeof(float));
        GL.VertexAttribDivisor(5, 1);

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void DrawInstanced()
    {
        if (!_instancingEnabled) return;

        GL.BindVertexArray(_vao);
        GL.DrawElementsInstanced(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero, _instanceCount);
        GL.BindVertexArray(0);
    }
}
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

            foreach (var vertex in mesh.Vertices)
            {
                vertices.Add(new Vector3(vertex.X, vertex.Y, vertex.Z));
            }

            foreach (var face in mesh.Faces)
            {
                indices.AddRange(face.Indices.Select(index => (uint)index));
            }

            _meshes.Add(new Mesh(vertices, indices));
        }
    }

    public void SetupInstancing(float[] instanceData, int instanceCount)
    {
        foreach (var mesh in _meshes)
        {
            mesh.SetupInstancing(instanceData, instanceCount);
        }
    }

    public void Draw()
    {
        foreach (var mesh in _meshes)
        {
            mesh.Draw();
        }
    }

    public void DrawInstanced()
    {
        foreach (var mesh in _meshes)
        {
            mesh.DrawInstanced();
        }
    }
}
