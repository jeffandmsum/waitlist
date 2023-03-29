using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GptPlayground
{
    public class GPT
    {
        public static async Task<Completion> CallGpt(string input)
        {
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

            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 200);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.SendAsync(httpRequest);
            var resultString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GPT error - {resultString}");
            }

            Completion completion = JsonConvert.DeserializeObject<Completion>(resultString);

            if (completion != null && completion.usage != null)
            {
                completion.usage.cost = (completion.usage.total_tokens * 0.02M / 1000);
            }

            while (completion.choices?.FirstOrDefault()?.text?.StartsWith("\n") == true || completion.choices?.FirstOrDefault()?.text?.StartsWith(" ") == true)
            {
                completion.choices[0].text = completion.choices[0].text.Substring(1);
            }

            return completion;
        }

        private static async Task<string> GetOpenAIKey()
        {
            if (string.IsNullOrEmpty(Program.openAIKey))
            {
                // I and members of my dev team are all authorized readers of this key vault secrets, as is
                // the waitlist app in the cloud 
                var cred = new DefaultAzureCredential(true);
                var client = new SecretClient(new Uri("https://jeffandopenaikeys.vault.azure.net"), cred);
                var secret = await client.GetSecretAsync("OpenAI");
                Program.openAIKey = secret.Value.Value;
            }

            return Program.openAIKey;
        }
    }

    public class Completion
    {
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Choice
    {
        public string text { get; set; }
        public int index { get; set; }
        public string finish_reason { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
        public decimal cost { get; set; }
    }
}
