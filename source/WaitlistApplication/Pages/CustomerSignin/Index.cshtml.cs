using System.Net.Http.Headers;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace WaitlistApplication.Pages.CustomerSignin
{
    public class IndexModel : PageModel
    {
        private static string openAIKey = string.Empty;
        private IHttpClientFactory clientFactory;
        private string todaysDetailedForecast = string.Empty;
        private string todaysTemp = string.Empty;
        private string todaysWind = string.Empty;
        private string todaysWindDirection = string.Empty;
        private int promptTokens = 0;
        private int completionTokens = 0;
        private int totalTokens = 0;
        private static Random random = new Random();

        public IndexModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public void OnGet()
        {
            GetWeatherServiceWeather().Wait();
            GetOpenAIWeather().Wait();
        }

        [BindProperty]
        public string WeatherReport
        {
            get
            {
                return todaysDetailedForecast;
            }
        }

        [BindProperty]
        public string UsageData
        {
            get
            {
                // Compute the cost at $0.02 per 1000 tokens
                decimal totalCost = totalTokens * 0.02M / 1000;

                return $"(Generating this page used {promptTokens} prompt tokens and {completionTokens} completion tokens, so it cost your instructor ${totalCost})";
            }
        }

        private async Task GetWeatherServiceWeather()
        {
            // TODO: Grab the user's location
            //string lat = "46.8790269";
            //string lon = "-96.7839108";
            //var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/points/{lat},{lon}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/gridpoints/FGF/100,57/forecast");
            httpRequest.Headers.Add("User-Agent", "WaitlistApplication/v1.0 (http://foo.bar.baz; foo@bar.baz)");
            var httpClient = clientFactory.CreateClient();
            httpClient.Timeout = new TimeSpan(0, 0, 200);
            var response = await httpClient.SendAsync(httpRequest);
            var resultString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Weather service error - {resultString}");
            }

            dynamic weatherObj = JsonConvert.DeserializeObject(resultString);

            todaysTemp = weatherObj.properties.periods[0].temperature;
            todaysWind = weatherObj.properties.periods[0].windSpeed;
            todaysWindDirection = weatherObj.properties.periods[0].windDirection;
            todaysDetailedForecast = weatherObj.properties.periods[0].detailedForecast;
        }

        private async Task<string> GetOpenAIKey()
        {
            if (string.IsNullOrEmpty(openAIKey))
            {
                // I and members of my dev team are all authorized readers of this key vault secrets, as is
                // the waitlist app in the cloud 
                var cred = new DefaultAzureCredential(true);
                var client = new SecretClient(new Uri("https://jeffandopenaikeys.vault.azure.net"), cred);
                var secret = await client.GetSecretAsync("OpenAI");
                openAIKey = secret.Value.Value;
            }

            return openAIKey;
        }

        private string BuildPromptString()
        {
            int selectedPrompt = random.Next(5);

            if (selectedPrompt == 0) return $"Pretend you are a teenage mutant ninja turtle giving a weather report for Moorhead, MN. Talk about how the weather impacts your ability to eat pizza. The current weather is {todaysDetailedForecast}";
            if (selectedPrompt == 1) return $"Pretend you are an over the top valley girl giving a weather report for Moorhead, MN. Use language like 'as if' and 'totes' as much as possible. The current weather is {todaysDetailedForecast}";
            if (selectedPrompt == 2) return $"Pretend you are a used car salesman giving a weather report for Moorhead, MN. Try to slip in sublte mentions that you have cars for sale, but tie those statements in with the weather report. The current weather is {todaysDetailedForecast}";
            if (selectedPrompt == 3) return $"Pretend you are a fraudulent scientist giving a weather report for Moorhead, MN. Use words that sound scientific, but use them incorrectly. The current weather is {todaysDetailedForecast}";
            if (selectedPrompt == 4) return $"Pretend you are a confused college student who just woke up from sleep giving a weather report for Moorhead, MN.Give the weather, but make it clear that you have no idea what is going on or why you are being asked.The current weather is {todaysDetailedForecast}";
           
            return todaysDetailedForecast;
        }

        private async Task GetOpenAIWeather()
        {
            // Use GPT3 for generation
            var body = new
            {
                prompt = BuildPromptString(),
                model = "text-davinci-003",
                max_tokens = 2000
            };

            var content = JsonConvert.SerializeObject(body);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");
            httpRequest.Headers.Add("Authorization", $"Bearer {await GetOpenAIKey()}");
            httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var httpClient = clientFactory.CreateClient();
            httpClient.Timeout = new TimeSpan(0, 0, 200);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.SendAsync(httpRequest);
            var resultString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GPT error - {resultString}");
            }

            dynamic aiResult = JsonConvert.DeserializeObject(resultString);

            todaysDetailedForecast = aiResult.choices[0].text;
            promptTokens = aiResult.usage.prompt_tokens;
            completionTokens = aiResult.usage.completion_tokens;
            totalTokens = aiResult.usage.total_tokens;
        }
    }
}
