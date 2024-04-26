using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;

namespace UserWeatherDataClasses
{
    public class UserWeatherDetails
    {
        public string City { get; set; }
        public string Time { get; set; }
        public double Temperature { get; set; }
        public double CloudAreaFraction { get; set; }
        public double PrecipitationAmount { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }

        public static void SaveUserWeatherData(UserWeatherDetails userWeatherLogEntry)
        {
            try
            {
                List<UserWeatherDetails> allDetails = new List<UserWeatherDetails>();
                string filePath = "weatherdatalogs/userweatherdatalog.json";
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    string json = File.ReadAllText(filePath);
                    allDetails = JsonSerializer.Deserialize<List<UserWeatherDetails>>(json);
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