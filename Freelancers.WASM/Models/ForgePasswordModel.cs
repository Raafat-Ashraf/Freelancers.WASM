using System.ComponentModel.DataAnnotations;

namespace Freelancers.WASM.Models;

public class ForgePasswordModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}
