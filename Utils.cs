using System.Text.RegularExpressions;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;


namespace CVtesting;

public static class Utils {
    public static Dictionary<string,string> MatchContacts(string text) {
        Dictionary<string, string> results = [];

        string phonePattern = @"(?:420\s*)?(?<number>(?:\d\s*){9})";
        Regex phoneRegex = new(phonePattern);
        results["phone"] = phoneRegex.Match(text).Value;
        
        string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
        Regex emailRegex = new(emailPattern, RegexOptions.IgnoreCase);
        results["email"] = emailRegex.Match(text).Value;

        string linkedinPattern = @"(www\.)?linkedin\.com/in/[a-zA-Z0-9-]+";
        Regex linkedinRegex = new(linkedinPattern, RegexOptions.IgnoreCase);
        results["linkedin"] = linkedinRegex.Match(text).Value;
        
        return results;
    }  

    public static async Task<String> GetKey(string keyName) // get key for a deployed service
    {
        var kvName = "CVbutbetter";
        var kvUri = $"https://{kvName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        var secret = await client.GetSecretAsync(keyName);
        string key = secret.Value.Value;

        return key;
    }
}
