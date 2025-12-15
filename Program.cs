using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using YarpOidc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// for debug
//builder.Services.AddHttpLogging(options =>
//{
 //   options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;
    // to see behind:
    // X-Forwarded-Proto: [Redacted]
    //options.RequestHeaders.Add("X-Forwarded-For");
    //options.RequestHeaders.Add("X-Forwarded-Proto");
//});


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    // | ForwardedHeaders.XForwardedHost seems to be not needed
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    
    // clear them to trust all proxies
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    
    // to not trust all add known proxies or networks
    // options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse("192.168.1.0"), 24));
    // options.KnownProxies.Add(IPAddress.Parse("192.168.1.2"));    

    // same as appsettings.json "AllowedHosts": "*"
    // options.AllowedHosts.Clear();
});


var oidcServer = Environment.GetEnvironmentVariable("OIDC_SERVER");
var oidcClient = Environment.GetEnvironmentVariable("OIDC_CLIENT");
var oidcSecret = Environment.GetEnvironmentVariable("OIDC_SECRET");

builder.AddOidcAuthentication(oidcServer, oidcClient, oidcSecret);

builder.ConfigureCookies();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Protection", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme);
    });
});

var yarpConfigPath = Environment.GetEnvironmentVariable("YARP_CONFIG_PATH");
builder.ConfigureYarp(yarpConfigPath);

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseForwardedHeaders();

// for debug
// app.UseHttpLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthorization();

app.Logger.LogInformation("yarpConfigPath={yarpConfigPath}", yarpConfigPath);

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

if (!string.IsNullOrWhiteSpace(oidcServer))
    app.Logger.LogInformation("oidcServer={oidcServer}", oidcServer);
if (!string.IsNullOrWhiteSpace(oidcClient))
    app.Logger.LogInformation("oidcClient={oidcClient}", oidcClient);


app.MapRazorPages()
    .WithStaticAssets();

app.MapReverseProxy();

app.Run();
