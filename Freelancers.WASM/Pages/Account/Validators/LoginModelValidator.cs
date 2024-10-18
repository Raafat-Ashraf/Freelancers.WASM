using FluentValidation;
using Freelancers.WASM.Models;

namespace Freelancers.WASM.Pages.Account.Validators;

public class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .EmailAddress();
    }
}
