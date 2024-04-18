using Layers.Middleware;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class RegisterViewModel
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

        [Required]
        [ValidatePhone]
        public string PhoneNumber { get; set; }
      
        [Required]
        [DataType(DataType.Text)]
        public string Address { get; set; }


    }
}
