#pragma warning disable
    public class Assets(string kind)
    {
        public string projectKind { get; set; } = kind;
        public List<Entity> entities { get; set; }
        public List<Document> documents { get; set; }
    }

    public class Document
    {
        public string location { get; set; }
        public string language { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class Entity
    {
        public int regionOffset { get; set; }
        public int regionLength { get; set; }
        public List<Label> labels { get; set; }
    }

    public class Label
    {
        public string category { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
    }

    public class Metadata(string projectName, string storageInputContainerName, string projectKind, string description, string language, bool multilingual)
{
    public string projectName { get; set; } = projectName;
    public string storageInputContainerName { get; set; } = storageInputContainerName;
    public string projectKind { get; set; } = projectKind;
    public string description { get; set; } = description;
    public string language { get; set; } = language;
    public bool multilingual { get; set; } = multilingual;
    public Settings settings { get; set; }
}

public class Root
    {
        public string projectFileVersion { get; set; }
        public string stringIndexType { get; set; }
        public Metadata metadata { get; set; }
        public Assets assets { get; set; }
    }

    public class Settings
    {
    }

#pragma warning restore

