using OpenGL_2;
using OpenTK.Windowing.Common;
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(800, 400, "LearnOpenTK"))
        {
            game.Run();

        }
    }
}
