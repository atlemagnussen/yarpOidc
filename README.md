# YARP OIDC

ASP.NET Core Reverse proxy server with OIDC protection

```sh
dotnet run --environment Production
```


```sh
podman build -f Containerfile --tag yarp-oidc:0.1 .

podman run --env-file .env -d --rm -p 8000:8080 yarp-oidc:0.1
```