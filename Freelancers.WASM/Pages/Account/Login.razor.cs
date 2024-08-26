using Freelancers.WASM.Extensions;
using Freelancers.WASM.Models;
using Microsoft.AspNetCore.Components;

namespace Freelancers.WASM.Pages.Account;

public partial class Login
{
    [SupplyParameterFromForm]
    private LoginModel? Model { get; set; }

    private string[] errorList = [];

    public bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model ??= new();

        if (await AuthenticationStateProvider.UserIsAuthenticated())
            Navigation.NavigateTo("/");

    }



    private async Task LoginAsync()
    {
        IsProcessing = true;

        var result = await AccountManagement.LoginAsync(Model!);

        if (result.Succeeded)
            Navigation.NavigateTo("/");
        else
        {
            IsProcessing = false;
            errorList = result.ErrorList;
        }

    }
}
