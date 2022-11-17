using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInVM
    {
        //Sign in için model oluşturduk
        [Required]
        [Display(Name ="Email Adresiniz")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Beni Hatırla")]
        public bool IsRemember { get; set; }




    }
}
