namespace HireZ.DTOs.Resume
{
    public class UploadResponse
    {
        public int ResumeId { get; set; }
        public string FileName { get; set; } = null!;
        public string Message { get; set; } = "Uploaded";
    }
}
