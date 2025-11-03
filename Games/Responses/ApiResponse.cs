namespace Games.Api.Responses
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public IEnumerable<string>? Messages { get; set; }
        public object? Pagination { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data)
        {
            Data = data;
        }
    }
}
