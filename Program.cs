using Menu;

namespace Program
{
    public class Program
    {
        public static void Main()
        {
            MenuScreen menuScreen = new MenuScreen();
            Directory.CreateDirectory("weatherdatalogs");
            while (true)
            {
                menuScreen.Input();
                menuScreen.Update();
                menuScreen.Draw();
                Thread.Sleep(75);
            }
        }
    }
}