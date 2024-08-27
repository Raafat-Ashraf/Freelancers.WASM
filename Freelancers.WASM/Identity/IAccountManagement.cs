using Freelancers.WASM.Models;

namespace Freelancers.WASM.Identity;

public interface IAccountManagement
{
    Task<AuthResult> LoginAsync(LoginModel credentials);

    Task<AuthResult> RegisterAsync(RegisterModel model);

    // Task LogoutAsync();
}
