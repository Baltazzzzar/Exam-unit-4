using System.Text.Json;
using CoordinatesDataClasses;
using APIData;
using Utils;
using HelperFunctions;

namespace WeatherDataLogging
{
    public class PrintData
    {
        static CityCoordinates cityCoordinates = new CityCoordinates();
        public static void PrintWeatherForecast(APIWeatherData weatherJsonData, int countryIndex, int cityIndex, int forecastHour)
        {
            string convertedAPITime = HelpingFunctions.ConvertTimeFormat(weatherJsonData.properties.timeseries[0].time);
            int forecastAccuracyAdjustment = Convert.ToInt32(HelpingFunctions.GetHourDifference(convertedAPITime, DateTime.Now.ToString()));
            int adjustedForecastHour = forecastHour + forecastAccuracyAdjustment;
            Console.Clear();
            if (weatherJsonData == null)
            {
                Output.WriteInRed(Output.Reset("No API data available, sorry"), true);
                return;
            }
            else
            {
                Output.WriteInBlue(Output.Reset("Weather Forecast:"), true);
                PrintWeatherData(weatherJsonData, adjustedForecastHour, null, cityIndex, countryIndex);
            }
            Output.WriteInGreen(Output.Reset("Press any key to return"), true);
            Console.ReadLine();
        }
        public static void PrintWeatherDataLog(string filePathUserData, string filePathAPIData, int amountOfLogEntries, bool entriesAreInDays = true)
        {
            Console.Clear();
            if (entriesAreInDays)
            {
                amountOfLogEntries = HelpingFunctions.FindAmountOfLogEntries(amountOfLogEntries, filePathUserData);
            }
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
                        Output.WriteInBlue(Output.Reset($"User data entry {i + 1}:"));
                        PrintWeatherData(null, 0, userData[userData.Length - 1 - userCount]);
                        userCount++;
                    }
                    else
                    {
                        Output.WriteInRed(Output.Reset("No more user data in the log"), true);
                        break;
                    }
                    if (apiCount < apiData?.Length)
                    {
                        Output.WriteInBlue(Output.Reset($"API data entry {i + 1}:"));
                        PrintWeatherData(null, 0, apiData[apiData.Length - 1 - apiCount]);
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
            Output.WriteInGreen(Output.Reset("Press any key to return"), true);
            Console.ReadLine();
            Console.Clear();
        }
        public static void PrintComparisonData(WeatherDataLog[] comparisonData, int countryIndex, int cityIndex, int amountofLogEntries)
        {
            Console.Clear();
            if (comparisonData == null)
            {
                Output.WriteInRed(Output.Reset("No data to compare"), true);
                Output.WriteInGreen(Output.Reset("Press any key to return"), true);
                Console.ReadLine();
                return;
            }
            int count = amountofLogEntries;
            if (comparisonData.Length < count)
            {
                count = comparisonData.Length;
            }
            for (int i = 0; i < count; i++)
            {
                if (comparisonData[i] != null)
                {
                    Output.WriteInBlue(Output.Reset($"Comparison for data entry {i + 1}:"), true);
                    Output.WriteInBlue(Output.Reset("Difference in data (User - API):"));
                    PrintWeatherData(null, 0, null, i, 0, comparisonData);
                }
            }
            if (count != amountofLogEntries)
            {
                Output.WriteInRed(Output.Reset($"No more data to compare for entry {count + 1}"), true);
            }
            Output.WriteInGreen(Output.Reset("Press any key to return"), true);
            Console.ReadLine();
        }
        public static void PrintAverageDeviation(WeatherDataLog averageDeviation, int cityIndex, int countryIndex)
        {
            Console.Clear();
            if (averageDeviation == null)
            {
                Output.WriteInRed(Output.Reset("No data to calculate average deviation"), true);
                Output.WriteInGreen(Output.Reset("Press any key to return"), true);
                Console.ReadLine();
                return;
            }
            Output.WriteInBlue(Output.Reset("Average deviation:"), true);
            PrintWeatherData(null, 0, null, cityIndex, countryIndex, null, averageDeviation);
            Output.WriteInGreen(Output.Reset("Press any key to return"), true);
            Console.ReadLine();
        }
        public static void PrintWeatherData(APIWeatherData? weatherJsonData = null, int forecastHour = 0, WeatherDataLog? weatherDataLog = null, int cityIndex = 0, int countryIndex = 0, WeatherDataLog[]? comparisonData = null, WeatherDataLog? averageDeviation = null)
        {
            string city = "";
            string time = "";
            double temperature = 0;
            double cloudAreaFraction = 0;
            double precipitationAmount = 0;
            double humidity = 0;
            double windSpeed = 0;
            double precipitationAmountNextHour = 0;
            double precipitationAmountNext6Hours = 0;
            bool printTime = true;
            if (weatherJsonData != null)
            {
                city = cityCoordinates.CountryCityCoordinates[countryIndex].cities[cityIndex].city;
                time = HelpingFunctions.ConvertTimeFormat(weatherJsonData.properties.timeseries[forecastHour].time);
                temperature = weatherJsonData.properties.timeseries[forecastHour].data.instant.details.air_temperature;
                cloudAreaFraction = weatherJsonData.properties.timeseries[forecastHour].data.instant.details.cloud_area_fraction;
                precipitationAmount = weatherJsonData.properties.timeseries[forecastHour].data.next_1_hours.details.precipitation_amount;
                humidity = weatherJsonData.properties.timeseries[forecastHour].data.instant.details.relative_humidity;
                windSpeed = weatherJsonData.properties.timeseries[forecastHour].data.instant.details.wind_speed;
            }
            else if (weatherDataLog != null)
            {
                city = weatherDataLog.city;
                time = HelpingFunctions.ConvertTimeFormat(weatherDataLog.time);
                temperature = weatherDataLog.temperature;
                cloudAreaFraction = weatherDataLog.cloudAreaFraction;
                precipitationAmount = weatherDataLog.precipitationAmount;
                humidity = weatherDataLog.humidity;
                windSpeed = weatherDataLog.windSpeed;
            }
            else if (comparisonData != null)
            {
                city = comparisonData[cityIndex].city;
                time = comparisonData[cityIndex].time;
                temperature = comparisonData[cityIndex].temperature;
                cloudAreaFraction = comparisonData[cityIndex].cloudAreaFraction;
                precipitationAmount = comparisonData[cityIndex].precipitationAmount;
                humidity = comparisonData[cityIndex].humidity;
                windSpeed = comparisonData[cityIndex].windSpeed;
            }
            else if (averageDeviation != null)
            {
                city = cityCoordinates.CountryCityCoordinates[countryIndex].cities[cityIndex].city;
                temperature = averageDeviation.temperature;
                cloudAreaFraction = averageDeviation.cloudAreaFraction;
                precipitationAmount = averageDeviation.precipitationAmount;
                humidity = averageDeviation.humidity;
                windSpeed = averageDeviation.windSpeed;
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
            Output.WriteInGray(Output.Reset("Precipitation: "));
            Console.WriteLine(precipitationAmount + "mm");
            Console.WriteLine();
        }
    }
}