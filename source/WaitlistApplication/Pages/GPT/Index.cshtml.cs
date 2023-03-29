using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WaitlistApplication.Pages.GPT
{
    public class IndexModel : PageModel
    {
        private static string openAIKey = string.Empty;
        private IHttpClientFactory clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [BindProperty]
        public string Input { get; set; } = "";

        [BindProperty]
        public string HtmlHistory { get; set; } = "";

        public void OnGet()
        {
            TempData["HtmlHistory"] = null;
        }

        public void OnPost()
        {
            CallGpt().Wait();
        }

        private void UpdateHistory(string input, string response, int promptTokens, int completionTokens, decimal cost)
        {
            while (response.StartsWith("\n") || response.StartsWith(" "))
            {
                response = response.Substring(1);
            }

            HtmlHistory = TempData["HtmlHistory"] as string ?? "";

            HtmlHistory = $"<DIV class=\"history-item\"><DIV class=\"userinputtitle\">Prompt</DIV><DIV class=\"userinputvalue\">{input}</DIV></DIV><DIV><DIV class=\"machineoutputtitle\">Completion</DIV><DIV class=\"machineoutputvalue\">{response}</DIV><DIV class=\"completion-details\"><DIV class=\"completion-prompttokens\">Prompt Tokens: {promptTokens}</DIV><DIV class=\"completion-responsetokens\">Response Tokens: {completionTokens}</DIV><DIV class=\"completion-cost\">Total Cost: {cost.ToString("C5")}</DIV></DIV></DIV>" + HtmlHistory;

            TempData["HtmlHistory"] = HtmlHistory;
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

        private async Task CallGpt()
        {
            string input = Input;

            // Use GPT3 for generation
            var body = new
            {
                prompt = input,
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

            int promptTokens = aiResult.usage.prompt_tokens;
            int completionTokens = aiResult.usage.completion_tokens;
            int totalTokens = aiResult.usage.total_tokens;
            decimal cost = totalTokens * 0.02M / 1000;
            string completion = aiResult.choices[0].text;

            UpdateHistory(input, completion, promptTokens, completionTokens, cost);
        }
    }
}
