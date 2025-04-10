using System.Text.RegularExpressions;


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
    }  // TODO: !if results[thing] == string.empty : display alert to user to add it
}
