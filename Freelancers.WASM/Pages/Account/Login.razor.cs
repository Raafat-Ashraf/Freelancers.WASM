using Freelancers.WASM.Extensions;
using Freelancers.WASM.Models;
using Freelancers.WASM.Pages.Account.Validators;
using Microsoft.AspNetCore.Components;

namespace Freelancers.WASM.Pages.Account;

public partial class Login
{
    [SupplyParameterFromForm]
    private LoginModel? Model { get; set; }


    [SupplyParameterFromQuery(Name = "IsNew")]
    private bool IsNew { get; set; }



    private string[] errorList = [];

    public bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model ??= new();

        if (await AuthenticationStateProvider.UserIsAuthenticated())
            Navigation.NavigateTo("/");

    }

    public string EmailValidation { get; set; } = string.Empty;
    private async Task LoginAsync()
    {
        var validator = new LoginModelValidator();
        var validationResult = await validator.ValidateAsync(Model);
        if (!validationResult.IsValid)
        {
            EmailValidation = validationResult.Errors.FirstOrDefault()?.ErrorMessage!;
            return;
        }


        IsProcessing = true;

        var result = await AccountManagement.LoginAsync(Model!);

        if (result.Succeeded)
            Navigation.NavigateTo("/");
        else
            errorList = result.ErrorList;

        IsProcessing = false;
    }



    private async Task ResendConfirmationEmailAsync()
    {
        IsProcessing = false;
        await AccountManagement.ResendConfirmationEmailAsync(Model!.Email);
    }
}
