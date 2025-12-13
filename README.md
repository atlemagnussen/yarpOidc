# YARP OIDC

Example implementation of. 

ASP.NET Core Reverse proxy with external OIDC authentication 

- Configures ASP.NET Identity with one external OIDC authority
- Adds [YARP](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/yarp-overview?view=aspnetcore-10.0) from either appsettings or another file

This gives you a way of protecting any web resource with proper authentication and not expose the internal app


## run/build

```sh
dotnet run --environment Production
```


```sh
podman build -f Containerfile --tag yarp-oidc:0.1 .

podman run --env-file .env -d --rm -p 8000:8080 yarp-oidc:0.1
```