using OpenGL_2;
using OpenTK.Windowing.Common;
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(1000, 800, "LearnOpenTK"))
        {
            game.Run();

        }
    }
}
