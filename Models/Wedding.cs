using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding_planner.Models
{
    public class Wedding
    {
        [Key] // Primary Key
        public int WeddingId { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters")]
        [MaxLength(45, ErrorMessage = "Name is too long")]
        [Display(Name = "Wedder One:")]
        public string WedderOne { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters")]
        [MaxLength(45, ErrorMessage = "Name is too long")]
        [Display(Name = "Wedder Two:")]
        public string WedderTwo { get; set; }

        [Required(ErrorMessage = "Required")]
        [FutureDate]
        [Display(Name = "Date:")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Address:")]
        public string Address { get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Many to One (Many Weddings to One User who created)
        public int HostUserId { get; set; }
        public User HostUser { get; set; }

        // Many to Many (One User to Many Weddings, One Wedding to Many Users)
        public List<RSVP> AllRSVPs { get; set; }

    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime inputDate = Convert.ToDateTime(value);

            // logic for datetime =>  value.Date > CurrentTime
            if (inputDate > DateTime.UtcNow)
            {
                return new ValidationResult("Invalid date.");
            }
            else 
            { 
                return ValidationResult.Success; 
            }
        }
    }
}