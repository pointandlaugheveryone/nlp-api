


using System.Threading.Tasks;

namespace CVtesting;

class Program
{
    static void Main(string[] args)
    {
        using StreamReader sr = new("/home/ronji/repos/CVtesting/test.txt");
        string resume = sr.ReadToEnd();
        AzureAIService.TestEntities(resume);
    }
}