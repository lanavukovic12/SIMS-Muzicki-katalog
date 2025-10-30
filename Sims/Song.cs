namespace Sims
{
    public class Song
    {
        public string Title { get; set; }
        public string AssignedEditor { get; set; }
        public string Message { get; set; }
        public string Review { get; set; }
        public int? Grade { get; set; }
        public bool IsAssigned => !string.IsNullOrEmpty(AssignedEditor);

        public override string ToString()
        {
            return $"{Title} {(Grade != null ? $"(Grade: {Grade})" : "")}";
        }
    }
}