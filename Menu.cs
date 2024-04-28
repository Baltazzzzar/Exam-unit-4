using CoordinatesDataClasses;
using WeatherDataLogging;
using Utils;
using APIData;
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
        AverageDeviationMenu
    }

    public class MenuScreen
    {
        Menu currentMenu;
        TOC_INDEXES currentTOC = TOC_INDEXES.CountryMenu;
        int selectedItemInMenu;
        int menuChange = 0;
        static CityCoordinates cityCoordinates = new CityCoordinates();
        static Coordinates coordinates = new Coordinates();
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
                foreach (var city in cityCoordinates.CountryCityCoordinates[WeatherDataLog.countryIndex].cities)
                {
                    itemsDescriptionList.Add(city.city);
                    int currentCityIndex = itemsDescriptionList.Count - 1;
                    itemsActionList.Add(async () =>
                    {
                        WeatherDataLog.cityIndex = currentCityIndex;
                        coordinates.latitude = cityCoordinates.GetLatitude(WeatherDataLog.countryIndex, WeatherDataLog.cityIndex);
                        coordinates.longitude = cityCoordinates.GetLongitude(WeatherDataLog.countryIndex, WeatherDataLog.cityIndex);
                        WeatherDataLog.filePathAPIData = $"weatherdatalogs/{cityCoordinates.CountryCityCoordinates[WeatherDataLog.countryIndex].cities[WeatherDataLog.cityIndex].city.ToLower()}_apiweatherlog.json";
                        WeatherDataLog.filePathUserData = $"weatherdatalogs/{cityCoordinates.CountryCityCoordinates[WeatherDataLog.countryIndex].cities[WeatherDataLog.cityIndex].city.ToLower()}_userweatherlog.json";
                        APIRequestFunctions.url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.latitude}&lon={coordinates.longitude}";
                        string responseBody = await APIRequestFunctions.WriteAPIWeatherData(APIRequestFunctions.url);
                        if (responseBody == null)
                        {
                            SwapMenu(TOC_INDEXES.CountryMenu);
                            return;
                        }
                        APIWeatherData.aPIWeatherData = JsonSerializer.Deserialize<APIWeatherData>(responseBody);
                        WeatherDataLog.aPIWeatherDetails = ProcessData.ProcessAPIWeatherData(APIWeatherData.aPIWeatherData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex);
                        SwapMenu(TOC_INDEXES.FunctionMenu);
                    });
                }
                itemsDescriptionList.Add(Output.WriteInYellow(Output.Reset("Back")));
                itemsActionList.Add(() => { SwapMenu(TOC_INDEXES.CountryMenu); });
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = itemsActionList
                };
            }
            else if (TOC_INDEXES.FunctionMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "View Current Day Weather Data", "Save A Weather Data Log Entry", "Print Weather Log Entries", "Compare Weather Log Data", "See Average Deviation (Accuracy) Of API", Output.WriteInYellow(Output.Reset("Back")), Output.WriteInRed(Output.Reset("Exit")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ PrintData.PrintWeatherReport(APIWeatherData.aPIWeatherData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex); },
                        ()=>{ WeatherDataLog.userWeatherDetails = ProcessData.GetUserWeatherData(cityCoordinates, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex);
                            ProcessData.SaveWeatherData(WeatherDataLog.userWeatherDetails, WeatherDataLog.filePathUserData);
                            ProcessData.SaveWeatherData(WeatherDataLog.aPIWeatherDetails, WeatherDataLog.filePathAPIData);},
                        ()=>{ SwapMenu(TOC_INDEXES.ViewLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.CompareLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.AverageDeviationMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.CityMenu); },
                        ()=>{ Console.Clear(); Output.WriteInGreen(Output.Reset("Goodbye!")); Environment.Exit(0);}
                    }
                };
            }
            else if (TOC_INDEXES.ViewLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day", "Last Week", "Last Month", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 1); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 7); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 30); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ SwapMenu(TOC_INDEXES.FunctionMenu); },
                    }
                };
            }
            else if (TOC_INDEXES.CompareLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day", "Last Week", "Last Month", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ WeatherDataLog.comparisonData = ProcessData.CompareData(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 1); PrintData.PrintComparisonData(WeatherDataLog.comparisonData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.comparisonData = ProcessData.CompareData(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 7); PrintData.PrintComparisonData(WeatherDataLog.comparisonData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.comparisonData = ProcessData.CompareData(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 30); PrintData.PrintComparisonData(WeatherDataLog.comparisonData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex) ; SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ SwapMenu(TOC_INDEXES.FunctionMenu); },
                    }
                };
            }
            else if (TOC_INDEXES.AverageDeviationMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day", "Last Week", "Last Month", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ WeatherDataLog.averageDeviation = ProcessData.CalculateAverageDeviation(WeatherDataLog.filePathUserData,WeatherDataLog.filePathAPIData,1); PrintData.PrintAverageDeviation(WeatherDataLog.averageDeviation, WeatherDataLog.cityIndex, WeatherDataLog.countryIndex); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.averageDeviation = ProcessData.CalculateAverageDeviation(WeatherDataLog.filePathUserData,WeatherDataLog.filePathAPIData,7); PrintData.PrintAverageDeviation(WeatherDataLog.averageDeviation, WeatherDataLog.cityIndex, WeatherDataLog.countryIndex); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ WeatherDataLog.averageDeviation = ProcessData.CalculateAverageDeviation(WeatherDataLog.filePathUserData,WeatherDataLog.filePathAPIData,30); PrintData.PrintAverageDeviation(WeatherDataLog.averageDeviation, WeatherDataLog.cityIndex, WeatherDataLog.countryIndex); SwapMenu(TOC_INDEXES.FunctionMenu);},
                        ()=>{ SwapMenu(TOC_INDEXES.FunctionMenu); },
                    }
                };
            }
            else
            {
                List<string> itemsDescriptionList = new List<string>();
                List<Action> itemsActionList = new List<Action>();
                foreach (var country in cityCoordinates.CountryCityCoordinates)
                {
                    itemsDescriptionList.Add(country.country);
                    int currentCountryIndex = itemsDescriptionList.Count - 1;
                    itemsActionList.Add(() => { WeatherDataLog.countryIndex = currentCountryIndex; SwapMenu(TOC_INDEXES.CityMenu); });
                }
                itemsDescriptionList.Add(Output.WriteInRed(Output.Reset("Exit")));
                itemsActionList.Add(() => { Console.Clear(); Output.WriteInGreen(Output.Reset("Goodbye!")); Environment.Exit(0); });
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