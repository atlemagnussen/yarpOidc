# YARP OIDC

ASP.NET Core Reverse proxy with external OIDC authentication 

- [ASP.NET Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication) with one external OIDC authority
- [YARP](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/yarp-overview?view=aspnetcore-10.0)

Protect any web resource with proper authentication and without exposing it

## Why

Lets say you have a web app on http://192.168.1.101:8000 at your home that you want make more accessible, even outside your home.

But it has none/weak protection/authentication.

With this setup you can require authentication from any OIDC authority.  
Auhentication will be httpOnly secure cookie, which is as good as it gets. 

## Setup

### 1. Outer reverse proxy 

Use another reverse proxy in front, like [nginx](./nginx.conf)  
Set up domain and TLS, for example `https://yarp.example.com` and point this to yarpOidc

### 2. inner reverse proxy

This is where yarpOidc comes in.

environment variables

```sh
AUTH_SERVER=https://id.example.com # authentication authority
AUTH_CLIENT=authapp                # clientId
YARP_CONFIG_PATH=/data/yarp.json   # easier in container, map /data volume since yarp config is too verbose for using environment vars
STATIC_FOLDER_PATH=/optional/path/to/fallback/static/html
```

example of yarp.json

```json
{
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "AuthorizationPolicy": "Protection",
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "Destinations": {
          "destination1": {
            "Address": "http://192.168.1.101:8000/"
          }
        }
      }
    }
  }
}
```

## Notes on forwarded headers

This implementation trusts all forwarding proxies, but you can easily change that in Program.cs

## run/build

```sh
dotnet run --environment Production
```

```sh
podman build -f Containerfile --tag yarp-oidc:0.1 .

podman run --env-file .env -d --rm -p 8000:8080 yarp-oidc:0.1
```