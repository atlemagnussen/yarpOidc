# YARP OIDC

ASP.NET Core Reverse proxy server with OIDC protection

```sh
dotnet run --environment Production
```


```sh
podman build -f Containerfile --tag yarp-oidc:0.1 .

docker run --env-file .env.prod -d --rm -p 5000:5000 yarp-oidc:0.1
```