using Freelancers.WASM.Extensions;
using Freelancers.WASM.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Freelancers.WASM.Pages.Account;

public partial class SignUp
{

    [SupplyParameterFromForm]
    private RegisterModel? Model { get; set; }

    private string[] _errorList = [];

    public bool IsProcessing { get; set; }


    protected override async Task OnInitializedAsync()
    {
        Model ??= new RegisterModel();

        if (await AuthenticationStateProvider.UserIsAuthenticated())
            Navigation.NavigateTo("/");

    }



    private async Task RegisterAsync()
    {
        IsProcessing = true;

        var result = await AccountManagement.RegisterAsync(Model!);

        if (result.Succeeded)
        {
            Navigation.NavigateTo("/login?isNew=true");
        }
        else
        {
            _errorList = result.ErrorList;
        }

        IsProcessing = false;

    }
}
