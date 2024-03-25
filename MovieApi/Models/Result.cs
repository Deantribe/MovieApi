namespace MovieApi.Models
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Return { get; set; }

        public static Result<T> ReturnFail(T? result) => new()
        {
            Success = false,
            Return = result,
        };

        public static Result<T> ReturnSuccess() => new()
        {
            Success = true,
        };
    }
}