using System.ComponentModel.DataAnnotations;

namespace Freelancers.WASM.Models;

public class LoginModel
{
    // [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
