using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASAssignment.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string CreditCard { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Delivery { get; set; }

/*        [Display(Name = "Image")]
        public byte[] ProfilePicture { get; set; }*/

        [Required]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }
    }
}
