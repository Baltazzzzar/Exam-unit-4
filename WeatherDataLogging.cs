using System.Text.Json;
using CoordinatesDataClasses;
using APIWeatherDataClasses;
using Utils;
using System.Diagnostics.Contracts;

namespace LoggingWeatherDataClass
{
    public class WeatherDataLog
    {
        public string? City { get; set; }
        public string? UnParsedTime { get; set; }
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
        public static WeatherDataLog? averageDeviation = null;
        static readonly HttpClient client = new HttpClient();

        public static async Task<string> SendDataRequest(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/Baltazzzzar/Exam-Unit-4)");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> WriteAPIWeatherData(string url)
        {
            try
            {
                string responseBody = "";
                responseBody = await SendDataRequest(url);
                File.WriteAllText("weatherdatalogs/apirawweatherdata.json", responseBody);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Output.Write(Output.Bold(Output.Color($"An error occurred: {e.Message}", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""));
                Console.ReadLine();
                return null;
            }
        }
        public static WeatherDataLog ProcessWeatherData(APIWeatherData aPIWeatherData, int countryChoice, int cityChoice)
        {
            return new WeatherDataLog
            {
                City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
                Time = aPIWeatherData?.properties?.meta?.updated_at,
                Temperature = aPIWeatherData.properties.timeseries[0].data.instant.details.air_temperature,
                CloudAreaFraction = aPIWeatherData.properties.timeseries[0].data.instant.details.cloud_area_fraction,
                Humidity = aPIWeatherData.properties.timeseries[0].data.instant.details.relative_humidity,
                WindSpeed = aPIWeatherData.properties.timeseries[0].data.instant.details.wind_speed,
                PrecipitationAmount = aPIWeatherData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount
            };
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
        public static void SaveWeatherData(WeatherDataLog weatherLogEntry, string filePath)
        {
            List<WeatherDataLog>? allDetails = new List<WeatherDataLog>();
            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                string json = File.ReadAllText(filePath);
                allDetails = JsonSerializer.Deserialize<List<WeatherDataLog>>(json);
            }
            allDetails?.Add(weatherLogEntry);
            string newJsonEntries = JsonSerializer.Serialize(allDetails);
            File.WriteAllText(filePath, newJsonEntries);
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
                PrintWeatherData(weatherJsonData, null, cityChoice, countryChoice);
            }
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
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
                Console.WriteLine();
                for (int i = 0; i < amountOfLogEntries; i++)
                {
                    if (userCount < userData?.Length)
                    {
                        Output.WriteInYellow(Output.Reset($"User data entry {i + 1}:"));
                        PrintWeatherData(null, userData[userData.Length - 1 - userCount]);
                        userCount++;
                    }
                    else
                    {
                        Output.WriteInRed(Output.Reset("No more user data in the log"), true);
                        break;
                    }
                    if (apiCount < apiData?.Length)
                    {
                        Output.WriteInYellow(Output.Reset($"API data entry {i + 1}:"));
                        PrintWeatherData(null, apiData[apiData.Length - 1 - apiCount]);
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
            Console.Clear();
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
                    string convertedAPITime = ConvertTimeFormat(apiData[i].Time);
                    comparisonData[i] = new WeatherDataLog
                    {
                        City = userData[i].City,
                        Time = $"User: {userData[i].Time}  | API: {convertedAPITime}",
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
        public static void PrintComparisonData(WeatherDataLog[] comparisonData, int countryChoice, int cityChoice)
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
                    Output.WriteInYellow(Output.Reset("Difference in data (User - API):"));
                    PrintWeatherData(null, null, cityChoice, countryChoice, comparisonData);
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
        public static WeatherDataLog CalculateAverageDeviation(string filePathUserData, string filePathAPIData, int amountOfLogEntries)
        {
            if (!File.Exists(filePathUserData) || !File.Exists(filePathAPIData) || new FileInfo(filePathUserData).Length == 0 || new FileInfo(filePathAPIData).Length == 0)
            {
                return null;
            }
            string jsonUserData = File.ReadAllText(filePathUserData);
            string jsonAPIData = File.ReadAllText(filePathAPIData);
            var userData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonUserData);
            var apiData = JsonSerializer.Deserialize<WeatherDataLog[]>(jsonAPIData);
            double totalTemperatureDeviation = 0;
            double totalCloudAreaFractionDeviation = 0;
            double totalHumidityDeviation = 0;
            double totalWindSpeedDeviation = 0;
            double totalPrecipitationAmountDeviation = 0;
            int count = 0;
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                if (i < userData?.Length && i < apiData?.Length)
                {
                    totalTemperatureDeviation += Math.Abs(userData[i].Temperature - apiData[i].Temperature);
                    totalCloudAreaFractionDeviation += Math.Abs(userData[i].CloudAreaFraction - apiData[i].CloudAreaFraction);
                    totalHumidityDeviation += Math.Abs(userData[i].Humidity - apiData[i].Humidity);
                    totalWindSpeedDeviation += Math.Abs(userData[i].WindSpeed - apiData[i].WindSpeed);
                    totalPrecipitationAmountDeviation += Math.Abs(userData[i].PrecipitationAmount - apiData[i].PrecipitationAmount);
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == 0)
            {
                return null;
            }
            var averageDeviation = new WeatherDataLog
            {
                Temperature = Math.Round(totalTemperatureDeviation / count, 1),
                CloudAreaFraction = Math.Round(totalCloudAreaFractionDeviation / count, 1),
                Humidity = Math.Round(totalHumidityDeviation / count, 1),
                WindSpeed = Math.Round(totalWindSpeedDeviation / count, 1),
                PrecipitationAmount = Math.Round(totalPrecipitationAmountDeviation / count, 1)
            };
            return averageDeviation;
        }
        public static void PrintAverageDeviation(WeatherDataLog averageDeviation, int cityChoice, int countryChoice)
        {
            Console.Clear();
            if (averageDeviation == null)
            {
                Output.WriteInRed(Output.Reset("No data to calculate average deviation"), true);
                Output.WriteInGreen(Output.Reset("Press any key to return"));
                Console.ReadLine();
                return;
            }
            Output.WriteInYellow(Output.Reset("Average deviation:"), true);
            PrintWeatherData(null, null, cityChoice, countryChoice, null, averageDeviation);
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
        }

        public static string ConvertTimeFormat(string inputTime)
        {
            DateTime dt = DateTime.Parse(inputTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return dt.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static void PrintWeatherData(APIWeatherData? weatherJsonData = null, WeatherDataLog? weatherDataLog = null, int cityChoice = 0, int countryChoice = 0, WeatherDataLog[]? comparisonData = null, WeatherDataLog? averageDeviation = null)
        {
            string city = "";
            string time = "";
            double temperature = 0;
            double cloudAreaFraction = 0;
            double precipitationAmount = 0;
            double humidity = 0;
            double windSpeed = 0;
            bool printTime = true;
            if (weatherJsonData != null)
            {
                city = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
                time = ConvertTimeFormat(weatherJsonData.properties.meta.updated_at);
                temperature = weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature;
                cloudAreaFraction = weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction;
                precipitationAmount = weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount;
                humidity = weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity;
                windSpeed = weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed;
            }
            else if (weatherDataLog != null)
            {
                city = weatherDataLog.City;
                time = ConvertTimeFormat(weatherDataLog.Time);
                temperature = weatherDataLog.Temperature;
                cloudAreaFraction = weatherDataLog.CloudAreaFraction;
                precipitationAmount = weatherDataLog.PrecipitationAmount;
                humidity = weatherDataLog.Humidity;
                windSpeed = weatherDataLog.WindSpeed;
            }
            else if (comparisonData != null)
            {
                city = comparisonData[cityChoice].City;
                time = comparisonData[cityChoice].Time;
                temperature = comparisonData[cityChoice].Temperature;
                cloudAreaFraction = comparisonData[cityChoice].CloudAreaFraction;
                precipitationAmount = comparisonData[cityChoice].PrecipitationAmount;
                humidity = comparisonData[cityChoice].Humidity;
                windSpeed = comparisonData[cityChoice].WindSpeed;
            }
            else if (averageDeviation != null)
            {
                city = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
                temperature = averageDeviation.Temperature;
                cloudAreaFraction = averageDeviation.CloudAreaFraction;
                precipitationAmount = averageDeviation.PrecipitationAmount;
                humidity = averageDeviation.Humidity;
                windSpeed = averageDeviation.WindSpeed;
                printTime = false;
            }
            Console.WriteLine();
            Output.WriteInGray(Output.Reset("City: "));
            Console.WriteLine(city);
            if (printTime)
            {
                Output.WriteInGray(Output.Reset("Time: "));
                Console.WriteLine(time);
            }
            Output.WriteInGray(Output.Reset("Temperature: "));
            Console.WriteLine(temperature + "°C");
            Output.WriteInGray(Output.Reset("Cloud area fraction: "));
            Console.WriteLine(cloudAreaFraction + "%");
            Output.WriteInGray(Output.Reset("Relative humidity: "));
            Console.WriteLine(humidity + "%");
            Output.WriteInGray(Output.Reset("Wind speed: "));
            Console.WriteLine(windSpeed + "m/s");
            Output.WriteInGray(Output.Reset("Precipitation amount: "));
            Console.WriteLine(precipitationAmount + "mm");
            Console.WriteLine();
        }
    }
}