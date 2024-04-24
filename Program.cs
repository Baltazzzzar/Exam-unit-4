using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherData
{
    public int air_temperature { get; set; }
    public double cloud_area_fraction { get; set; }
    public double relative_humidity { get; set; }
    public string wind_from_direction { get; set; }
    public double wind_speed { get; set; }
}

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
            Console.WriteLine(responseBody);
            File.WriteAllText("weatherdata.json", responseBody);
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