using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YarpOidc.Model;

namespace YarpOidc.Pages.ExternalLogin;

[AllowAnonymous]
public class ExternalLoginModel(SignInManager<ApplicationUser> signInManager,
    IAuthenticationSchemeProvider schemes) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IAuthenticationSchemeProvider _schemes = schemes;

    [TempData]
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet() => RedirectToPage("/Login/Index");

    /// <summary>
    /// Challenge External Provider/Microsoft authentication - will redirect user into MS and prompt for user etc
    /// </summary>
    /// <param name="provider">Microsoft</param>
    /// <param name="returnUrl">The url which it should be redirected back to after external login</param>
    /// <returns></returns>
    public IActionResult OnPost(string provider, string? returnUrl)
    {
        var redirectUrl = Url.Page("/ExternalLogin/Index", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }

    /// <summary>
    /// The endpoint where user will be redirected back to after they have logged in with external provider
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <param name="remoteError"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl, string? remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("/Login/Index", new { ReturnUrl = returnUrl });
        }

        // get information from the external login
        var externalInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalInfo == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("/Login/Index", new { ReturnUrl = returnUrl });
        }

        // external user
        var externalUser = ClaimsHelper.GetAppUser(externalInfo.Principal);
        if (externalUser is null)
        {
            ErrorMessage = "Could not get external user.";
            return RedirectToPage("/Login/Index", new { ReturnUrl = returnUrl });
        }

        // use persistent cookie, not session cookie
        var localSignInProps = new AuthenticationProperties
        {
            IsPersistent = true
        };

        // var idToken = externalInfo.AuthenticationTokens?.FirstOrDefault(a => a.Name == "id_token");
        // if (idToken is not null)
        //     localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken.Value }]);

        // sign in to our application
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, externalInfo.Principal, localSignInProps);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return Redirect(returnUrl);
    }
}
