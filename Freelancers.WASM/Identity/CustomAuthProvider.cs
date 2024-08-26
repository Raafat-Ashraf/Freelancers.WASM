using Blazored.LocalStorage;
using Freelancers.WASM.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
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
        var jsonPayload = JsonSerializer.Serialize(credentials);
        var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var httpClient = _httpClientFactory.CreateClient("Auth");
        var response = await httpClient.PostAsync("Auth/Login", requestContent);

        if (response.IsSuccessStatusCode)
        {

            var tokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponseModel>();

            await _localStorageService.SetItemAsync("jwt-access-token", tokenResponse!.token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return new AuthResult { Succeeded = true };
        }



        return new()
        {
            Succeeded = false,
            ErrorList = ["Invalid email or password"]
        };
    }



    /*var jwtToken = await _localStorageService.GetItemAsync<string>("jwt-access-token");
                var claims = ParseClaimsFromJwt(jwtToken!);
                user = new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));*/






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