#pragma warning disable

public class Annotation
    {
        public List<string> label { get; set; }
        public List<Point> points { get; set; }
    }

public class Point
    {
        public int start { get; set; }
        public int end { get; set; }
        public string text { get; set; }
    }

public class Resume
    {
    public string content { get; set; }
    public List<Annotation> annotation { get; set; }
    public object extras { get; set; }
    }

#pragma warning restore
