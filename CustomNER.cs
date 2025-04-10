using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using Azure.AI.TextAnalytics;

namespace CVtesting;

public class NERaccess
{
    private static async Task<String> GetKey()
    {
        const string keyName = "customNER";
        var kvName = "CVbutbetter";
        var kvUri = $"https://{kvName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        var secret = await client.GetSecretAsync(keyName);
        string key = secret.Value.Value;

        return key;
    }

    public static async Task<List<string>> GetSkills(string text)
    {
        string endpoint = "https://nerbutbetter.cognitiveservices.azure.com/";
        string key = GetKey().Result;

        var client = new TextAnalyticsClient(new Uri(endpoint), new Azure.AzureKeyCredential(key));
        string projectName = "CV_NER";
        string deploymentName = "NERskills";

        List<string> resultSkills = [];

        List<string> documents = [];
        documents.Add(text);
        var actions = new TextAnalyticsActions()
        {
            RecognizeCustomEntitiesActions = new List<RecognizeCustomEntitiesAction>()
            {
                new(projectName, deploymentName)
            }
        };
        AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(documents, actions);
        await operation.WaitForCompletionAsync();

        // https://github.com/Azure/azure-sdk-for-net/blob/Azure.AI.TextAnalytics_5.2.0-beta.3/sdk/textanalytics/Azure.AI.TextAnalytics/samples/Sample9_RecognizeCustomEntities.md
        // I know. Disgusting code.
        await foreach (AnalyzeActionsResult documentsResult in operation.Value)
        {
            IReadOnlyCollection<RecognizeCustomEntitiesActionResult> customEntitiesActionResults = documentsResult.RecognizeCustomEntitiesResults;
            foreach (RecognizeCustomEntitiesActionResult customEntitiesActionResult in customEntitiesActionResults)
            {
                foreach (RecognizeEntitiesResult documentResults in customEntitiesActionResult.DocumentsResults)
                {
                    foreach (CategorizedEntity entity in documentResults.Entities)
                    {
                        resultSkills.Add($"{entity.Text},{entity.Category}, {entity.ConfidenceScore}");
                        foreach (var skill in resultSkills) {
                            Console.WriteLine(skill);
                        }
                    }
                }
            }
        }
        return resultSkills;
    }
}