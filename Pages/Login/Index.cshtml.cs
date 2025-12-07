using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YarpOidc.Model;

namespace YarpOidc.Pages.Login;

[AllowAnonymous]
public class Index(ILogger<Index> logger, SignInManager<ApplicationUser> signInManager) : PageModel
{
    private readonly ILogger<Index> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    [TempData]
    public string? ErrorMessage { get; set; }

    public ApplicationUser? AppUser { get; set; } = null;

    public string ReturnUrl {get;set;} = string.Empty;
    public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string? returnUrl)
    {
        AppUser = ClaimsHelper.GetAppUser(User); // check if already logged in

        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }
    
        // Clear the existing external cookie to ensure a clean login process
        //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (externalProviders is not null && externalProviders.Any())
            ExternalProviders = [.. externalProviders];

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostLogout()
    {
        if (User.Identity?.IsAuthenticated == true)
            await _signInManager.SignOutAsync();

        return Redirect("/");
    }
}