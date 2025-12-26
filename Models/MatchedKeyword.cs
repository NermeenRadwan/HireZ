namespace HireZ.Models
{
    public class MatchedKeyword
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public string Keyword { get; set; } = null!;
        public bool IsPresent { get; set; }

        // Navigation
        public Resume? Resume { get; set; }
    }
}
