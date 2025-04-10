using System.Text;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;

namespace CVtesting;

public class AzureAI
{

    private static async Task<String> GetAzureKey(string keyName)
    {
        var kvName = "CVbutbetter";
        var kvUri = $"https://{kvName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        var secret = await client.GetSecretAsync(keyName);
        string key = secret.Value.Value;

        return key;
    }

    public static async Task<String> Translate(string from, string to, string text)
    {
        string endpoint = "https://api.cognitive.microsofttranslator.com";
        string key = GetAzureKey("translationKey").Result;
        string route = $"/translate?api-version=3.0&from={from}&to={to}";
        string location = "germanywestcentral";

        string textfrom = text; // <-- Comment out
        //string textEN = <parameter>;
        object[] textObject = new object[] { new { Text = textfrom }}; 
        var textJson = JsonConvert.SerializeObject(textObject);

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage()) {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(textJson, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode) {
            string resultJson = await response.Content.ReadAsStringAsync();
            var translations = JsonConvert.DeserializeObject<List<translateResponse>>(resultJson);
            string textto = translations?[0]?.Translations?[0]?.Text ?? "Translation request or parsing failed";
            return textto;
            }
            else {  //TODO: azure application insights or whatever for logging
                Console.WriteLine($"{response.StatusCode}\n{response.Content}");
                return String.Empty;
            }
            
        }
    }

    public static void GetSkills(string text) { // async is actually built in into Azure ai sdk
        string key = GetAzureKey("cv-ai-apis-key").Result;
        AzureKeyCredential credentials = new(key);

        Uri endpoint = new("https://cv-ai-apis.cognitiveservices.azure.com/");
        
        var client = new TextAnalyticsClient(endpoint, credentials);
        var response = client.RecognizeEntities(text);

        var entities = response.Value;
        foreach (var entity in entities) {
            Console.WriteLine($"{entity.Text},{entity.Category}");
        }
    }
}




