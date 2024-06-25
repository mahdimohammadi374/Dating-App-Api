using API.Enums;
using API.Extensions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class User : IdentityUser<int>
    {
        public GenderEnum Gender { get; set; }
        public override string UserName { get; set; }
        public override string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastActive { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Interests { get; set; }
        public string LookingFor { get; set; }
        public string Introduction { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<UserLike> SourceUserLikes { get; set; }
        public ICollection<UserLike> TargetUserLikes { get; set; }
        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageRecieved { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }


        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
    }
}
 