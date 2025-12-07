using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using YarpOidc.Model;

namespace YarpOidc;

public static class Config
{
    public static void AddOidcAuthentication(this WebApplicationBuilder builder, string? authServer, string? authClient)
    {
        if (string.IsNullOrWhiteSpace(authServer) || string.IsNullOrWhiteSpace(authClient))
            return; // disabled
        
        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Stores.MaxLengthForKeys = 128; 
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddUserStore<DummyUserStore>()
            .AddDefaultTokenProviders() // Keeps the token providers you were asking about
            .AddSignInManager();

        builder.Services.AddAuthentication(options =>
        {
            // probably default
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme; 
        })
        .AddCookie(IdentityConstants.ApplicationScheme, options =>
        {
            options.LoginPath = "/Login"; 
            options.AccessDeniedPath = "/AccessDenied";
            // Add your persistence configuration here:
            options.ExpireTimeSpan = TimeSpan.FromDays(14); 
            options.SlidingExpiration = true;
        })
        .AddCookie(IdentityConstants.ExternalScheme, options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        })
        .AddOpenIdConnect("external", "External", options =>
        {
            options.Authority = authServer;
            options.ClientId = authClient;
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true; // to get customer_id ++

            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.SignOutScheme = IdentityConstants.ApplicationScheme;
            options.CallbackPath = "/signin-oidc"; // default - allow this in auth server

            options.Scope.Add("openid");
            options.Scope.Add("profile");

            options.AccessDeniedPath = "/AccessDenied";
            
            options.MapInboundClaims = false; // Prevents Microsoft from changing claim types

            options.ClaimActions.Clear();
            options.ClaimActions.DeleteClaim("nonce");
            options.ClaimActions.DeleteClaim("at_hash");

            options.ClaimActions.MapJsonKey("role", "role", "role"); 
            options.ClaimActions.MapUniqueJsonKey("name", "name", "name");
            options.ClaimActions.MapUniqueJsonKey("customer_id", "customer_id", "customer_id");
            
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };

            options.Events.OnRemoteFailure = context =>
            {
                context.Response.Redirect("/Error?message=" + context.Failure?.Message);
                context.HandleResponse();
                return Task.CompletedTask;
            };
        });
    }

    internal static void ConfigureCookies(this WebApplicationBuilder builder)
    {
        // Application Cookie is what we use to hold the authentication cookie
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax; // to work with localhost / test environment

            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;

            options.ExpireTimeSpan = TimeSpan.FromDays(14);
            options.SlidingExpiration = true; // slide after half of expirationTime

            options.LoginPath = "/Login";
            options.AccessDeniedPath = "/AccessDenied";
        });

        // External cookie is what we use in authenticate with external provider
        builder.Services.ConfigureExternalCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }
}