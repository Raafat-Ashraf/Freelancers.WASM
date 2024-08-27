using Microsoft.AspNetCore.Components;

namespace Freelancers.WASM.Pages.Account;

public partial class ConfirmEmail
{
    [Parameter]
    public string UserId { get; set; } = string.Empty;

    [Parameter]
    public string Code { get; set; } = string.Empty;

    private string[] errorList = [];

    public bool IsEmailConfirmed { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var uri = new Uri(Navigation.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("userId", out var userId))
            UserId = userId!;

        if (query.TryGetValue("code", out var code))
            Code = code!;

        await ConfirmPasswordAsync();

        await base.OnInitializedAsync();
    }


    private async Task ConfirmPasswordAsync()
    {
        var result = await AccountManagement.ConfirmPasswordAsync(UserId, Code);

        if (result.Succeeded)
            IsEmailConfirmed = true;
        else
            errorList = result.ErrorList;

    }

}
