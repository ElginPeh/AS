using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ASAssignment.ViewModels
{
    public class Register
    {
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card must contain 16 digits.")]
        public string CreditCard { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Mobile number must contain 8 digits.")]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Delivery { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }
    }
}