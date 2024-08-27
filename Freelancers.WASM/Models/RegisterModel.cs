using System.ComponentModel.DataAnnotations;

namespace Freelancers.WASM.Models;

public class RegisterModel
{

    [Required, Length(3, 100)]
    public string FirstName { get; set; } = string.Empty;


    [Required, Length(3, 100)]
    public string LastName { get; set; } = string.Empty;


    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;


    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
