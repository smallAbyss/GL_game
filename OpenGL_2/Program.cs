using OpenGL_2;
using OpenTK.Windowing.Common;
class Program
{
    static void Main(string[] args)
    {
        using (Game_pat game = new Game_pat(1000, 900, "LearnOpenTK"))
        {
            game.Run();


        }
    }
}
