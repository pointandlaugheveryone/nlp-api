using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;
using Newtonsoft.Json;

namespace CVtesting;

public class AzureAIService
{
    public static async Task<String> Translate(string from, string to, string text)
    {
        string endpoint = "https://api.cognitive.microsofttranslator.com";
        string key = Utils.GetKey("translationKey").Result;
        string route = $"/translate?api-version=3.0&from={from}&to={to}";
        string location = "germanywestcentral";

        string textfrom = text; // <-- Comment out
        //string textEN = <parameter>;
        object[] textObject = new object[] { new { Text = textfrom } };
        var textJson = JsonConvert.SerializeObject(textObject);

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(textJson, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string resultJson = await response.Content.ReadAsStringAsync();
                var translations = JsonConvert.DeserializeObject<List<translateResponse>>(
                    resultJson
                );
                string textto =
                    translations?[0]?.Translations?[0]?.Text
                    ?? "Translation request or parsing failed";

                Console.WriteLine(textto);
                return textto;
            }
            else
            { //TODO: azure application insights or whatever for logging
                Console.WriteLine($"{response.StatusCode}\n{response.Content}");
                return String.Empty;
            }
        }
    }

    private static async Task<List<Dictionary<string, string>>> GetEntities(string text)
    {
        string endpoint = "https://cv-ai-apis.cognitiveservices.azure.com/";
        string key = Utils.GetKey("aai-key").Result;
        var client = new TextAnalyticsClient(new Uri(endpoint), new Azure.AzureKeyCredential(key));

        List<string> documents = [];
        documents.Add(text);

        string projectName = "custom-ner-skills";
        string deploymentName = "azureNER";
        var actions = new TextAnalyticsActions()
        {
            RecognizeCustomEntitiesActions =
            [
                new(projectName, deploymentName),
            ],
        };
        AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(
            documents,
            actions
        );
        await operation.WaitForCompletionAsync();

        List<Dictionary<string, string>> results = [];
        // from azure samples
        await foreach (AnalyzeActionsResult documentsResult in operation.Value)
        {
            IReadOnlyCollection<RecognizeCustomEntitiesActionResult> customEntitiesActionResults =
                documentsResult.RecognizeCustomEntitiesResults;
            foreach (
                RecognizeCustomEntitiesActionResult customEntitiesActionResult in customEntitiesActionResults
            )
            {
                foreach (
                    RecognizeEntitiesResult documentResults in customEntitiesActionResult.DocumentsResults
                )
                {
                    foreach (CategorizedEntity entity in documentResults.Entities)
                    {
                        Dictionary<string, string> EntityResults = new Dictionary<string, string>{
                            {"text",$"{entity.Text}"},
                            {"category",$"{entity.Category}"},
                            {"offset",$"{entity.Offset}"},
                            {"length", $"{entity.Length}"},
                            {"confidence", $"{entity.ConfidenceScore}"}
                        };
                        results.Add(EntityResults);
                    }
                }
            }
        }
        return results;
    }

    public static void TestEntities(string text)
    { // async is built into Azure ai sdk
        string texten = Translate("en", "cs", text).Result;
        List<Dictionary<string, string>> results = GetEntities(texten).Result;
        foreach (Dictionary<string, string> result in results)
        {
            // TODO
            Console.WriteLine($"{result["text"]} {result["category"]} {result["confidence"]}");
        }
    }

}
