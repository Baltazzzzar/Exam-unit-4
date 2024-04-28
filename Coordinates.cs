namespace CoordinatesDataClasses
{
    public class Coordinates
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
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

    public class CityCoordinates
    {
        public List<Countries> CountryCityCoordinates = new List<Countries>
        {
            new Countries
            {
                country = "Norway",
                cities = new List<Cities>
                {
                    new Cities { city = "Oslo", Coordinates = new Coordinates { latitude = 59.91, longitude = 10.75 } },
                    new Cities { city = "Bergen", Coordinates = new Coordinates { latitude = 60.39, longitude = 5.32 } },
                    new Cities { city = "Stavanger", Coordinates = new Coordinates { latitude = 58.96, longitude = 5.73 } },
                    new Cities { city = "Tromsø", Coordinates = new Coordinates { latitude = 69.64, longitude = 18.95 } },
                    new Cities { city = "Trondheim", Coordinates = new Coordinates { latitude = 63.43, longitude = 10.39 } },
                    new Cities { city = "Grimstad", Coordinates = new Coordinates { latitude = 58.34, longitude = 8.59 } },
                    new Cities { city = "Kristiansand", Coordinates = new Coordinates { latitude = 58.16, longitude = 8.00 } },
                    new Cities { city = "Bodø", Coordinates = new Coordinates { latitude = 67.28, longitude = 14.40 } }
                }
            },
            new Countries
            {
                country = "Sweden",
                cities = new List<Cities>
                {
                    new Cities { city = "Stockholm", Coordinates = new Coordinates { latitude = 59.32, longitude = 18.06 } },
                    new Cities { city = "Gothenburg", Coordinates = new Coordinates { latitude = 57.70, longitude = 11.97 } },
                    new Cities { city = "Malmö", Coordinates = new Coordinates { latitude = 55.60, longitude = 13.00 } },
                    new Cities { city = "Luleå", Coordinates = new Coordinates { latitude = 65.58, longitude = 22.15 } }
                }
            },
            new Countries
            {
                country = "Denmark",
                cities = new List<Cities>
                {
                    new Cities { city = "Copenhagen", Coordinates = new Coordinates { latitude = 55.67, longitude = 12.56 } },
                    new Cities { city = "Odense", Coordinates = new Coordinates { latitude = 55.39, longitude = 10.38 } },
                    new Cities { city = "Fredericia", Coordinates = new Coordinates { latitude = 55.56, longitude = 9.75 } },
                    new Cities { city = "Aalborg", Coordinates = new Coordinates { latitude = 57.04, longitude = 9.92 } },
                    new Cities { city = "Aarhus", Coordinates = new Coordinates { latitude = 56.16, longitude = 10.20 } }
                }
            },
            new Countries
            {
                country = "United Kingdom",
                cities = new List<Cities>
                {
                    new Cities { city = "London", Coordinates = new Coordinates { latitude = 51.50, longitude = -0.12 } },
                    new Cities { city = "Manchester", Coordinates = new Coordinates { latitude = 53.48, longitude = -2.24 } },
                    new Cities { city = "Birmingham", Coordinates = new Coordinates { latitude = 52.4862, longitude = -1.89 } },
                    new Cities { city = "Glasgow", Coordinates = new Coordinates { latitude = 55.86, longitude = -4.25 } },
                    new Cities { city = "Edinburgh", Coordinates = new Coordinates { latitude = 55.95, longitude = -3.18 } },
                    new Cities { city = "Belfast", Coordinates = new Coordinates { latitude = 54.60, longitude = -5.93 } },
                    new Cities { city = "Cardiff", Coordinates = new Coordinates { latitude = 51.48, longitude = -3.18 } }
                }
            },
            new Countries
            {
                country = "France",
                cities = new List<Cities>
                {
                    new Cities { city = "Paris", Coordinates = new Coordinates { latitude = 48.85, longitude = 2.35 } },
                    new Cities { city = "Marseille", Coordinates = new Coordinates { latitude = 43.29, longitude = 5.36 } },
                    new Cities { city = "Lyon", Coordinates = new Coordinates { latitude = 45.75, longitude = 4.85 } },
                    new Cities { city = "Toulouse", Coordinates = new Coordinates { latitude = 43.60, longitude = 1.44 } },
                    new Cities { city = "Nice", Coordinates = new Coordinates { latitude = 43.71, longitude = 7.26 } },
                    new Cities { city = "Montpellier", Coordinates = new Coordinates { latitude = 43.61, longitude = 3.87 } }
                }
            },
            new Countries
            {
                country = "Spain",
                cities = new List<Cities>
                {
                    new Cities { city = "Madrid", Coordinates = new Coordinates { latitude = 40.41, longitude = -3.70 } },
                    new Cities { city = "Barcelona", Coordinates = new Coordinates { latitude = 41.38, longitude = 2.17 } },
                    new Cities { city = "Valencia", Coordinates = new Coordinates { latitude = 39.46, longitude = -0.37 } },
                    new Cities { city = "Seville", Coordinates = new Coordinates { latitude = 37.38, longitude = -5.98 } },
                    new Cities { city = "Malaga", Coordinates = new Coordinates { latitude = 36.72, longitude = -4.42 } },
                    new Cities { city = "Bilbao", Coordinates = new Coordinates { latitude = 43.26, longitude = -2.93 } },
                    new Cities { city = "Alicante", Coordinates = new Coordinates { latitude = 38.35, longitude = -0.48 } }
                }
            },
            new Countries
            {
                country = "Italy",
                cities = new List<Cities>
                {
                    new Cities { city = "Rome", Coordinates = new Coordinates { latitude = 41.90, longitude = 12.49 } },
                    new Cities { city = "Milan", Coordinates = new Coordinates { latitude = 45.46, longitude = 9.19 } },
                    new Cities { city = "Naples", Coordinates = new Coordinates { latitude = 40.85, longitude = 14.26 } },
                    new Cities { city = "Turin", Coordinates = new Coordinates { latitude = 45.07, longitude = 7.68 } },
                    new Cities { city = "Palermo", Coordinates = new Coordinates { latitude = 38.11, longitude = 13.36 } },
                    new Cities { city = "Genoa", Coordinates = new Coordinates { latitude = 44.41, longitude = 8.93 } }
                }
            },
            new Countries
            {
                country = "Germany",
                cities = new List<Cities>
                {
                    new Cities { city = "Berlin", Coordinates = new Coordinates { latitude = 52.52, longitude = 13.40 } },
                    new Cities { city = "Hamburg", Coordinates = new Coordinates { latitude = 53.55, longitude = 9.99 } },
                    new Cities { city = "Munich", Coordinates = new Coordinates { latitude = 48.13, longitude = 11.58 } },
                    new Cities { city = "Cologne", Coordinates = new Coordinates { latitude = 50.93, longitude = 6.96 } },
                    new Cities { city = "Frankfurt", Coordinates = new Coordinates { latitude = 50.11, longitude = 8.68 } },
                    new Cities { city = "Stuttgart", Coordinates = new Coordinates { latitude = 48.78, longitude = 9.18 } }
                }
            },
            new Countries
            {
                country = "Netherlands",
                cities = new List<Cities>
                {
                    new Cities { city = "Amsterdam", Coordinates = new Coordinates { latitude = 52.37, longitude = 4.89 } },
                    new Cities { city = "Rotterdam", Coordinates = new Coordinates { latitude = 51.92, longitude = 4.48 } },
                    new Cities { city = "The Hague", Coordinates = new Coordinates { latitude = 52.08, longitude = 4.30 } },
                    new Cities { city = "Utrecht", Coordinates = new Coordinates { latitude = 52.09, longitude = 5.12 } }
                }
            },
            new Countries
            {
                country = "Russia",
                cities = new List<Cities>
                {
                    new Cities { city = "Moscow", Coordinates = new Coordinates { latitude = 55.75, longitude = 37.61 } },
                    new Cities { city = "Saint Petersburg", Coordinates = new Coordinates { latitude = 59.93, longitude = 30.33 } }
                }
            },
            new Countries
            {
                country = "United States",
                cities = new List<Cities>
                {
                    new Cities { city = "New York", Coordinates = new Coordinates { latitude = 40.71, longitude = -74.01 } },
                    new Cities { city = "Los Angeles", Coordinates = new Coordinates { latitude = 34.05, longitude = -118.24 } },
                    new Cities { city = "Chicago", Coordinates = new Coordinates { latitude = 41.88, longitude = -87.63 } },
                    new Cities { city = "Houston", Coordinates = new Coordinates { latitude = 29.76, longitude = -95.37 } },
                    new Cities { city = "Phoenix", Coordinates = new Coordinates { latitude = 33.45, longitude = -112.07 } }
                }
            },
            new Countries
            {
                country = "Brazil",
                cities = new List<Cities>
                {
                    new Cities { city = "São Paulo", Coordinates = new Coordinates { latitude = -23.55, longitude = -46.63 } },
                    new Cities { city = "Rio de Janeiro", Coordinates = new Coordinates { latitude = -22.91, longitude = -43.17 } },
                    new Cities { city = "Brasília", Coordinates = new Coordinates { latitude = -15.79, longitude = -47.88 } },
                    new Cities { city = "Salvador", Coordinates = new Coordinates { latitude = -12.97, longitude = -38.50 } }
                }
            },
        };
        public double GetLatitude(int countryIndex, int cityIndex)
        {
            if (countryIndex >= 0 && countryIndex < CountryCityCoordinates.Count)
            {
                var countryData = CountryCityCoordinates[countryIndex];
                if (cityIndex >= 0 && cityIndex < countryData.cities.Count)
                {
                    return countryData.cities[cityIndex].Coordinates.latitude;
                }
            }
            throw new ArgumentOutOfRangeException("Invalid country or city index");
        }
        public double GetLongitude(int countryIndex, int cityIndex)
        {
            if (countryIndex >= 0 && countryIndex < CountryCityCoordinates.Count)
            {
                var countryData = CountryCityCoordinates[countryIndex];
                if (cityIndex >= 0 && cityIndex < countryData.cities.Count)
                {
                    return countryData.cities[cityIndex].Coordinates.longitude;
                }
            }
            throw new ArgumentOutOfRangeException("Invalid country or city index");
        }
    }
}