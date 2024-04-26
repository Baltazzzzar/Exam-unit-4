using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;

namespace LoggingWeatherDataClass
{
    public class WeatherDetailsLog
    {
        public string City { get; set; }
        public string Time { get; set; }
        public double Temperature { get; set; }
        public double CloudAreaFraction { get; set; }
        public double PrecipitationAmount { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }

        public static void SaveWeatherData(WeatherDetailsLog userWeatherLogEntry, string filePath)
        {
            try
            {
                List<WeatherDetailsLog> allDetails = new List<WeatherDetailsLog>();
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    string json = File.ReadAllText(filePath);
                    allDetails = JsonSerializer.Deserialize<List<WeatherDetailsLog>>(json);
                }
                allDetails.Add(userWeatherLogEntry);
                string newJsonEntries = JsonSerializer.Serialize(allDetails);
                File.WriteAllText(filePath, newJsonEntries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public static void PrintWeatherDataLog(string filePathUserData, string filePathAPIData, int amountOfLogEntries = 7)
        {
            if (File.Exists(filePathUserData) && new FileInfo(filePathUserData).Length > 0 && File.Exists(filePathAPIData) && new FileInfo(filePathAPIData).Length > 0)
            {
                string jsonUserData = File.ReadAllText(filePathUserData);
                string jsonAPIData = File.ReadAllText(filePathAPIData);
                var userData = JsonSerializer.Deserialize<WeatherDetailsLog[]>(jsonUserData);
                var apiData = JsonSerializer.Deserialize<WeatherDetailsLog[]>(jsonAPIData);
                int userCount = 0;
                int apiCount = 0;
                for (int i = 0; i < amountOfLogEntries; i++)
                {
                    if (userCount < userData.Length)
                    {
                        Console.WriteLine($"User data entry {i + 1}:");
                        Console.WriteLine($"City: {userData[userData.Length - 1 - userCount].City}");
                        Console.WriteLine($"Time: {userData[userData.Length - 1 - userCount].Time}");
                        Console.WriteLine($"Temperature: {userData[userData.Length - 1 - userCount].Temperature}°C");
                        Console.WriteLine($"Cloud area fraction: {userData[userData.Length - 1 - userCount].CloudAreaFraction}%");
                        Console.WriteLine($"Precipitation amount: {userData[userData.Length - 1 - userCount].PrecipitationAmount}mm");
                        Console.WriteLine($"Relative humidity: {userData[userData.Length - 1 - userCount].Humidity}%");
                        Console.WriteLine($"Wind speed: {userData[userData.Length - 1 - userCount].WindSpeed}m/s");
                        Console.WriteLine();
                        userCount++;
                    }
                    else
                    {
                        Console.WriteLine("No more user data in the log");
                        Console.WriteLine();
                    }
                    if (apiCount < apiData.Length)
                    {
                        Console.WriteLine($"API data entry {i + 1}:");
                        Console.WriteLine($"City: {apiData[apiData.Length - 1 - apiCount].City}");
                        Console.WriteLine($"Time: {apiData[apiData.Length - 1 - apiCount].Time}");
                        Console.WriteLine($"Temperature: {apiData[apiData.Length - 1 - apiCount].Temperature}°C");
                        Console.WriteLine($"Cloud area fraction: {apiData[apiData.Length - 1 - apiCount].CloudAreaFraction}%");
                        Console.WriteLine($"Precipitation amount: {apiData[apiData.Length - 1 - apiCount].PrecipitationAmount}mm");
                        Console.WriteLine($"Relative humidity: {apiData[apiData.Length - 1 - apiCount].Humidity}%");
                        Console.WriteLine($"Wind speed: {apiData[apiData.Length - 1 - apiCount].WindSpeed}m/s");
                        Console.WriteLine();
                        apiCount++;
                    }
                    else
                    {
                        Console.WriteLine("No more API data in the log");
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("No logged data available");
            }
        }
    }
}