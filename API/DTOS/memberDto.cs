using API.Entities;
using API.Enums;
using API.Extensions;

namespace API.DTOS
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastActive { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Interests { get; set; }
        public string LookingFor { get; set; }
        public string Introduction { get; set; }
        public string PhotoUrl { get; set; }
        public int Age { get; set; } 
        public GenderEnum Gender { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Photo> Photos { get; set; }
        


       
    }
}
