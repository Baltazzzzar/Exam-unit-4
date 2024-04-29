using CoordinatesDataClasses;
using WeatherDataLogging;
using Utils;
using APIData;
using System.Text.Json;
using HelperFunctions;




namespace Menu
{
    enum TOC_INDEXES : int
    {
        CountryMenu,
        CityMenu,
        FunctionMenu,
        ForecastMenu,
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
                        SwapMenu(TOC_INDEXES.FunctionMenu);
                    });
                }
                itemsDescriptionList.Add(Output.WriteInYellow(Output.Reset("Back")));
                itemsActionList.Add(() => { SwapMenuWithSetIndex(TOC_INDEXES.CountryMenu, WeatherDataLog.countryIndex); });
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = itemsActionList
                };
            }
            else if (TOC_INDEXES.FunctionMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "View Weather Forecast", "Save A Weather Data Log Entry", "View Weather Log Entries", "Compare Weather Log Data", "See Average Deviation (Accuracy) Of API", Output.WriteInYellow(Output.Reset("Back")), Output.WriteInRed(Output.Reset("Exit")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ SwapMenu(TOC_INDEXES.ForecastMenu); },
                        ()=>{ HelpingFunctions.ProcessAndSaveWeatherData(cityCoordinates, APIWeatherData.aPIWeatherData);},
                        ()=>{ SwapMenu(TOC_INDEXES.ViewLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.CompareLogMenu); },
                        ()=>{ SwapMenu(TOC_INDEXES.AverageDeviationMenu); },
                        ()=>{ SwapMenuWithSetIndex(TOC_INDEXES.CityMenu, cityCoordinates.CountryCityCoordinates[WeatherDataLog.countryIndex].cities.Count); },
                        ()=>{ HelpingFunctions.ExitProgram();}
                    }
                };
            }
            else if (TOC_INDEXES.ForecastMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Present forecast", "Forecast in 6 hours", "Forecast in 12 hours", "Forecast Tomorrow", "Forecast in 2 days", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ PrintData.PrintWeatherForecast(APIWeatherData.aPIWeatherData,WeatherDataLog.countryIndex,WeatherDataLog.cityIndex,0);},
                        ()=>{ PrintData.PrintWeatherForecast(APIWeatherData.aPIWeatherData,WeatherDataLog.countryIndex,WeatherDataLog.cityIndex,5);},
                        ()=>{ PrintData.PrintWeatherForecast(APIWeatherData.aPIWeatherData,WeatherDataLog.countryIndex,WeatherDataLog.cityIndex,11);},
                        ()=>{ PrintData.PrintWeatherForecast(APIWeatherData.aPIWeatherData,WeatherDataLog.countryIndex,WeatherDataLog.cityIndex,23);},
                        ()=>{ PrintData.PrintWeatherForecast(APIWeatherData.aPIWeatherData,WeatherDataLog.countryIndex,WeatherDataLog.cityIndex,47);},
                        ()=>{ SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 0); },
                    }
                };
            }
            else if (TOC_INDEXES.ViewLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day Entries", "Last Week Entries", "Last Month Entries", "Choose Amount Of Days", "Choose Amount Of Log Entries", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 1); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2);},
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 7); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2);},
                        ()=>{ PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, 30); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of days worth of entries: ") ; PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, choice); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of entries: ") ; PrintData.PrintWeatherDataLog(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, choice, false); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2);},
                        ()=>{ SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 2); },
                    }
                };
            }
            else if (TOC_INDEXES.CompareLogMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day Entries", "Last Week Entries", "Last Month Entries", "Choose Amount Of Days", "Choose Amount Of Log Entries", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ HelpingFunctions.ProcessAndPrintComparisonData(1) ; SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3);},
                        ()=>{ HelpingFunctions.ProcessAndPrintComparisonData(7) ; SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3);},
                        ()=>{ HelpingFunctions.ProcessAndPrintComparisonData(30) ; SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of days worth of entries: ") ; HelpingFunctions.ProcessAndPrintComparisonData(choice) ; SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of entries: ") ; HelpingFunctions.ProcessAndPrintComparisonData(choice, false) ; SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3);},
                        ()=>{ SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 3); },
                    }
                };
            }
            else if (TOC_INDEXES.AverageDeviationMenu == menuIndex)
            {
                List<string> itemsDescriptionList = new List<string> { "Last Day Entries", "Last Week Entries", "Last Month Entries", "Choose Amount Of Days", "Choose Amount Of Log Entries", Output.WriteInYellow(Output.Reset("Back")) };
                output = new Menu()
                {
                    itemsDescription = itemsDescriptionList,
                    itemsAction = new List<Action> {
                        ()=>{ HelpingFunctions.CalculateAndPrintAverageDeviation(1); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4);},
                        ()=>{ HelpingFunctions.CalculateAndPrintAverageDeviation(7); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4);},
                        ()=>{ HelpingFunctions.CalculateAndPrintAverageDeviation(30); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of days worth of entries: ") ; HelpingFunctions.CalculateAndPrintAverageDeviation(choice); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4);},
                        ()=>{ int choice = HelpingFunctions.GetValidInt("Choose the amount of entries: ") ; HelpingFunctions.CalculateAndPrintAverageDeviation(choice, false); SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4);},
                        ()=>{ SwapMenuWithSetIndex(TOC_INDEXES.FunctionMenu, 4); },
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
                itemsActionList.Add(() => { HelpingFunctions.ExitProgram(); });
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
        void SwapMenuWithSetIndex(TOC_INDEXES newMenuIndex, int index)
        {
            currentMenu = GetMenuForMenuIndex(newMenuIndex);
            selectedItemInMenu = index;
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