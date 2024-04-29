using System.Text.Json;
using CoordinatesDataClasses;
using APIData;
using HelperFunctions;

namespace WeatherDataLogging
{
    public class ProcessData
    {
        static CityCoordinates cityCoordinates = new CityCoordinates();
        public static WeatherDataLog ProcessAPIWeatherData(APIWeatherData aPIWeatherData, int countryIndex, int cityIndex, int manualHourAdjustment = 0)
        {
            int forecastAccuracyAdjustment;
            string convertedAPITime = HelpingFunctions.ConvertTimeFormat(aPIWeatherData.properties.timeseries[0].time);
            forecastAccuracyAdjustment = Convert.ToInt32(HelpingFunctions.GetHourDifference(convertedAPITime, DateTime.Now.ToString()));
            if (manualHourAdjustment > 0)
            {
                forecastAccuracyAdjustment = forecastAccuracyAdjustment - manualHourAdjustment;
            }
            if (forecastAccuracyAdjustment < 0)
            {
                forecastAccuracyAdjustment = 0;
            }
            return new WeatherDataLog
            {
                city = cityCoordinates?.CountryCityCoordinates?[countryIndex].cities?[cityIndex].city,
                time = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].time,
                temperature = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].data.instant.details.air_temperature,
                cloudAreaFraction = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].data.instant.details.cloud_area_fraction,
                humidity = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].data.instant.details.relative_humidity,
                windSpeed = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].data.instant.details.wind_speed,
                precipitationAmount = aPIWeatherData.properties.timeseries[forecastAccuracyAdjustment].data.next_1_hours.details.precipitation_amount,
            };
        }
        public static WeatherDataLog GetUserWeatherData(CityCoordinates cityCoordinates, int countryIndex, int cityIndex)
        {
            Console.Clear();
            int hoursAgo = HelpingFunctions.GetValidInt("How many hours ago was the weather data recorded? ");
            return new WeatherDataLog
            {
                city = cityCoordinates?.CountryCityCoordinates?[countryIndex].cities?[cityIndex].city,
                time = DateTime.Now.AddHours(-hoursAgo).ToString(),
                temperature = HelpingFunctions.GetValidDouble("Enter the temperature (°C): "),
                cloudAreaFraction = HelpingFunctions.GetValidDouble("Enter the cloud area fraction (%): "),
                precipitationAmount = HelpingFunctions.GetValidDouble("Enter the precipitation amount (mm): "),
                humidity = HelpingFunctions.GetValidDouble("Enter the humidity (%): "),
                windSpeed = HelpingFunctions.GetValidDouble("Enter the wind speed (m/s): "),
                hourAdjustment = hoursAgo
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
        public static WeatherDataLog[] CompareData(string filePathUserData, string filePathAPIData)
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
            for (int i = 0; i < Math.Min(userData.Length, apiData.Length); i++)
            {
                int reverseIndex = userData.Length - 1 - i;
                string convertedAPITime = HelpingFunctions.ConvertTimeFormat(apiData[reverseIndex].time);
                comparisonData.Add(new WeatherDataLog
                {
                    city = userData[reverseIndex].city,
                    time = $"User: {userData[reverseIndex].time}  | API: {convertedAPITime}",
                    temperature = Math.Round(userData[reverseIndex].temperature - apiData[reverseIndex].temperature, 1),
                    cloudAreaFraction = Math.Round(userData[reverseIndex].cloudAreaFraction - apiData[reverseIndex].cloudAreaFraction, 1),
                    humidity = Math.Round(userData[reverseIndex].humidity - apiData[reverseIndex].humidity, 1),
                    windSpeed = Math.Round(userData[reverseIndex].windSpeed - apiData[reverseIndex].windSpeed, 1),
                    precipitationAmount = Math.Round(userData[reverseIndex].precipitationAmount - apiData[reverseIndex].precipitationAmount, 1)
                });
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
            if (amountOfLogEntries > userData.Length || amountOfLogEntries > apiData.Length)
            {
                amountOfLogEntries = Math.Min(userData.Length, apiData.Length);
            }
            for (int i = 0; i < amountOfLogEntries; i++)
            {
                totalTemperatureDeviation += Math.Abs(userData[i].temperature - apiData[i].temperature);
                totalCloudAreaFractionDeviation += Math.Abs(userData[i].cloudAreaFraction - apiData[i].cloudAreaFraction);
                totalHumidityDeviation += Math.Abs(userData[i].humidity - apiData[i].humidity);
                totalWindSpeedDeviation += Math.Abs(userData[i].windSpeed - apiData[i].windSpeed);
                totalPrecipitationAmountDeviation += Math.Abs(userData[i].precipitationAmount - apiData[i].precipitationAmount);
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

    }
}