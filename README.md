# YARP OIDC

Example of YARP Reverse proxy with external Open ID Connect authentication 

Combination of two technologies:

- [YARP](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/yarp-overview) is the reverse proxy powering [Azure's frontend fleet](https://devblogs.microsoft.com/dotnet/bringing-kestrel-and-yarp-to-azure-app-services/)
- [ASP.NET Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication) with one external OIDC authority


Protect any web resource with proper authentication

## Usecase

Lets say you run web app http://192.168.1.101:8000 in your home LAN. Now you want make it more accessible, even outside your home.

But it has no proper protection/authentication mechanism. So you want to wrap it in something that enables authentication

Combining YARP with OIDC you can require authentication for every request to multiple apps, enforced by your OIDC authority of choice.  
Auhentication will be httpOnly secure cookie.

You can achieve this with Nginx Plus as well, but then you have to pay.

## Setup

### 1. Outer reverse proxy 

In my experiment I run YARP in podman and therefore didn't manage to route port 80/443 directly

You *can* add TLS directly in ASP.NET and therefore also on YARP

So, Nginx as the outer proxy with TLS and the domains pointing down to the YARP instance

[nginx example of one site `https://yarp.example.com`](./nginx.conf)  


### 2. Inner reverse proxy

the YARP OIDC instance

environment variables

```sh
OIDC_SERVER=https://id.example.com # authentication authority
OIDC_CLIENT=authapp                # client
YARP_CONFIG_PATH=/data/yarp.json   # easier in container, map /data volume since yarp config is too verbose for using environment vars
STATIC_FOLDER_PATH=/optional/path/to/fallback/static/html
```

some OIDC servers requires secrets, like Google
```sh
OIDC_SECRET=xxx
```

example of yarp.json - see more on [YARP Docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/config-files)

```json
{
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "AuthorizationPolicy": "Protection",
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}",
          "Hosts" : ["yarp.example.com"]
        }
      },
      "route2": {
        "AuthorizationPolicy": "Anonymous",
        "ClusterId": "cluster2",
        "Match": {
          "Path": "{**catch-all}",
          "Hosts" : ["hello.example.com"]
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
      },
      "cluster2": {
        "Destinations": {
          "destination2": {
            "Address": "http://192.168.1.101:8001/"
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

or build container

```sh
podman build -f Containerfile --tag yarp-oidc:0.1 .

podman run --env-file .env -d --rm -p 8000:8080 yarp-oidc:0.1
```