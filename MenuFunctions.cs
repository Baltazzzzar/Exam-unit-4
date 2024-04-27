using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoordinatesDataClasses;
using Program;
using LoggingWeatherDataClass;
using Utils;
using APIWeatherDataClasses;
using System.Text.Json;



namespace Menu
{
    using MenuItem = System.ValueTuple<string, Action>;
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
        public static int amountOfLogEntries = 0;
        public static double latitude;
        public static double longitude;
        public static string url = "";
        APIWeatherData aPIWeatherData = null;
        public static WeatherDataLog userWeatherDetails = null;
        public static WeatherDataLog aPIWeatherDetails = null;
        public static string filePathAPIData = "";
        public static string filePathUserData = "";
        static readonly HttpClient client = new HttpClient();
        static CoordinatesDataClass coordinatesDataClass = new CoordinatesDataClass();
        Output output = new Output();
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
                        aPIWeatherData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);
                        aPIWeatherDetails = new WeatherDataLog();
                        {
                            aPIWeatherDetails.City = coordinatesDataClass.CountryCityCoordinates[countryChoice].cities[cityChoice].city;
                            aPIWeatherDetails.Time = aPIWeatherData.properties.meta.updated_at;
                            aPIWeatherDetails.Temperature = aPIWeatherData.properties.timeseries[0].data.instant.details.air_temperature;
                            aPIWeatherDetails.CloudAreaFraction = aPIWeatherData.properties.timeseries[0].data.instant.details.cloud_area_fraction;
                            aPIWeatherDetails.Humidity = aPIWeatherData.properties.timeseries[0].data.instant.details.relative_humidity;
                            aPIWeatherDetails.WindSpeed = aPIWeatherData.properties.timeseries[0].data.instant.details.wind_speed;
                            aPIWeatherDetails.PrecipitationAmount = aPIWeatherData.properties.timeseries[0].data.next_1_hours.details.precipitation_amount;
                        }

                        SwapMenu(TOC_INDEXES.FunctionMenu);
                    });
                }
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
                        ()=>{ WeatherDataLog.PrintWeatherData(aPIWeatherData, countryChoice, cityChoice); },
                        ()=>{ userWeatherDetails = WeatherDataLog.GetUserWeatherData(coordinatesDataClass, countryChoice, cityChoice); WeatherDataLog.SaveWeatherData(userWeatherDetails, filePathUserData);WeatherDataLog.SaveWeatherData(aPIWeatherDetails, filePathAPIData);},
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
                        ()=>{ WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 1); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 7); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.CompareData(filePathUserData, filePathAPIData, 30); SwapMenu(TOC_INDEXES.FunctionMenu);},
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
            if (!Enum.IsDefined(typeof(TOC_INDEXES), newMenuIndex))
            {
                return;
            }
            currentMenu = GetMenuForMenuIndex(newMenuIndex);
            selectedItemInMenu = 0;
        }

        void OnMenuAction(int item)
        {
            if (item < 0 || item >= currentMenu.itemsAction.Count)
            {
                return;
            }
            currentMenu.itemsAction[item]();
        }
        public void input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey keyCode = Console.ReadKey(true).Key;
                if (keyCode == ConsoleKey.DownArrow)
                {
                    menuChange = 1;
                }
                else if (keyCode == ConsoleKey.UpArrow)
                {
                    menuChange = -1;
                }
                else if (keyCode == ConsoleKey.Enter)
                {
                    OnMenuAction(selectedItemInMenu);
                }
            }
        }
        public void update()
        {
            selectedItemInMenu += menuChange;
            selectedItemInMenu = Math.Clamp(selectedItemInMenu, 0, currentMenu.Length - 1);
            menuChange = 0;
        }
        public void draw()
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("");

            for (int index = 0; index < currentMenu.Length; index++)
            {
                if (index == selectedItemInMenu)
                {
                    printActiveMenuItem($"* {currentMenu[index].description} *");
                }
                else
                {
                    printMenuItem($"  {currentMenu[index].description}  ");
                }
            }
        }
        void printActiveMenuItem(string item)
        {
            Output.Write(Output.Reset(Output.Bold(Output.Align(Output.Color(item, ANSICodes.Colors.Green), Alignment.LEFT))), newLine: true);
        }
        void printMenuItem(string item)
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