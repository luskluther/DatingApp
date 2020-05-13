using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForLoginDto
    {
        //[Required] // These are part of apicontroller attribute for the controller which can be used for validations
        public string UserName { get; set; }

        // [Required]
        // [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify password between 4-8 characters")]
        public string Password { get; set; }
    }
}