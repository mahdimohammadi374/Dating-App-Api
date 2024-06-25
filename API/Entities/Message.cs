namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public int RecieverId { get; set; }
        public string RecieverUserName { get; set; }
        public string  Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool SenderDeleted { get; set; } = false;
        public bool RecierverDeleted { get; set; } = false;
        public bool IsRead { get; set; }=false;
        public User Sender { get; set; }
        public User Reciever { get; set; }
    }
}
