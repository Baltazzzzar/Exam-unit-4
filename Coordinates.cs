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
                    new Cities { city = "Bergen", Coordinates = new Coordinates { Latitude = 60.3913, Longitude = 5.3221 } },
                    new Cities { city = "Stavanger", Coordinates = new Coordinates { Latitude = 58.9690, Longitude = 5.7331 } },
                    new Cities { city = "Tromsø", Coordinates = new Coordinates { Latitude = 69.6496, Longitude = 18.9560 } },
                    new Cities { city = "Trondheim", Coordinates = new Coordinates { Latitude = 63.4305, Longitude = 10.3951 } },
                    new Cities { city = "Grimstad", Coordinates = new Coordinates { Latitude = 58.34, Longitude = 8.59 } }

                }
            },
            new Countries
            {
                country = "Sweden",
                cities = new List<Cities>
                {
                    new Cities { city = "Stockholm", Coordinates = new Coordinates { Latitude = 59.3293, Longitude = 18.0686 } },
                    new Cities { city = "Gothenburg", Coordinates = new Coordinates { Latitude = 57.7089, Longitude = 11.9746 } },
                    new Cities { city = "Malmö", Coordinates = new Coordinates { Latitude = 55.6049, Longitude = 13.0038 } },
                    new Cities { city = "Luleå", Coordinates = new Coordinates { Latitude = 65.5848, Longitude = 22.1567 } }

                }
            },
            new Countries
            {
                country = "Denmark",
                cities = new List<Cities>
                {
                    new Cities { city = "Copenhagen", Coordinates = new Coordinates { Latitude = 55.6761, Longitude = 12.5683 } },
                    new Cities { city = "Odense", Coordinates = new Coordinates { Latitude = 55.3966, Longitude = 10.3886 } },
                    new Cities { city = "Fredericia", Coordinates = new Coordinates { Latitude = 55.5650, Longitude = 9.7525 } },
                    new Cities { city = "Aalborg", Coordinates = new Coordinates { Latitude = 57.0488, Longitude = 9.9217 } },
                    new Cities { city = "Aarhus", Coordinates = new Coordinates { Latitude = 56.1629, Longitude = 10.2039 } }

                }
            },
            new Countries
            {
                country = "United Kingdom",
                cities = new List<Cities>
                {
                    new Cities { city = "London", Coordinates = new Coordinates { Latitude = 51.5074, Longitude = -0.1278 } },
                    new Cities { city = "Manchester", Coordinates = new Coordinates { Latitude = 53.4808, Longitude = -2.2426 } },
                    new Cities { city = "Birmingham", Coordinates = new Coordinates { Latitude = 52.4862, Longitude = -1.8904 } },
                    new Cities { city = "Glasgow", Coordinates = new Coordinates { Latitude = 55.8642, Longitude = -4.2518 } },
                    new Cities { city = "Edinburgh", Coordinates = new Coordinates { Latitude = 55.9533, Longitude = -3.1883 } }

                }
            },
            new Countries
            {
                country = "France",
                cities = new List<Cities>
                {
                    new Cities { city = "Paris", Coordinates = new Coordinates { Latitude = 48.8566, Longitude = 2.3522 } },
                    new Cities { city = "Marseille", Coordinates = new Coordinates { Latitude = 43.2965, Longitude = 5.3698 } },
                    new Cities { city = "Lyon", Coordinates = new Coordinates { Latitude = 45.75, Longitude = 4.85 } },
                    new Cities { city = "Toulouse", Coordinates = new Coordinates { Latitude = 43.6045, Longitude = 1.4442 } },
                    new Cities { city = "Nice", Coordinates = new Coordinates { Latitude = 43.7102, Longitude = 7.2620 } }
                }
            },
            new Countries
            {
                country = "Spain",
                cities = new List<Cities>
                {
                    new Cities { city = "Madrid", Coordinates = new Coordinates { Latitude = 40.4168, Longitude = -3.7038 } },
                    new Cities { city = "Barcelona", Coordinates = new Coordinates { Latitude = 41.3851, Longitude = 2.1734 } },
                    new Cities { city = "Valencia", Coordinates = new Coordinates { Latitude = 39.4699, Longitude = -0.3763 } },
                    new Cities { city = "Seville", Coordinates = new Coordinates { Latitude = 37.3886, Longitude = -5.9826 } }
                }
            },
            new Countries
            {
                country = "Italy",
                cities = new List<Cities>
                {
                    new Cities { city = "Rome", Coordinates = new Coordinates { Latitude = 41.9028, Longitude = 12.4964 } },
                    new Cities { city = "Milan", Coordinates = new Coordinates { Latitude = 45.4642, Longitude = 9.1900 } },
                    new Cities { city = "Naples", Coordinates = new Coordinates { Latitude = 40.8518, Longitude = 14.2681 } },
                    new Cities { city = "Turin", Coordinates = new Coordinates { Latitude = 45.0703, Longitude = 7.6869 } }
                }
            },
            new Countries
            {
                country = "Germany",
                cities = new List<Cities>
                {
                    new Cities { city = "Berlin", Coordinates = new Coordinates { Latitude = 52.5200, Longitude = 13.4050 } },
                    new Cities { city = "Hamburg", Coordinates = new Coordinates { Latitude = 53.5511, Longitude = 9.9937 } },
                    new Cities { city = "Munich", Coordinates = new Coordinates { Latitude = 48.1351, Longitude = 11.5820 } },
                    new Cities { city = "Cologne", Coordinates = new Coordinates { Latitude = 50.9375, Longitude = 6.9603 } }

                }
            },
            new Countries
            {
                country = "Russia",
                cities = new List<Cities>
                {
                    new Cities { city = "Moscow", Coordinates = new Coordinates { Latitude = 55.7558, Longitude = 37.6176 } },
                    new Cities { city = "Saint Petersburg", Coordinates = new Coordinates { Latitude = 59.9343, Longitude = 30.3351 } }
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