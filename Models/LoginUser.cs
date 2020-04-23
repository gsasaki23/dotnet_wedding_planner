using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding_planner.Models
{
    // We need a User that's different from the regular user to preserve the register and login on the same view.
    // But we don't need a separate class from the User
    [NotMapped]
    public class LoginUser
    {
        [Required(ErrorMessage = "Required.")]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string LoginPassword { get; set; }
    }
}