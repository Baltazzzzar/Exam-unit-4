namespace WeatherDataLogging
{
    public class WeatherDataLog
    {
        public string? city { get; set; }
        public string? time { get; set; }
        public double temperature { get; set; }
        public double cloudAreaFraction { get; set; }
        public double precipitationAmount { get; set; }
        public double precipitationAmountNextHour { get; set; }
        public double precipitationAmountNext6Hours { get; set; }
        public double humidity { get; set; }
        public double windSpeed { get; set; }
        public static WeatherDataLog? userWeatherDetails = null;
        public static WeatherDataLog? aPIPresentWeatherDetails = null;
        public static WeatherDataLog[]? comparisonData = null;
        public static WeatherDataLog? averageDeviation = null;
        public static string filePathAPIData = "";
        public static string filePathUserData = "";
        public static int countryIndex;
        public static int cityIndex;
    }
}