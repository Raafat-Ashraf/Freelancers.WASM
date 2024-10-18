using Freelancers.WASM.Models;
using Freelancers.WASM.Models.Profile;

namespace Freelancers.WASM.Identity;

public interface IAccountManagement
{
    Task<AuthResult> LoginAsync(LoginModel credentials);

    Task<AuthResult> RegisterAsync(RegisterModel model);
    Task<AuthResult> ForgetPasswordAsync(string email);
    Task<AuthResult> ResendConfirmationEmailAsync(string email);
    Task<AuthResult> ConfirmPasswordAsync(string email, string code);
    Task<AuthResult> ResetPasswordAsync(ResetPasswordModel model);
    Task<ProfileResponseModel> GetUserProfile();
    // Task LogoutAsync();
}
