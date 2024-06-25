using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class UpdateUserDto
    {

        [Required]
        public string Email { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Interests { get; set; }
        [Required]
        public string LookingFor { get; set; }
        [Required]
        public string Introduction { get; set; }
    }
}
