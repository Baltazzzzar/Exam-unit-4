using System.Text.Json;
using WeatherDataLogging;
using Utils;
using CoordinatesDataClasses;
using APIData;
using Menu;


namespace HelperFunctions
{
    public class HelpingFunctions
    {
        public static string ConvertTimeFormat(string inputTime)
        {
            DateTime dt = DateTime.Parse(inputTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return dt.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static int FindAmountOfLogEntries(int amountOfDays, string filePath)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                return 0;
            }
            string json = File.ReadAllText(filePath);
            var allDetails = JsonSerializer.Deserialize<List<WeatherDataLog>>(json);
            int amountOfLogEntries = 0;
            for (int i = 0; i < allDetails.Count; i++)
            {
                if (DateTime.Parse(allDetails[i].time) > DateTime.Now.AddDays(-amountOfDays))
                {
                    amountOfLogEntries++;
                }
            }
            return amountOfLogEntries;
        }

        public static double GetHourDifference(string time1, string time2)
        {
            DateTime dt1 = DateTime.Parse(time1);
            DateTime dt2 = DateTime.Parse(time2);
            return Math.Abs((dt1 - dt2).TotalHours);
        }
        public static double GetValidDouble(string prompt)
        {
            double result;
            Output.WriteInGray(Output.Reset(prompt));
            while (!double.TryParse(Console.ReadLine(), out result))
            {
                Console.Clear();
                Output.WriteInRed(Output.Reset("Invalid input. Please enter a valid number: "));
                Thread.Sleep(1500);
                Console.Clear();
                Output.WriteInGray(Output.Reset(prompt));
            }
            Console.Clear();
            return result;
        }
        public static void ExitProgram()
        {
            Console.Clear();
            Output.WriteInGreen(Output.Reset("Goodbye!"));
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
        public static void ProcessAndSaveWeatherData(CityCoordinates cityCoordinates, APIWeatherData aPIWeatherData)
        {
            WeatherDataLog.userWeatherDetails = ProcessData.GetUserWeatherData(cityCoordinates, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex);
            WeatherDataLog.aPIWeatherDetails = ProcessData.ProcessAPIWeatherData(aPIWeatherData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex, WeatherDataLog.userWeatherDetails.hourAdjustment);
            ProcessData.SaveWeatherData(WeatherDataLog.userWeatherDetails, WeatherDataLog.filePathUserData);
            ProcessData.SaveWeatherData(WeatherDataLog.aPIWeatherDetails, WeatherDataLog.filePathAPIData);
        }
        public static void ProcessAndPrintComparisonData(int entries, bool useDays = true)
        {
            int amountOfLogEntriesInDays = FindAmountOfLogEntries(entries, WeatherDataLog.filePathUserData);
            if (!useDays)
            {
                amountOfLogEntriesInDays = entries;
            }
            WeatherDataLog.comparisonData = ProcessData.CompareData(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData);
            PrintData.PrintComparisonData(WeatherDataLog.comparisonData, WeatherDataLog.countryIndex, WeatherDataLog.cityIndex, amountOfLogEntriesInDays);
        }
        public static void CalculateAndPrintAverageDeviation(int entries, bool useDays = true)
        {
            int amountOfLogEntriesInDays = FindAmountOfLogEntries(entries, WeatherDataLog.filePathUserData);
            if (!useDays)
            {
                amountOfLogEntriesInDays = entries;
            }
            WeatherDataLog.averageDeviation = ProcessData.CalculateAverageDeviation(WeatherDataLog.filePathUserData, WeatherDataLog.filePathAPIData, amountOfLogEntriesInDays);
            PrintData.PrintAverageDeviation(WeatherDataLog.averageDeviation, WeatherDataLog.cityIndex, WeatherDataLog.countryIndex);
        }
        public static int GetValidInt(string prompt)
        {
            int result;
            Console.Clear();
            Output.WriteInGray(Output.Reset(prompt));
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.Clear();
                Output.WriteInRed(Output.Reset("Invalid input. Please enter a valid number: "));
                Thread.Sleep(1500);
                Console.Clear();
                Output.WriteInGray(Output.Reset(prompt));
            }
            Console.Clear();
            return result;
        }
    }
}