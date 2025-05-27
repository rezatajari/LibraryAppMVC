namespace LibraryAppMVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
    }
}
