using Newtonsoft.Json;

namespace CVtesting;

public class Data
{
    public static void MakeFiles()
    {
        using (StreamReader sr = new("/home/roni/repos/CVtesting/dataset_raw.json"))
        {
            string line;
            int i = 1;
            while ((line = sr.ReadLine()!) != null)
            {
                using (StreamWriter sw = new($"/home/roni/repos/CVtesting/datajson/{i}.json"))
                    sw.WriteLine(line);
                i++;
            }
        }
    }

    public static void ProcessText()
    {
        for (int i = 1; i <= 220; i++)
        {
            string pathread = $"/home/roni/repos/CVtesting/datajson/{i}.json";
            string pathwrite = $"/home/roni/repos/CVtesting/DATASET/{i}.txt";

            using (StreamReader sr = new(pathread))
            {
                string jsonString = sr.ReadLine()!;
                Resume dataRoot = JsonConvert.DeserializeObject<Resume>(jsonString)!;

                using StreamWriter sw = new(pathwrite);
                sw.WriteLine(dataRoot.content);
            }
        }
    }

    public static void Label() // creates an azureAI-formatted json label document  
    {
        Root root = new()
        {
            projectFileVersion = "2023-04-01",
            stringIndexType = "Utf16CodeUnit"
        };

        Metadata meta = new("customNERattempt", "nerblob", "CustomEntityRecognition", "please work.", "en", false)
        {
            settings = new()
        };
        root.metadata = meta;

        Assets assets = new("CustomEntityRecognition")
        {
            documents = [],
            entities = []
        };

        for (int i = 1; i <= 220; i++)
        {
            string jsonPath = $"/home/roni/repos/CVtesting/datajson/{i}.json";
            using StreamReader sr = new(jsonPath);
            string jsonString = sr.ReadLine()!;
            Resume inputData = JsonConvert.DeserializeObject<Resume>(jsonString)!;

            Document doc = new()
            {
                location = $"{i}.txt",
                language = "en",
                entities = []
            };

            foreach (var annotation in inputData.annotation) // labels -> document object
            {
                foreach (var point in annotation.points)
                {
                    int offset = point.start;
                    int length = point.end - point.start;

                    foreach (var labelValue in annotation.label)
                    {
                        Entity entity = new()
                        {
                            regionOffset = offset,
                            regionLength = length,
                            labels =
                            [
                                new Label
                                {
                                    category = labelValue,
                                    offset = offset,
                                    length = length
                                }
                            ]
                        };
                        doc.entities.Add(entity);
                    }
                }
            }

            assets.documents.Add(doc);
        }

        root.assets = assets;
        string outputFile = "/home/roni/repos/CVtesting/labels3.json";
        string result = JsonConvert.SerializeObject(root, Formatting.Indented);
        File.WriteAllText(outputFile, result);
    }
}