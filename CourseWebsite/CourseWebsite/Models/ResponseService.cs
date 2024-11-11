namespace CourseWebsite.Models
{
    public class ResponseService<T>
    {
        public T? data { get; set; }
        public bool success { get; set; } = true;
        public string message { get; set; } = string.Empty;
    }
}
