using System.ComponentModel.DataAnnotations;

namespace PhezuluManagementSystem.ViewModels;

public class CreateStaffViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Staff email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 10)]
    [DataType(DataType.Password)]
    [Display(Name = "Temporary password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Staff role")]
    public string Role { get; set; } = string.Empty;
}