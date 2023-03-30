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
            string input = Input ?? "";

            var completion = CallGpt(input).Result;

            UpdateHistory(input, completion.choices.FirstOrDefault()?.message?.content ?? "", completion.usage.prompt_tokens, completion.usage.completion_tokens, completion.usage.cost);
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

        public async Task<Completion> CallGpt(IEnumerable<Prompt> prompts, int maxTokens = 2000)
        {
            IEnumerable<SerializedPrompt> serializedPrompts = prompts.Select(x => new SerializedPrompt()
            {
                role = Enum.GetName(typeof(ChatRole), x.role).ToLower(),
                content = x.content
            });

            // Use GPT3 for generation
            var body = new
            {
                //prompt = "user input goes here",
                //model = "text-davinci-003",
                messages = serializedPrompts.ToArray(),
                model = "gpt-3.5-turbo",
                max_tokens = maxTokens
            };

            var content = JsonConvert.SerializeObject(body);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            httpRequest.Headers.Add("Authorization", $"Bearer {await GetOpenAIKey()}");
            httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 200);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.SendAsync(httpRequest);
            var resultString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GPT error - {resultString}");
            }

            Completion completion = JsonConvert.DeserializeObject<Completion>(resultString);//, new ChoiceConverter());

            if (completion != null && completion.usage != null)
            {
                completion.usage.cost = (completion.usage.total_tokens * 0.002M / 1000);
            }

            while (completion.choices?.FirstOrDefault()?.message?.content?.StartsWith("\n") == true || completion.choices?.FirstOrDefault()?.message?.content?.StartsWith(" ") == true)
            {
                completion.choices[0].message.content = completion.choices[0].message.content.Substring(1);
            }

            return completion;
        }

        public async Task<Completion> CallGpt(string userInput, int maxTokens = 2000)
        {
            return await CallGpt(new List<Prompt>() { new Prompt {
                role = ChatRole.User,
                content = userInput
            } }, maxTokens);
        }
    }

    public enum ChatRole
    {
        System,
        Assistant,
        User,
    }

    public class SerializedPrompt
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Prompt
    {
        public ChatRole role { get; set; }
        public string content { get; set; }
    }

    public class Completion
    {
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Choice
    {
        public SerializedPrompt message { get; set; }
        public int index { get; set; }
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
        public decimal cost { get; set; }
    }
}
