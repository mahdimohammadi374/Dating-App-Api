namespace API.DTOS
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecieverId { get; set; }
        public string RecieverUserName { get; set; }
        public string RecieverPhotoUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

    }
}
