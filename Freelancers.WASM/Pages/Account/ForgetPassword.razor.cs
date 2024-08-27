using Freelancers.WASM.Extensions;
using Freelancers.WASM.Models;
using Microsoft.AspNetCore.Components;

namespace Freelancers.WASM.Pages.Account;

public partial class ForgetPassword
{
    [SupplyParameterFromForm]
    public ForgePasswordModel? Model { get; set; }

    private string[] errorList = [];

    public bool IsSend { get; set; }

    public bool IsProcessing { get; set; }



    protected override async Task OnInitializedAsync()
    {
        Model ??= new();

        if (await AuthenticationStateProvider.UserIsAuthenticated())
            Navigation.NavigateTo("/");

    }

    private async Task ForgetPasswordAsync()
    {
        IsProcessing = true;

        var result = await AccountManagement.ForgetPasswordAsync(Model!.Email);

        if (result.Succeeded)
            IsSend = true;
        else
            errorList = result.ErrorList;

        IsProcessing = false;
    }


    private async Task ResendConfirmationEmailAsync()
    {
        IsProcessing = false;
        var result = await AccountManagement.ResendConfirmationEmailAsync(Model!.Email);

        if (result.Succeeded)
            IsSend = true;
        else
            errorList = result.ErrorList;
    }

}
