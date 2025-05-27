namespace LibraryAppMVC.Utilities
{
    public class ResultTask<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static ResultTask<T> Success(T data) => new() { Succeeded = true, Data = data, ErrorMessage = null };
        public static ResultTask<T> Failure(string? errorMessage) => new() { Succeeded = false, Data = default, ErrorMessage = errorMessage };
    }
}
