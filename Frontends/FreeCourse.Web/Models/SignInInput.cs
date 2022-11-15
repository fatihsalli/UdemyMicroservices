using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInInput
    {
        //Sign in için model oluşturduk
        [Display(Name ="Email Adresiniz")]
        public string Email { get; set; }

        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool IsRemember { get; set; }




    }
}
