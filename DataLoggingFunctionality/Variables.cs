namespace WeatherDataLogging
{
    public class WeatherDataLog
    {
        public string? City { get; set; }
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public double CloudAreaFraction { get; set; }
        public double PrecipitationAmount { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public static WeatherDataLog? userWeatherDetails = null;
        public static WeatherDataLog? aPIWeatherDetails = null;
        public static WeatherDataLog[]? comparisonData = null;
        public static WeatherDataLog? averageDeviation = null;
        public static string filePathAPIData = "";
        public static string filePathUserData = "";
        public static int countryChoice;
        public static int cityChoice;
    }
}