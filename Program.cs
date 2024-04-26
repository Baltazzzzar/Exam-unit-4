using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using APIWeatherDataClasses;
using CoordinatesDataClasses;
using System.Transactions;
using UserWeatherDataClasses;
using System.Security.Cryptography.X509Certificates;


namespace Program
{
    public class Program
    {
        static HttpClient client = new HttpClient();
        static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();

        public static async Task Main()
        {
            Directory.CreateDirectory("weatherdatalogs");
            double latitude;
            double longitude;


            Console.Clear();
            Console.WriteLine("Choose a country:");


            int index = 1;
            foreach (var countryData in coordinatesDataClass.CountryCityCoordinates)
            {
                Console.WriteLine($"{index}. {countryData.country}");
                index++;
            }
            int countryChoice = int.Parse(Console.ReadLine());

            Console.Clear();
            Console.WriteLine($"Choose a city in {coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].country}: ");

            index = 1;
            foreach (var cityData in coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].cities)
            {
                Console.WriteLine($"{index}. {cityData.city}");
                index++;
            }

            int cityChoice = int.Parse(Console.ReadLine());
            Coordinates coordinates = coordinatesDataClass.GetCoordinates(countryChoice - 1, cityChoice - 1);
            latitude = coordinates.Latitude;
            longitude = coordinates.Longitude;
            Console.Clear();


            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";

            APIWeatherDetails aPIWeatherDetails = null;

            try
            {
                string responseBody = await FetchWeatherData(url);
                File.WriteAllText("weatherdatalogs/apirawweatherdata.json", responseBody);
                APIWeatherData weatherJsonData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);

                Program.PrintWeatherData(weatherJsonData, countryChoice, cityChoice);

                aPIWeatherDetails = new APIWeatherDetails();
                {
                    aPIWeatherDetails.City = coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].cities[cityChoice - 1].city;
                    aPIWeatherDetails.Time = weatherJsonData.properties.meta.updated_at;
                    aPIWeatherDetails.Temperature = weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature;
                    aPIWeatherDetails.CloudAreaFraction = weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction;
                    aPIWeatherDetails.Humidity = weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity;
                    aPIWeatherDetails.WindSpeed = weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed;
                    aPIWeatherDetails.PrecipitationAmount = weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount;
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            // Fetch Data from the user

            Console.WriteLine("Do you have any data to share? (y/n)");
            string userResponse = Console.ReadLine();
            if (userResponse == "y")
            {
                Console.WriteLine("Enter the temperature (°C): ");
                double temperature = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter the cloud area fraction (%): ");
                double cloudAreaFraction = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter the precipitation amount (mm): ");
                double precipitationAmount = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter the relative humidity (%): ");
                double relativeHumidity = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter the wind speed (m/s): ");
                double windSpeed = double.Parse(Console.ReadLine());

                UserWeatherDetails userWeatherDetails = new UserWeatherDetails
                {
                    City = coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].cities[cityChoice - 1].city,
                    Time = DateTime.Now.ToString(),
                    Temperature = temperature,
                    CloudAreaFraction = cloudAreaFraction,
                    PrecipitationAmount = precipitationAmount,
                    Humidity = relativeHumidity,
                    WindSpeed = windSpeed
                };
                Console.WriteLine("Do you want to save the data to a file? (y/n)");
                if (Console.ReadLine() == "y")
                {
                    UserWeatherDetails.SaveUserWeatherData(userWeatherDetails);
                    APIWeatherDetails.SaveAPIWeatherData(aPIWeatherDetails);
                }
            }
        }
        public static async Task<string> FetchWeatherData(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/Baltazzzzar/Exam-Unit-4)");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static void PrintWeatherData(APIWeatherData weatherJsonData, int countryChoice, int cityChoice)
        {
            Console.WriteLine("Weather in: " + coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].cities[cityChoice - 1].city + ", " + coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].country);
            Console.WriteLine("Last updated: " + weatherJsonData.properties.meta.updated_at);
            Console.WriteLine("Temperature: " + weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature + "°C");
            Console.WriteLine("Cloud area fraction: " + weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction + "%");
            Console.WriteLine("Relative humidity: " + weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity + "%");
            Console.WriteLine("Wind speed: " + weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed + "m/s");
            Console.WriteLine("Precipitation amount: " + weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount + "mm");
        }
        public class APIWeatherDetails
        {
            public string City { get; set; }
            public string Time { get; set; }
            public double Temperature { get; set; }
            public double CloudAreaFraction { get; set; }
            public double Humidity { get; set; }
            public double WindSpeed { get; set; }
            public double PrecipitationAmount { get; set; }

            public static void SaveAPIWeatherData(APIWeatherDetails apiWeatherDetails)
            {
                string filePath = "weatherdatalogs/apiweatherdatalog.json";
                List<APIWeatherDetails> allDetails = new List<APIWeatherDetails>();

                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    string entriesJson = File.ReadAllText(filePath);
                    allDetails = JsonSerializer.Deserialize<List<APIWeatherDetails>>(entriesJson);
                }

                allDetails.Add(apiWeatherDetails);
                string newJsonEntries = JsonSerializer.Serialize(allDetails);
                File.WriteAllText(filePath, newJsonEntries);
            }
        }

    }
}