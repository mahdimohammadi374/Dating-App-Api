namespace API.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse(int statusCode=400, string message = null) : base(statusCode, message)
        {
        }
    }
}
