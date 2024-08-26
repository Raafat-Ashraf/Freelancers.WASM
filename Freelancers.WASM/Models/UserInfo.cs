namespace Freelancers.WASM.Models;

public class UserInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }

    public Dictionary<string, string> Claims { get; set; } = [];
}
