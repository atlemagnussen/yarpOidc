using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YarpOidc.Model;

namespace YarpOidc.Pages;

[AllowAnonymous]
public class AccessDeniedModel : PageModel
{
    public ApplicationUser? AppUser { get; set; } = null;
    public void OnGet()
    {
        AppUser = ClaimsHelper.GetAppUser(User);
    }
}