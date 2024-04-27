using System.Text.Json;
using CoordinatesDataClasses;
using APIWeatherDataClasses;
using Utils;

namespace LoggingWeatherDataClass
{
    public class WeatherDataLog
    {
        public string? City { get; set; }
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public double CloudAreaFraction { get; set; }
        public double PrecipitationAmount { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();
        public static WeatherDataLog? userWeatherDetails = null;
        public static WeatherDataLog? aPIWeatherDetails = null;
        public static WeatherDataLog[]? comparisonData = null;
        static readonly HttpClient client = new HttpClient();

        public static void SaveWeatherData(WeatherDataLog userWeatherLogEntry, string filePath)
        {
            List<WeatherDataLog>? allDetails = new List<WeatherDataLog>();
            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                string json = File.ReadAllText(filePath);
                allDetails = JsonSerializer.Deserialize<List<WeatherDataLog>>(json);
            }
            allDetails?.Add(userWeatherLogEntry);
            string newJsonEntries = JsonSerializer.Serialize(allDetails);
            File.WriteAllText(filePath, newJsonEntries);
        }
        public static void PrintWeatherDataLog(string filePathUserData, string filePathAPIData, int amountOfLogEntries)
        {
            Console.Clear();
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
                    if (userCount < userData?.Length)
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
                        Output.WriteInRed(Output.Reset("No more user data in the log"), true);
                        break;
                    }
                    if (apiCount < apiData?.Length)
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
                        Output.WriteInRed(Output.Reset("No more API data in the log"), true);
                    }
                }
            }
            else
            {
                Output.WriteInRed(Output.Reset("No logged data available"), true);
            }
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
        }
        public static WeatherDataLog[] CompareData(string filePathUserData, string filePathAPIData, int amountOfLogEntries)
        {
            if (!File.Exists(filePathUserData) || !File.Exists(filePathAPIData) || new FileInfo(filePathUserData).Length == 0 || new FileInfo(filePathAPIData).Length == 0)
            {
                return null;
            }
            string jsonUserData = File.ReadAllText(filePathUserData);
            string jsonAPIData = File.ReadAllText(filePathAPIData);
            var userData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonUserData);
            var apiData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonAPIData);

            var comparisonData = new WeatherDataLog[amountOfLogEntries];
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                if (i < userData?.Length && i < apiData?.Length)
                {
                    comparisonData[i] = new WeatherDataLog
                    {
                        Temperature = Math.Round(userData[i].Temperature - apiData[i].Temperature, 1),
                        CloudAreaFraction = Math.Round(userData[i].CloudAreaFraction - apiData[i].CloudAreaFraction, 1),
                        Humidity = Math.Round(userData[i].Humidity - apiData[i].Humidity, 1),
                        WindSpeed = Math.Round(userData[i].WindSpeed - apiData[i].WindSpeed, 1),
                        PrecipitationAmount = Math.Round(userData[i].PrecipitationAmount - apiData[i].PrecipitationAmount, 1)
                    };
                }
                else
                {
                    break;
                }
            }
            return comparisonData;
        }

        public static void PrintComparisonData(WeatherDataLog[] comparisonData)
        {
            Console.Clear();
            if (comparisonData == null)
            {
                Output.WriteInRed(Output.Reset("No data to compare"), true);
                Output.WriteInGreen(Output.Reset("Press any key to return"));
                Console.ReadLine();
                return;
            }
            for (int i = 0; i < comparisonData.Length; i++)
            {
                if (comparisonData[i] != null)
                {
                    Output.WriteInYellow(Output.Reset($"Comparison for data entry {i + 1}:"), true);
                    Output.WriteInYellow(Output.Reset("Difference in data (User - API):"), true);
                    Console.WriteLine();
                    Console.WriteLine("Temperature: " + comparisonData[i].Temperature + "°C");
                    Console.WriteLine("Cloud area fraction: " + comparisonData[i].CloudAreaFraction + "%");
                    Console.WriteLine("Relative humidity: " + comparisonData[i].Humidity + "%");
                    Console.WriteLine("Wind speed: " + comparisonData[i].WindSpeed + "m/s");
                    Console.WriteLine("Precipitation amount: " + comparisonData[i].PrecipitationAmount + "mm");
                    Console.WriteLine();
                }
                else
                {
                    Output.WriteInRed(Output.Reset($"No more data to compare for entry {i + 1}."), true);
                    break;
                }
            }
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
        }
        public static WeatherDataLog GetUserWeatherData(CoordinatesDataClass coordinatesDataClass, int countryChoice, int cityChoice)
        {
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the temperature (°C): "));
            double temperature = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the cloud area fraction (%): "));
            double cloudAreaFraction = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the precipitation amount (mm): "));
            double precipitationAmount = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the relative humidity (%): "));
            double relativeHumidity = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the wind speed (m/s): "));
            double windSpeed = double.Parse(Console.ReadLine());
            Console.Clear();
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
        public static void PrintWeatherReport(APIWeatherData weatherJsonData, int countryChoice, int cityChoice)
        {
            Console.Clear();
            if (weatherJsonData == null)
            {
                Output.WriteInRed(Output.Reset("Invalid data"));
                return;
            }
            else
            {
                Output.WriteInYellow(Output.Reset("Curent Day Weather Report:"), true);
                Console.WriteLine();
                Output.WriteInGray(Output.Reset("Weather in: "));
                Console.WriteLine(coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city + ", " + coordinatesDataClass.CountryCityCoordinates[countryChoice].country);
                PrintWeatherData(weatherJsonData);
            }
            Console.WriteLine();
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
        }
        public static void PrintWeatherData(APIWeatherData weatherJsonData)
        {
            Output.WriteInGray(Output.Reset("Last updated: "));
            Console.WriteLine(weatherJsonData?.properties?.meta?.updated_at);
            Output.WriteInGray(Output.Reset("Temperature: "));
            Console.WriteLine(weatherJsonData?.properties?.timeseries?[0].data?.instant?.details?.air_temperature + "°C");
            Output.WriteInGray(Output.Reset("Cloud area fraction: "));
            Console.WriteLine(weatherJsonData?.properties?.timeseries?[0].data?.instant?.details?.cloud_area_fraction + "%");
            Output.WriteInGray(Output.Reset("Relative humidity: "));
            Console.WriteLine(weatherJsonData?.properties?.timeseries?[0].data?.instant?.details?.relative_humidity + "%");
            Output.WriteInGray(Output.Reset("Wind speed: "));
            Console.WriteLine(weatherJsonData?.properties?.timeseries?[0].data?.instant?.details?.wind_speed + "m/s");
            Output.WriteInGray(Output.Reset("Precipitation amount: "));
            Console.WriteLine(weatherJsonData?.properties?.timeseries?[0].data?.next_1_hours?.details?.precipitation_amount + "mm");
        }

        public static WeatherDataLog ProcessWeatherData(string responseBody, int countryChoice, int cityChoice)
        {
            APIWeatherData? weatherJsonData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);
            WeatherDataLog aPIWeatherDetails = new WeatherDataLog
            {
                City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
                Time = weatherJsonData?.properties?.meta?.updated_at,
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