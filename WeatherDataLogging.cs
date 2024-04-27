using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;
using CoordinatesDataClasses;
using APIWeatherDataClasses;
using Utils;

namespace LoggingWeatherDataClass
{
    public class WeatherDataLog
    {
        public string City { get; set; }
        public string Time { get; set; }
        public double Temperature { get; set; }
        public double CloudAreaFraction { get; set; }
        public double PrecipitationAmount { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();
        static readonly HttpClient client = new HttpClient();

        public static void SaveWeatherData(WeatherDataLog userWeatherLogEntry, string filePath)
        {
            try
            {
                List<WeatherDataLog> allDetails = new List<WeatherDataLog>();
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    string json = File.ReadAllText(filePath);
                    allDetails = JsonSerializer.Deserialize<List<WeatherDataLog>>(json);
                }
                allDetails.Add(userWeatherLogEntry);
                string newJsonEntries = JsonSerializer.Serialize(allDetails);
                File.WriteAllText(filePath, newJsonEntries);
            }
            catch (Exception ex)
            {
                Output.Write(Output.Bold(Output.Color($"An error occurred: {ex.Message}", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""));
                Console.WriteLine();
            }
        }
        public static void PrintWeatherDataLog(string filePathUserData, string filePathAPIData, int amountOfLogEntries)
        {
            if (File.Exists(filePathUserData) && new FileInfo(filePathUserData).Length > 0 && File.Exists(filePathAPIData) && new FileInfo(filePathAPIData).Length > 0)
            {
                string jsonUserData = File.ReadAllText(filePathUserData);
                string jsonAPIData = File.ReadAllText(filePathAPIData);
                var userData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonUserData);
                var apiData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonAPIData);
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
                        break;
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
                        Output.Write(Output.Bold(Output.Color("No more API data in the log", ANSICodes.Colors.Red)));
                        Output.Write(Output.Reset(""));
                    }
                }
            }
            else
            {
                Output.Write(Output.Bold(Output.Color("No logged data available", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""));
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to return");
            Console.ReadLine();
        }
        public static void CompareData(string filePathUserData, string filePathAPIData, int amountOfLogEntries)
        {
            if (!File.Exists(filePathUserData) || !File.Exists(filePathAPIData) || new FileInfo(filePathUserData).Length == 0 || new FileInfo(filePathAPIData).Length == 0)
            {
                Console.Clear();
                Output.Write(Output.Bold(Output.Color("No data to compare", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""), true);
                Console.WriteLine("Press any key to return");
                Console.ReadLine();
                return;
            }
            string jsonUserData = File.ReadAllText(filePathUserData);
            string jsonAPIData = File.ReadAllText(filePathAPIData);
            var userData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonUserData);
            var apiData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonAPIData);
            Console.Clear();
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                if (i < userData.Length && i < apiData.Length)
                {
                    Console.WriteLine($"Comparison for data entry {i + 1}:");
                    Console.WriteLine("Difference in data (User - API):");
                    Console.WriteLine();
                    Console.WriteLine("Temperature: " + Math.Round(userData[i].Temperature - apiData[i].Temperature, 1) + "°C");
                    Console.WriteLine("Cloud area fraction: " + Math.Round(userData[i].CloudAreaFraction - apiData[i].CloudAreaFraction, 1) + "%");
                    Console.WriteLine("Relative humidity: " + Math.Round(userData[i].Humidity - apiData[i].Humidity, 1) + "%");
                    Console.WriteLine("Wind speed: " + Math.Round(userData[i].WindSpeed - apiData[i].WindSpeed, 1) + "m/s");
                    Console.WriteLine("Precipitation amount: " + Math.Round(userData[i].PrecipitationAmount - apiData[i].PrecipitationAmount, 1) + "mm");
                    Console.WriteLine();
                }
                else
                {
                    Output.Write(Output.Bold(Output.Color($"No more data to compare for entry {i + 1}.", ANSICodes.Colors.Red)));
                    Output.Write(Output.Reset(""));
                    Console.WriteLine("Press any key to return");
                    Console.ReadLine();
                    break;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to return");
            Console.ReadLine();
        }
        public static WeatherDataLog GetUserWeatherData(CoordinatesDataClass coordinatesDataClass, int countryChoice, int cityChoice)
        {
            Console.Clear();
            Console.WriteLine("Enter the temperature (°C): ");
            double temperature = double.Parse(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Enter the cloud area fraction (%): ");
            double cloudAreaFraction = double.Parse(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Enter the precipitation amount (mm): ");
            double precipitationAmount = double.Parse(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Enter the relative humidity (%): ");
            double relativeHumidity = double.Parse(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Enter the wind speed (m/s): ");
            double windSpeed = double.Parse(Console.ReadLine());
            return new WeatherDataLog
            {
                City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
                Time = DateTime.Now.ToString(),
                Temperature = temperature,
                CloudAreaFraction = cloudAreaFraction,
                PrecipitationAmount = precipitationAmount,
                Humidity = relativeHumidity,
                WindSpeed = windSpeed
            };
        }
        public static void PrintWeatherData(APIWeatherData weatherJsonData, int countryChoice, int cityChoice)
        {
            Console.Clear();
            if (weatherJsonData == null)
            {
                Output.Write(Output.Bold(Output.Color("Invalid data", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""));
                return;
            }
            else
            {
                Console.WriteLine("Weather in: " + coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city + ", " + coordinatesDataClass.CountryCityCoordinates[countryChoice].country);
                Console.WriteLine("Last updated: " + weatherJsonData.properties.meta.updated_at);
                Console.WriteLine("Temperature: " + weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature + "°C");
                Console.WriteLine("Cloud area fraction: " + weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction + "%");
                Console.WriteLine("Relative humidity: " + weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity + "%");
                Console.WriteLine("Wind speed: " + weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed + "m/s");
                Console.WriteLine("Precipitation amount: " + weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount + "mm");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to return");
            Console.ReadLine();
        }

        public static WeatherDataLog ProcessWeatherData(string responseBody, int countryChoice, int cityChoice)
        {
            APIWeatherData weatherJsonData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);

            WeatherDataLog aPIWeatherDetails = new WeatherDataLog
            {
                City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
                Time = weatherJsonData.properties.meta.updated_at,
                Temperature = weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature,
                CloudAreaFraction = weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction,
                Humidity = weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity,
                WindSpeed = weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed,
                PrecipitationAmount = weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount
            };
            return aPIWeatherDetails;
        }
        public static async Task<string> FetchWeatherData(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/Baltazzzzar/Exam-Unit-4)");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}