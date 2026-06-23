using System.ComponentModel.DataAnnotations;

namespace LibraryAppMVC.Shared.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; } = DateTime.Now;
}
