namespace CoordinatesDataClasses
{
    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class Countries
    {
        public string country { get; set; }
        public List<Cities> cities { get; set; }
    }

    public class Cities
    {
        public string city { get; set; }
        public Coordinates Coordinates { get; set; }
    }

    public class CoordinatesDataClass
    {
        public List<Countries> CountryCityCoordinates = new List<Countries>
        {
            new Countries
            {
                country = "Norway",
                cities = new List<Cities>
                {
                    new Cities { city = "Oslo", Coordinates = new Coordinates { Latitude = 59.9139, Longitude = 10.7522 } },
                    new Cities { city = "Bergen", Coordinates = new Coordinates { Latitude = 60.3913, Longitude = 5.3221 } }
                }
            },
            new Countries
            {
                country = "Sweden",
                cities = new List<Cities>
                {
                    new Cities { city = "Stockholm", Coordinates = new Coordinates { Latitude = 59.3293, Longitude = 18.0686 } }
                }
            },
            new Countries
            {
                country = "Denmark",
                cities = new List<Cities>
                {
                    new Cities { city = "Copenhagen", Coordinates = new Coordinates { Latitude = 55.6761, Longitude = 12.5683 } }
                }
            }
        };
        public Coordinates GetCoordinates(int countryIndex, int cityIndex)
        {
            if (countryIndex >= 0 && countryIndex < CountryCityCoordinates.Count)
            {
                var countryData = CountryCityCoordinates[countryIndex];
                if (cityIndex >= 0 && cityIndex < countryData.cities.Count)
                {
                    return countryData.cities[cityIndex].Coordinates;
                }
            }
            Console.WriteLine("Invalid choice");
            Environment.Exit(0);
            return null;
        }
    }
}