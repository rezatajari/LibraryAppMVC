namespace LibraryAppMVC.Utilities
{
    public class ResultTask<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public static ResultTask<T> Success(T data) => new ResultTask<T> { Succeeded = true, Data = data, ErrorMessage = null };
        public static ResultTask<T> Failure(string errorMessage) => new ResultTask<T> { Succeeded = false, Data = default, ErrorMessage = errorMessage };
    }
}
