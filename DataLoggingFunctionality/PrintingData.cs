using System.Text.Json;
using CoordinatesDataClasses;
using APIData;
using Utils;

namespace WeatherDataLogging
{
    public class PrintingData
    {
        static CityCoordinates cityCoordinates = new CityCoordinates();
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
                Output.WriteInBlue(Output.Reset("Curent Day Weather Report:"), true);
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
                        Output.WriteInBlue(Output.Reset($"User data entry {i + 1}:"));
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
                        Output.WriteInBlue(Output.Reset($"API data entry {i + 1}:"));
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
                    Output.WriteInBlue(Output.Reset($"Comparison for data entry {i + 1}:"), true);
                    Output.WriteInBlue(Output.Reset("Difference in data (User - API):"));
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
            Output.WriteInBlue(Output.Reset("Average deviation:"), true);
            PrintWeatherData(null, null, cityChoice, countryChoice, null, averageDeviation);
            Output.WriteInGreen(Output.Reset("Press any key to return"));
            Console.ReadLine();
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
                city = cityCoordinates.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
                time = ProcessData.ConvertTimeFormat(weatherJsonData.properties.meta.updated_at);
                temperature = weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature;
                cloudAreaFraction = weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction;
                precipitationAmount = weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount;
                humidity = weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity;
                windSpeed = weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed;
            }
            else if (weatherDataLog != null)
            {
                city = weatherDataLog.City;
                time = ProcessData.ConvertTimeFormat(weatherDataLog.Time);
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
                city = cityCoordinates.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
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