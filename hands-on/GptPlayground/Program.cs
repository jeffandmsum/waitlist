using GptPlayground;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

internal class Program
{
    // TODO: Replace string.Empty with the key provided in class
    internal static string openAIKey = string.Empty;

    private static void Main(string[] args)
    {
        List<ECCN> eccns = ReadEccnData();
        List<Product> products = ReadProductData();

        // Try stuff interactively
        //ProcessUserInput();

        // Process across a set of four sample products
        //ProcessProducts(products, eccns);
        
        // See how good it does against the MGCR3, which the manufacturer classifies as 7A994
        ProcessMGCR3(eccns);
    }

    private static void ProcessMGCR3(List<ECCN> eccns)
    {
        ProcessProducts(new List<Product>() { new Product
        {
            Name = "MGC-R3 Gyro Compass and ISN",
            Manufacturer = "Kongsberg",
            Description = @"The MGC R3 product is a fully inertial navigation system 
(INS). It can output heading, roll, pitch, heave and 
position. Acceleration and velocity of linear motions, as 
well as angular rates, are output from the unit. The MGC 
R3 product outputs both processed and raw (gyro and 
accelerometer) sensor data.

The MGC R3 product can be used as a stand-alone unit 
or as an IMU in other systems. The product is designed 
for high precision maritime applications such as offshore 
operations and seabed mapping

• 0.01° roll and pitch accuracy
• 0.04° heading accuracy GNSS aided
• Includes INS capability
• Outputs on RS-232, RS-422 and Ethernet
• High output data rate (200 Hz)
• Precise heave at long wave periods by use of PFreeHeave® algorithms
• Lever arm compensation to two individually configurable monitoring points
• Small size and low power consumption
• Each MGC delivered with a Calibration Certificate
• Selectable communication protocols in the Windows based configuration software"
        } }, eccns);
    }

    private static void ProcessUserInput()
    {
        string prompt = ReadPrompt();

        while (!string.IsNullOrEmpty(prompt))
        {
            // Call GPT3
            Completion completion = GPT.CallGpt(prompt).Result;

            // Show the result
            ShowCompletion(completion);

            // Ask for more input
            prompt = ReadPrompt();
        }
    }

    private static void ProcessProducts(List<Product> products, List<ECCN> eccns)
    {
        foreach (Product product in products)
        {
            List<GptPlayground.Prompt> prompts = new List<GptPlayground.Prompt>();

            // System prompt tells the system what type of LLM it is
            prompts.Add(new GptPlayground.Prompt
            {
                role = ChatRole.System,
                content = "You are an assistant helping determine which export control classification number (ECCN) should be used to classify a product. You do NOT know anything about existing jurisdictions or regimes. You do not know any ECCN information other than what we provide to you."
            });

            // Assistant prompts give background information and other data to the LLM
            prompts.Add(new GptPlayground.Prompt
            {
                role = ChatRole.Assistant,
                content = $"An example of an ECCN is {eccns.First().Code}, which has description {eccns.First().Description} and controls items {eccns.First().ItemsControlled}."
            });

            // User prompts ask the LLM to do something it will respond to
            prompts.Add(new GptPlayground.Prompt
            {
                role = ChatRole.User,
                content = BuildPromptForProduct(product)
            });

            Console.WriteLine("***********************************************************************************************");
            Console.WriteLine($"Product {product.Name} from {product.Manufacturer}:");
            Completion completion = GPT.CallGpt(prompts, 200).Result;
            ShowCompletion(completion);
            Console.WriteLine("***********************************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    private static string BuildPromptForProduct(Product product)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Give me the top 5 most likely ECCN numbers for the following product along with a percentage likelihood each is the right answer and a short one-sentence explanation as to why in decreasing order:");
        sb.Append($"Name: {product.Name}");
        sb.Append($"Manufacturer: {product.Manufacturer}");
        sb.Append($"Description: {product.Description}");
        sb.Append("Return the results as a JSON array with no extra explanation.");

        return sb.ToString();
    }

    #region Data Loading and Console Helpers
    private static void ShowCompletion(Completion completion)
    {
        Console.WriteLine();
        Console.WriteLine(completion.choices.FirstOrDefault()?.message?.content);
        Console.WriteLine($"{completion.usage.prompt_tokens} prompt tokens, {completion.usage.completion_tokens} completion tokens, {completion.usage.cost.ToString("C5")} cost");
        Console.WriteLine();
    }

    private static string ReadPrompt()
    {
        Console.WriteLine("Prompt? (hit enter twice to submit the request)");
        StringBuilder result = new StringBuilder();

        string line = Console.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            result.Append(line);
            line = Console.ReadLine();
        }

        Console.WriteLine("...thinking...");
        return result.ToString();
    }

    private static List<ECCN> ReadEccnData()
    {
        Console.WriteLine("Loading BIS JSON data...");

        List<ECCN> eccns = null;

        using (StreamReader sr = new StreamReader("..\\..\\..\\..\\..\\bis.json"))
        {
            string json = sr.ReadToEnd();
            eccns = JsonConvert.DeserializeObject<List<ECCN>>(json);
        }

        Console.WriteLine($"Read {eccns.Count} ECCN records from JSON");

        return eccns;
    }

    private static List<Product> ReadProductData()
    {
        Console.WriteLine("Loading Product data...");

        List<Product> products = null;

        using (StreamReader sr = new StreamReader("..\\..\\..\\..\\..\\ProductExamples.json"))
        {
            string json = sr.ReadToEnd();
            products = JsonConvert.DeserializeObject<List<Product>>(json);
        }

        Console.WriteLine($"Read {products.Count} Product records from JSON");

        return products;
    }
    #endregion
}