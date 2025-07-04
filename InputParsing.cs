using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;

namespace CVtesting;

public class InputParsing { // TODO: support pdf parsing
    
    public static string ExtractTextFromDocx(string filePath)
    {
        using WordprocessingDocument docReader = WordprocessingDocument.Open(filePath, false);  // open read-only

        var body = docReader.MainDocumentPart!.Document.Body!;
        List<string> segments = [];

        foreach (var element in body.Elements())
        {
            string elementText = string.Join("", element.Descendants<Text>().Select(t => t.Text)); 
            if (!string.IsNullOrWhiteSpace(elementText)) { segments.Add(elementText); }
            else { segments.Add(" "); }  // empty cells or lines used for formatting
        }

        string text = string.Join(" ", segments);

        if (text.Length < 50)
        {
            throw new Exception("Parsing failed");
        }

        return text;
    }
}

