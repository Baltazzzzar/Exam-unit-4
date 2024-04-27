using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using APIWeatherDataClasses;
using CoordinatesDataClasses;
using System.Transactions;
using LoggingWeatherDataClass;
using System.Security.Cryptography.X509Certificates;
using Menu;


namespace Program
{
    public class Program
    {
        public static async Task Main()
        {
            MenuScreen menuScreen = new MenuScreen();
            Directory.CreateDirectory("weatherdatalogs");
            while (true)
            {
                menuScreen.input();
                menuScreen.update();
                menuScreen.draw();
                Thread.Sleep(50);
            }
        }
    }
}