using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;

namespace LoggingWeatherDataClasses
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
    }
}