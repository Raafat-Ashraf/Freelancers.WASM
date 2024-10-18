using Blazored.LocalStorage;
using Freelancers.WASM.Models;
using Freelancers.WASM.Models.Profile;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Freelancers.WASM.Identity;

public class CustomAuthProvider(ILocalStorageService _localStorageService,
    IHttpClientFactory _httpClientFactory) : AuthenticationStateProvider, IAccountManagement
{

    private readonly HttpClient _httpClient = _httpClientFactory.CreateClient("Auth");

    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {

        var user = _unauthenticated;

        try
        {
            var userResponse = await _httpClient.GetAsync("/account");

            userResponse.EnsureSuccessStatusCode();

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

            if (userInfo is not null)
            {

                var claims = new List<Claim>
                {
                    new(ClaimTypes.GivenName , userInfo.FirstName),
                    new(ClaimTypes.Surname , userInfo.LastName),
                    new(ClaimTypes.Email , userInfo.Email),
                };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(CustomAuthProvider));
                user = new ClaimsPrincipal(claimsIdentity);
            }

        }
        catch
        {
            // Logging
        }




        return new AuthenticationState(user);
    }


    public async Task<AuthResult> LoginAsync(LoginModel credentials)
    {

        var response = await _httpClient.PostAsJsonAsync("Auth/Login", new
        {
            credentials.Email,
            credentials.Password,
        });

        if (response.IsSuccessStatusCode)
        {

            var tokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponseModel>();

            await _localStorageService.SetItemAsync("jwt-access-token", tokenResponse!.token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return new AuthResult { Succeeded = true };
        }

        var details = await response.Content.ReadAsStringAsync();
        var errors = ExtractErrors(details);

        return new()
        {
            Succeeded = false,
            ErrorList = [.. errors]
        };
    }


    public async Task<AuthResult> RegisterAsync(RegisterModel model)
    {

        try
        {

            var response = await _httpClient.PostAsJsonAsync("Auth/SignUp", new
            {
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.ConfirmPassword
            });

            if (response.IsSuccessStatusCode)
            {
                return new AuthResult { Succeeded = true };
            }

            var details = await response.Content.ReadAsStringAsync();
            var errors = ExtractErrors(details);

            return new AuthResult { Succeeded = false, ErrorList = [.. errors] };


        }
        catch
        {

        }

        return new AuthResult { Succeeded = false, ErrorList = ["An unknown error prevented registration"] };

    }


    public async Task<AuthResult> ForgetPasswordAsync(string email)
    {

        var response = await _httpClient.PostAsJsonAsync("Auth/forget-password", new
        {
            email
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Succeeded = true };
        }

        var details = await response.Content.ReadAsStringAsync();
        var errors = ExtractErrors(details);

        return new()
        {
            Succeeded = false,
            ErrorList = [.. errors]
        };
    }



    public async Task<AuthResult> ResendConfirmationEmailAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/resend-confirmation-email", new
        {
            email
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Succeeded = true };
        }

        var details = await response.Content.ReadAsStringAsync();
        var errors = ExtractErrors(details);

        return new()
        {
            Succeeded = false,
            ErrorList = [.. errors]
        };
    }


    public async Task<AuthResult> ConfirmPasswordAsync(string userId, string code)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/confirm-email", new
        {
            userId,
            code
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Succeeded = true };
        }

        var details = await response.Content.ReadAsStringAsync();
        var errors = ExtractErrors(details);

        return new()
        {
            Succeeded = false,
            ErrorList = [.. errors]
        };
    }

    public async Task<AuthResult> ResetPasswordAsync(ResetPasswordModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/reset-password", new
        {
            model.Email,
            model.Code,
            model.NewPassword
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Succeeded = true };
        }

        var details = await response.Content.ReadAsStringAsync();
        var errors = ExtractErrors(details);

        return new()
        {
            Succeeded = false,
            ErrorList = [.. errors]
        };
    }

    public async Task<ProfileResponseModel> GetUserProfile()
    {
        var response = await _httpClient.GetFromJsonAsync<ProfileResponseModel>("account");

        return response;
    }


    private List<string> ExtractErrors(string details)
    {
        var errors = new List<string>();

        try
        {
            using (var document = JsonDocument.Parse(details))
            {
                var root = document.RootElement;

                if (root.TryGetProperty("errors", out var errorElement))
                {
                    if (errorElement.ValueKind == JsonValueKind.Object)
                    {
                        // Handle the case where "errors" is an object with properties as keys
                        foreach (var property in errorElement.EnumerateObject())
                        {
                            // property.Name represents the key (e.g., "Password"), property.Value is the array of error messages
                            foreach (var errorMessage in property.Value.EnumerateArray())
                            {
                                errors.Add(errorMessage.GetString());
                            }
                        }
                    }
                    else if (errorElement.ValueKind == JsonValueKind.Array)
                    {
                        // Handle the case where "errors" is an array of objects
                        foreach (var error in errorElement.EnumerateArray())
                        {
                            if (error.TryGetProperty("description", out var description))
                            {
                                errors.Add(description.GetString());
                            }
                            else
                            {
                                errors.Add("An unknown error occurred.");
                            }
                        }
                    }
                    else
                    {
                        errors.Add("Unexpected error format in the response.");
                    }
                }
                else
                {
                    errors.Add("An unexpected error occurred.");
                }
            }
        }
        catch (JsonException)
        {
            errors.Add("An error occurred while parsing server response.");
        }

        return errors;
    }

    public void NotifyAuthState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }


}