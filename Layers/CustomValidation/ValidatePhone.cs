using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Layers.Middleware
{
    public class ValidatePhone : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phone = value as string;
            if (string.IsNullOrWhiteSpace(phone))
            {
                return new ValidationResult("Phone number cannot be null or empty.");

            }
            var prefix = phone.Substring(0, 2);
            if (prefix != "98" && prefix != "97" && prefix != "96")
            {
                return new ValidationResult("Phone number should start with 97, 96, or 98.");
            }
            return ValidationResult.Success;

        }
    }
    public class ValidatePassword : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _minUniqueCharacters;

        public ValidatePassword(int minLength = 8, int minUniqueCharacters = 3)
        {
            _minLength = minLength;
            _minUniqueCharacters = minUniqueCharacters;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            if (password.Length < _minLength)
            {
                return new ValidationResult($"Password must be at least {_minLength} characters long.");
            }

            if (password.Distinct().Count() < _minUniqueCharacters)
            {
                return new ValidationResult($"Password must contain at least {_minUniqueCharacters} unique characters.");
            }

            // Additional complexity checks
            if (!ContainsUppercaseLetter(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!ContainsLowercaseLetter(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!ContainsDigit(password))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }


            if (IsCommonPassword(password))
            {
                return new ValidationResult("Password is too common or weak.");
            }

            return ValidationResult.Success;
        }

        private bool ContainsUppercaseLetter(string password)
        {
            return password.Any(char.IsUpper);
        }

        private bool ContainsLowercaseLetter(string password)
        {
            return password.Any(char.IsLower);
        }


        private bool ContainsDigit(string password)
        {
            return password.Any(_ => char.IsDigit(_));
        }

        // Method to check against common or weak passwords 
        private bool IsCommonPassword(string password)
        {
            string[] commonPasswords = { "password", "123456", "qwerty", "abc123", "bokoChoda" }; // Example weak passwords

            return commonPasswords.Contains(password.ToLower());
        }
    }
    public class ValidateEmail : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("email shouldnot be empty");
            }
            if (!IsValidEmail(email))
            {
                return new ValidationResult("it is not valid format");
            }
            return ValidationResult.Success;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                var regex = new Regex(pattern);
                return regex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }

}
