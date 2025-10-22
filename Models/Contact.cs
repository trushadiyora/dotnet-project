using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContactManagerApp.Models
{
    [FirestoreData]
    public class Contact
    {
        [FirestoreProperty]
        public string Id { get; set; } = "";

        [FirestoreProperty]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = "";

        [FirestoreProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [FirestoreProperty]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; } = "";

        [FirestoreProperty]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; } = "";

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; }

        public Contact()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}