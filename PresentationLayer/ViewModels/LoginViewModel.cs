using Layers.Middleware;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
        [Required]
        [ValidateEmail]
        public string Email { get; set; }
        [Required]
        [ValidatePassword]
        public string Password { get; set; }


    }
}
