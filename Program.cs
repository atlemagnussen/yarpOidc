using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using YarpOidc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;

    options.RequestHeaders.Add("X-Original-Proto");
    options.RequestHeaders.Add("X-Original-For");
});


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    //options.KnownIPNetworks.Clear();
    options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse("192.168.1.0"), 24));
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
    options.AllowedHosts.Clear();
    //options.ForwardLimit = 2;
    //options.KnownProxies.Add(IPAddress.Parse("192.168.1.2"));
});


var authServer = Environment.GetEnvironmentVariable("AUTH_SERVER");
var authClient = Environment.GetEnvironmentVariable("AUTH_CLIENT");

builder.AddOidcAuthentication(authServer, authClient);

builder.ConfigureCookies();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Protection", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme);
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseForwardedHeaders();

app.UseHttpLogging();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Request RemoteIp: {RemoteIpAddress}",
        context.Connection.RemoteIpAddress);

    await next(context);
});

if (!app.Environment.IsDevelopment())
{
    
    app.UseHsts();
    //app.UseHttpsRedirection();
}



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


app.MapRazorPages()
    .WithStaticAssets();

app.MapReverseProxy();

app.Run();
