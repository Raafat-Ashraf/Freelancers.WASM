using Freelancers.WASM.Models;

namespace Freelancers.WASM.Pages.Account;

public partial class ResetPassword
{
    public ResetPasswordModel Model { get; set; } = new();

    private string[] errorList = [];

    public bool IsReset { get; set; }
    public bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var uri = new Uri(Navigation.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("email", out var email))
            Model.Email = email!;

        if (query.TryGetValue("code", out var code))
            Model.Code = code!;



        await base.OnInitializedAsync();
    }


    private async Task ResetPasswordAsync()
    {
        IsProcessing = true;
        var result = await AccountManagement.ResetPasswordAsync(Model);

        if (result.Succeeded)
        {
            errorList = [];
            IsReset = true;
        }
        else
            errorList = result.ErrorList;

        IsProcessing = false;
    }

}
