using Microsoft.AspNetCore.Components.Authorization;

namespace Freelancers.WASM.Extensions;

public static class UserExtensions
{
    public static async Task<bool> UserIsAuthenticated(this AuthenticationStateProvider authenticationStateProvider)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }
}
