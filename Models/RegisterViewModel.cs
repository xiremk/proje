
using System.ComponentModel.DataAnnotations;

namespace IdentityServerProject.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        //
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        //
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

