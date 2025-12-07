using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using YarpOidc;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

builder.Services.AddRazorPages();

var authServer = Environment.GetEnvironmentVariable("AUTH_SERVER");
var authClient = Environment.GetEnvironmentVariable("AUTH_CLIENT");

builder.AddOidcAuthentication(authServer, authClient);

builder.ConfigureCookies();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
        .Build();
});

var app = builder.Build();

app.UseExceptionHandler("/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseHsts();
    app.UseHttpsRedirection();
}

// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedProto
// });
app.UseRouting();
app.UseAuthorization();

var staticFolderPath = Environment.GetEnvironmentVariable("STATIC_FOLDER_PATH");
if (!string.IsNullOrWhiteSpace(staticFolderPath))
{
    app.Logger.LogInformation($"staticFolderPath=${staticFolderPath}");
    // this just works with FallbackPolicy
    var folderProvider = new PhysicalFileProvider(staticFolderPath);
    app.UseFileServer(new FileServerOptions { FileProvider = folderProvider });    
}
else
    app.UseFileServer(); // wwwroot

if (!string.IsNullOrWhiteSpace(authServer))
    app.Logger.LogInformation("AuthServer={authServer}", authServer);
if (!string.IsNullOrWhiteSpace(authClient))
    app.Logger.LogInformation("AuthClient={authClient}", authClient);

//app.UseDefaultFiles();
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = folderProvider,
//     RequestPath = "",
    
//     OnPrepareResponse = async context =>
//     {
//         var httpContext = context.Context;
//         var user = httpContext.User;
        
//         if (user.Identity is null || !user.Identity.IsAuthenticated)
//         {
//             httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             httpContext.Response.Redirect("/Login");
//         }
        
//         if (!user.HasClaim("customer_id", "1"))
//         {
//             httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
//             httpContext.Response.Redirect("/AccessDenied");
//         }
//     }
// });


app.MapRazorPages()
    .WithStaticAssets();

app.Run();
