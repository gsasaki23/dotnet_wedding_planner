using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding_planner.Models
{
    public class User
    {
        [Key] // Primary Key
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters")]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters")]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(8, ErrorMessage = "must be at least 8 characters")]
        [DataType(DataType.Password)] // Changes form input's type to password
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [NotMapped] // NOT ADDING TO DB
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Must match password")]
        [Display(Name = "Confirm Password:")]
        public string PasswordConfirm { get; set; }

        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // One to Many (One User to Many Weddings, just the ones created by this User)
        public List<Wedding> HostedWeddings { get; set; }

        // Many to Many (One User to Many Weddings, One Wedding to Many Users)
        public List<RSVP> AllRSVPs { get; set; }
    }
}