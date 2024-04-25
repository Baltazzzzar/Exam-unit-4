using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherDataClasses;




class Program
{
    static HttpClient client = new HttpClient();
    public static async Task Main()
    {
        double latitudeGrimstad = 58.20;
        double longitudeGrimstad = 8.35;
        string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitudeGrimstad}&lon={longitudeGrimstad}";
        try
        {
            string responseBody = await FetchWeatherData(url);
            File.WriteAllText("weatherdata.json", responseBody);
            WeatherData weatherJsonData = JsonSerializer.Deserialize<WeatherData>(responseBody);

            Console.WriteLine("Weather data for Grimstad");
            Console.WriteLine("Last updated: " + weatherJsonData.properties.meta.updated_at);
            Console.WriteLine("Temperature: " + weatherJsonData.properties.timeseries[0].data.instant.details.air_temperature + "°C");
            Console.WriteLine("Cloud area fraction: " + weatherJsonData.properties.timeseries[0].data.instant.details.cloud_area_fraction + "%");
            Console.WriteLine("Relative humidity: " + weatherJsonData.properties.timeseries[0].data.instant.details.relative_humidity + "%");
            Console.WriteLine("Wind speed: " + weatherJsonData.properties.timeseries[0].data.instant.details.wind_speed + "m/s");


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

