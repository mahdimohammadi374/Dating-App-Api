using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class loginDto
    {
        [Display(Name = "username")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(3, ErrorMessage = "{0} can be atleast 3 characters")]
        public string username { get; set; }
        [Display(Name = "password")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(6, ErrorMessage = "{0} can be atleast 3 characters")]
        public string password { get; set; }
    }
}
