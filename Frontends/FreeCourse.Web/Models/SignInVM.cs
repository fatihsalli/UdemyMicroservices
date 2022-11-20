using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInVM
    {
        //Sign in için model oluşturduk
        [Required]
        [Display(Name ="Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Remember me!")]
        public bool IsRemember { get; set; }




    }
}
