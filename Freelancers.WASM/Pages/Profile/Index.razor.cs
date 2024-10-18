using Freelancers.WASM.Models.Profile;

namespace Freelancers.WASM.Pages.Profile;

public partial class Index
{
    public ProfileResponseModel Profile { get; set; } = new();

    protected override void OnInitialized()
    {
        _ = GetUserDetailsAsync();
        base.OnInitialized();
    }

    private async Task GetUserDetailsAsync()
    {
        await AccountManagement.GetUserProfile();

    }
}
