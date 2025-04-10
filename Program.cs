


namespace CVtesting;

class Program
{
    static async Task Main(string[] args)
    {
        
        string parsedText = InputParser.ExtractTextFromDocx("/home/roni/repos/CVtesting/test.docx");
        string entext = await AzureAI.Translate("cs","en", parsedText);
        Console.WriteLine(entext);

        Dictionary<string,string> regexres = Utils.MatchContacts(parsedText);
        //Console.WriteLine($"{regexres["phone"]}\n{regexres["email"]}\n{regexres["linkedin"]}");

        //List<string> skills = await NERaccess.GetSkills(entext);
        //foreach (string skill in skills) { Console.WriteLine($"{skill}"); }
    }
}