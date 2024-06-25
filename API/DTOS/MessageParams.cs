namespace API.DTOS
{
    public class MessageParams:BasePagination
    {
        public string? UserName { get; set; }
        public string Container { get; set; }
    }
}
