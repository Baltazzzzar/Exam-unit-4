using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherDataClasses;
using CoordinatesDataClasses;




class Program
{
    static HttpClient client = new HttpClient();
    static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();
    public static async Task Main()
    {
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
        try
        {
            string responseBody = await FetchWeatherData(url);
            File.WriteAllText("weatherdata.json", responseBody);
            WeatherData weatherJsonData = JsonSerializer.Deserialize<WeatherData>(responseBody);
            Console.WriteLine("Weather in: " + coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].cities[cityChoice - 1].city + ", " + coordinatesDataClass.CountryCityCoordinates[countryChoice - 1].country);
            Console.WriteLine("Last updated: " + weatherJsonData.properties.meta.updated_at);
            Console.WriteLine("Temperature: " + weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature + "°C");
            Console.WriteLine("Cloud area fraction: " + weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction + "%");
            Console.WriteLine("Relative humidity: " + weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity + "%");
            Console.WriteLine("Wind speed: " + weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed + "m/s");
            Console.WriteLine("Precipitation amount next hour: " + weatherJsonData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount + "mm");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
    public static async Task<string> FetchWeatherData(string url)
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/Baltazzzzar/Exam-Unit-4)");
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

