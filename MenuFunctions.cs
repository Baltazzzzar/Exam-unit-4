using CoordinatesDataClasses;
using LoggingWeatherDataClass;
using Utils;
using APIWeatherDataClasses;
using System.Text.Json;



namespace Menu
{
    enum TOC_INDEXES : int
    {
        CountryMenu,
        CityMenu,
        FunctionMenu,
        ViewLogMenu,
        CompareLogMenu,
    }

    public class MenuScreen
    {
        Menu currentMenu;
        TOC_INDEXES currentTOC = TOC_INDEXES.CountryMenu;
        int selectedItemInMenu;
        int menuChange = 0;
        public static int countryChoice;
        public static int cityChoice;
        public static double latitude;
        public static double longitude;
        public static string url = "";
        public static string filePathAPIData = "";
        public static string filePathUserData = "";
        static readonly HttpClient client = new HttpClient();
        static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();
        public MenuScreen()
        {
            currentMenu = GetMenuForMenuIndex(currentTOC);
        }

        Menu GetMenuForMenuIndex(TOC_INDEXES menuIndex)
        {
            Menu output;
            if (TOC_INDEXES.CityMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string>();
                List<Action> itemsActionList = new List<Action>();
                foreach (var city in coordinatesDataClass.CountryCityCoordinates[countryChoice].cities)
                {
                    itemsDescriptionList.Add(city.city);
                    int currentCityIndex = itemsDescriptionList.Count - 1;
                    itemsActionList.Add(async () =>
                    {
                        cityChoice = currentCityIndex;
                        latitude = coordinatesDataClass.GetLatitude(countryChoice, cityChoice);
                        longitude = coordinatesDataClass.GetLongitude(countryChoice, cityChoice);
                        filePathAPIData = $"weatherdatalogs/{coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city.ToLower()}_apiweatherlog.json";
                        filePathUserData = $"weatherdatalogs/{coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city.ToLower()}_userweatherlog.json";
                        url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";
                        string responseBody = "";
                        try
                        {
                            responseBody = await WeatherDataLog.FetchWeatherData(url);
                            File.WriteAllText("weatherdatalogs/apirawweatherdata.json", responseBody);
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("\nException Caught!");
                            Console.WriteLine("Message :{0} ", e.Message);
                            Output.Write(Output.Bold(Output.Color($"An error occurred: {e.Message}", ANSICodes.Colors.Red)));
                            Output.Write(Output.Reset(""));
                            Console.ReadLine();
                        }
                        APIWeatherData.aPIWeatherData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);
                        WeatherDataLog.aPIWeatherDetails = new WeatherDataLog();
                        {
                            WeatherDataLog.aPIWeatherDetails.City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
                            WeatherDataLog.aPIWeatherDetails.Time = APIWeatherData.aPIWeatherData.properties.meta.updated_at;
                            WeatherDataLog.aPIWeatherDetails.Temperature = APIWeatherData.aPIWeatherData.properties.timeseries[0].data.instant.details.air_temperature;
                            WeatherDataLog.aPIWeatherDetails.CloudAreaFraction = APIWeatherData.aPIWeatherData.properties.timeseries[0].data.instant.details.cloud_area_fraction;
                            WeatherDataLog.aPIWeatherDetails.Humidity = APIWeatherData.aPIWeatherData.properties.timeseries[0].data.instant.details.relative_humidity;
                            WeatherDataLog.aPIWeatherDetails.WindSpeed = APIWeatherData.aPIWeatherData.properties.timeseries[0].data.instant.details.wind_speed;
                            WeatherDataLog.aPIWeatherDetails.PrecipitationAmount = APIWeatherData.aPIWeatherData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount;
                        }

                        SwapMenu(TOC_INDEXES.FunctionMenu);
                    });
                }
                itemsDescriptionList.Add("Back");
                itemsActionList.Add(() => { SwapMenu(TOC_INDEXES.CountryMenu); });
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = itemsActionList
                };
            }
            else if (TOC_INDEXES.FunctionMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "View Current Day Weather Data", "Save A Weather Data Log Entry", "Print Weather Log Entries", "Compare Weather Log Data", "Back" };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ WeatherDataLog.PrintWeatherReport(APIWeatherData.aPIWeatherData, countryChoice, cityChoice); },
                        ()=>{ WeatherDataLog.userWeatherDetails = WeatherDataLog.GetUserWeatherData(coordinatesDataClass, countryChoice, cityChoice);
                            WeatherDataLog.SaveWeatherData(WeatherDataLog.userWeatherDetails, filePathUserData);
                            WeatherDataLog.SaveWeatherData(WeatherDataLog.aPIWeatherDetails, filePathAPIData);},
                        ()=>{ SwapMenu(TOC_INDEXES.ViewLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.CompareLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.CityMenu); },
                    }
                };
            }
            else if (TOC_INDEXES.ViewLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day", "Last Week", "Last Month", "Back" };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ WeatherDataLog.PrintWeatherDataLog(filePathUserData, filePathAPIData, 1); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.PrintWeatherDataLog(filePathUserData, filePathAPIData, 7); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.PrintWeatherDataLog(filePathUserData, filePathAPIData, 30); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ SwapMenu(TOC_INDEXES.FunctionMenu); },
                    }
                };
            }
            else if (TOC_INDEXES.CompareLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day", "Last Week", "Last Month", "Back" };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ WeatherDataLog.comparisonData = WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 1); WeatherDataLog.PrintComparisonData(WeatherDataLog.comparisonData) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.comparisonData = WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 7); WeatherDataLog.PrintComparisonData(WeatherDataLog.comparisonData) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.comparisonData = WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 30); WeatherDataLog.PrintComparisonData(WeatherDataLog.comparisonData) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ SwapMenu(TOC_INDEXES.FunctionMenu); },
                    }
                };
            }
            else
            {
                List<string> itemsDescriptionList = new List<string>();
                List<Action> itemsActionList = new List<Action>();
                foreach (var country in coordinatesDataClass.CountryCityCoordinates)
                {
                    itemsDescriptionList.Add(country.country);
                    int currentCountryIndex = itemsDescriptionList.Count - 1;
                    itemsActionList.Add(() =>
                    {
                        countryChoice = currentCountryIndex;
                        SwapMenu(TOC_INDEXES.CityMenu);
                    });
                }
                itemsDescriptionList.Add("Exit");
                itemsActionList.Add(() =>
                {
                    Console.Clear();
                    Output.WriteInGreen(Output.Reset("Goodbye!"));
                    Environment.Exit(0);
                });
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = itemsActionList
                };
            }
            return output;
        }
        void SwapMenu(TOC_INDEXES newMenuIndex)
        {
            currentMenu = GetMenuForMenuIndex(newMenuIndex);
            selectedItemInMenu = 0;
        }

        void OnMenuAction(int item)
        {
            if (item < currentMenu.Length)
            {
                currentMenu[item].action();
            }
        }
        public void Input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey keyCode = Console.ReadKey(true).Key;
                if (keyCode == ConsoleKey.DownArrow)
                {
                    if (selectedItemInMenu < currentMenu.Length - 1)
                    {
                        menuChange = 1;
                    }
                    else
                    {
                        menuChange = -selectedItemInMenu;
                    }
                }
                else if (keyCode == ConsoleKey.UpArrow)
                {
                    if (selectedItemInMenu > 0)
                    {
                        menuChange = -1;
                    }
                    else
                    {
                        menuChange = currentMenu.Length - 1;
                    }
                }
                else if (keyCode == ConsoleKey.Enter)
                {
                    OnMenuAction(selectedItemInMenu);
                }
            }
        }
        public void Update()
        {
            selectedItemInMenu += menuChange;
            selectedItemInMenu = Math.Clamp(selectedItemInMenu, 0, currentMenu.Length - 1);
            menuChange = 0;
        }
        public void Draw()
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("");

            for (int index = 0; index < currentMenu.Length; index++)
            {
                if (index == selectedItemInMenu)
                {
                    PrintActiveMenuItem($"-> {currentMenu[index].description}");
                }
                else
                {
                    PrintMenuItem($"   {currentMenu[index].description}");
                }
            }
        }
        static void PrintActiveMenuItem(string item)
        {
            Output.WriteInGreen(Output.Reset(Output.Bold(Output.Align(item, Alignment.LEFT))), newLine: true);
        }
        static void PrintMenuItem(string item)
        {
            Output.Write(Output.Reset(Output.Align(item, Alignment.LEFT)), newLine: true);
        }
    }
    class Menu
    {

        public List<string> itemsDescription { get; set; }
        public List<Action> itemsAction { get; set; }

        public int Length { get { return this.itemsDescription.Count; } }

        public (string description, Action action) this[int index]
        {
            get
            {
                if (index >= 0 && index < itemsDescription.Count && index < itemsAction.Count)
                {
                    return (description: itemsDescription[index], action: itemsAction[index]);
                }
                else
                {
                    return (description: "Invalid index", action: () => Console.WriteLine("Invalid index"));
                }
            }
        }

    }
}