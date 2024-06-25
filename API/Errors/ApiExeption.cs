namespace API.Errors
{
    public class ApiExeption : ApiResponse
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string Detail { get; }
        public ApiExeption(int statusCode, string message = null, string detail = null) : base(statusCode, message)
        {
            StatusCode = statusCode;
            Message = message;
            Detail = detail;
        }

    }
}
