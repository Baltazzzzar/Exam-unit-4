using Utils;

namespace APIData
{
    public class APIRequestFunctions
    {
        static readonly HttpClient client = new HttpClient();
        public static string url = "";

        public static async Task<string> SendDataRequest(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/Baltazzzzar/Exam-Unit-4)");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> WriteAPIWeatherData(string url)
        {
            try
            {
                string responseBody = "";
                responseBody = await SendDataRequest(url);
                File.WriteAllText("weatherdatalogs/apirawweatherdata.json", responseBody);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Output.Write(Output.Bold(Output.Color($"An error occurred: {e.Message}", ANSICodes.Colors.Red)));
                Output.Write(Output.Reset(""));
                Console.ReadLine();
                return null;
            }
        }
    }
}