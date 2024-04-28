using System.Text.Json;
using CoordinatesDataClasses;
using APIData;
using Utils;

namespace WeatherDataLogging
{
    public class ProcessData
    {
        static CityCoordinates cityCoordinates = new CityCoordinates();
        static WeatherDataLog weatherLogEntry = new WeatherDataLog();
        public static WeatherDataLog ProcessAPIWeatherData(APIWeatherData aPIWeatherData, int countryIndex, int cityIndex)
        {
            return new WeatherDataLog
            {
                city = cityCoordinates?.CountryCityCoordinates?[countryIndex].cities?[cityIndex].city,
                time = aPIWeatherData?.properties?.meta?.updated_at,
                temperature = aPIWeatherData.properties.timeseries[0].data.instant.details.air_temperature,
                cloudAreaFraction = aPIWeatherData.properties.timeseries[0].data.instant.details.cloud_area_fraction,
                humidity = aPIWeatherData.properties.timeseries[0].data.instant.details.relative_humidity,
                windSpeed = aPIWeatherData.properties.timeseries[0].data.instant.details.wind_speed,
                precipitationAmount = aPIWeatherData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount
            };
        }
        public static WeatherDataLog GetUserWeatherData(CityCoordinates cityCoordinates, int countryIndex, int cityIndex)
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
                city = cityCoordinates?.CountryCityCoordinates?[countryIndex].cities?[cityIndex].city,
                time = DateTime.Now.ToString(),
                temperature = temperature,
                cloudAreaFraction = cloudAreaFraction,
                precipitationAmount = precipitationAmount,
                humidity = relativeHumidity,
                windSpeed = windSpeed
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
            var comparisonData = new List<WeatherDataLog>();
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                if (i < userData?.Length && i < apiData?.Length)
                {
                    string convertedAPITime = ConvertTimeFormat(apiData[i].time);
                    comparisonData.Add(new WeatherDataLog
                    {
                        city = userData[i].city,
                        time = $"User: {userData[i].time}  | API: {convertedAPITime}",
                        temperature = Math.Round(userData[i].temperature - apiData[i].temperature, 1),
                        cloudAreaFraction = Math.Round(userData[i].cloudAreaFraction - apiData[i].cloudAreaFraction, 1),
                        humidity = Math.Round(userData[i].humidity - apiData[i].humidity, 1),
                        windSpeed = Math.Round(userData[i].windSpeed - apiData[i].windSpeed, 1),
                        precipitationAmount = Math.Round(userData[i].precipitationAmount - apiData[i].precipitationAmount, 1)
                    });
                }
                else
                {
                    break;
                }
            }
            return comparisonData.ToArray();
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
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                if (i < userData?.Length && i < apiData?.Length)
                {
                    totalTemperatureDeviation += Math.Abs(userData[i].temperature - apiData[i].temperature);
                    totalCloudAreaFractionDeviation += Math.Abs(userData[i].cloudAreaFraction - apiData[i].cloudAreaFraction);
                    totalHumidityDeviation += Math.Abs(userData[i].humidity - apiData[i].humidity);
                    totalWindSpeedDeviation += Math.Abs(userData[i].windSpeed - apiData[i].windSpeed);
                    totalPrecipitationAmountDeviation += Math.Abs(userData[i].precipitationAmount - apiData[i].precipitationAmount);
                }
                else
                {
                    break;
                }
            }
            if (amountOfLogEntries == 0)
            {
                return null;
            }
            return new WeatherDataLog
            {
                temperature = Math.Round(totalTemperatureDeviation / amountOfLogEntries, 2),
                cloudAreaFraction = Math.Round(totalCloudAreaFractionDeviation / amountOfLogEntries, 2),
                humidity = Math.Round(totalHumidityDeviation / amountOfLogEntries, 2),
                windSpeed = Math.Round(totalWindSpeedDeviation / amountOfLogEntries, 2),
                precipitationAmount = Math.Round(totalPrecipitationAmountDeviation / amountOfLogEntries, 2)
            };
        }
        public static string ConvertTimeFormat(string inputTime)
        {
            DateTime dt = DateTime.Parse(inputTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return dt.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}