using System.ComponentModel.DataAnnotations;

namespace Freelancers.WASM.Models;

public class ResetPasswordModel
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
