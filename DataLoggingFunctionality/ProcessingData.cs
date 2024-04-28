using System.Text.Json;
using CoordinatesDataClasses;
using APIData;
using Utils;

namespace WeatherDataLogging
{
    public class ProcessData
    {
        static CityCoordinates cityCoordinates = new CityCoordinates();
        public static WeatherDataLog ProcessAPIWeatherData(APIWeatherData aPIWeatherData, int countryChoice, int cityChoice)
        {
            return new WeatherDataLog
            {
                City = cityCoordinates.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
                Time = aPIWeatherData?.properties?.meta?.updated_at,
                Temperature = aPIWeatherData.properties.timeseries[0].data.instant.details.air_temperature,
                CloudAreaFraction = aPIWeatherData.properties.timeseries[0].data.instant.details.cloud_area_fraction,
                Humidity = aPIWeatherData.properties.timeseries[0].data.instant.details.relative_humidity,
                WindSpeed = aPIWeatherData.properties.timeseries[0].data.instant.details.wind_speed,
                PrecipitationAmount = aPIWeatherData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount
            };
        }
        public static WeatherDataLog GetUserWeatherData(CityCoordinates cityCoordinates, int countryChoice, int cityChoice)
        {
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the temperature (°C): "));
            double temperature = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the cloud area fraction (%): "));
            double cloudAreaFraction = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the relative humidity (%): "));
            double relativeHumidity = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the wind speed (m/s): "));
            double windSpeed = double.Parse(Console.ReadLine());
            Console.Clear();
            Output.WriteInGray(Output.Reset("Enter the precipitation amount (mm): "));
            double precipitationAmount = double.Parse(Console.ReadLine());
            Console.Clear();
            return new WeatherDataLog
            {
                City = cityCoordinates.CountryCityCoordinates[countryChoice].cities[cityChoice].city,
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
        public static string ConvertTimeFormat(string inputTime)
        {
            DateTime dt = DateTime.Parse(inputTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return dt.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}