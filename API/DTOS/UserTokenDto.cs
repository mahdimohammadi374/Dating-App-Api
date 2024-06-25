using API.Enums;

namespace API.DTOS
{
    public class UserTokenDto
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        public string photoUrl  { get; set; }
        public string KnownAs { get; set; }
        public GenderEnum Gender { get; set; }
        public List<string> Roles { get; set; }

    }
}
