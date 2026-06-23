
namespace LibraryAppMVC.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; } = DateTime.Now;
}
