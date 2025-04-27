using OpenGL_2;
using OpenTK.Mathematics;

internal class FlowerField
{
    private List<FlowerInstance> flowers = new List<FlowerInstance>();

    private Model flowerModel;
    private bool initialized = false;

    public void LoadFlowerModel(string path)
    {
        flowerModel = new Model();
        flowerModel.LoadModel(path);
    }

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

            Matrix4 modelMatrix = Matrix4.CreateScale(scale) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation)) * Matrix4.CreateTranslation(new Vector3(x, y, z));

            flowers.Add(new FlowerInstance
            {
                ModelMatrix = modelMatrix,
                Color = new Vector3(
                    1.0f,
                    (float)(random.NextDouble() * 0.3f + 0.7f),
                    (float)(random.NextDouble())
                )
            });
        }

        Initialize();
    }

    private void Initialize()
    {
        if (initialized || flowerModel == null) return;

        List<float> instanceData = new List<float>();

        foreach (var flower in flowers)
        {
            var m = flower.ModelMatrix;

            // матрица
            instanceData.Add(m.M11); instanceData.Add(m.M21); instanceData.Add(m.M31); instanceData.Add(m.M41);
            instanceData.Add(m.M12); instanceData.Add(m.M22); instanceData.Add(m.M32); instanceData.Add(m.M42);
            instanceData.Add(m.M13); instanceData.Add(m.M23); instanceData.Add(m.M33); instanceData.Add(m.M43);
            instanceData.Add(m.M14); instanceData.Add(m.M24); instanceData.Add(m.M34); instanceData.Add(m.M44);

            // цвет
            instanceData.Add(flower.Color.X);
            instanceData.Add(flower.Color.Y);
            instanceData.Add(flower.Color.Z);
        }

        flowerModel.SetupInstancing(instanceData.ToArray(), flowers.Count);

        initialized = true;
    }

    public void Draw(Shader shader, Camera camera)
    {
        shader.Use();
        Matrix4 sc = Matrix4.CreateScale(0.2f);
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("scale", sc);
        shader.SetMatrix4("projection", camera.GetProjection());

        flowerModel.DrawInstanced();
    }
}

internal class FlowerInstance
{
    public Matrix4 ModelMatrix;
    public Vector3 Color;
}
