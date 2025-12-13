#!/usr/bin/env sh

podman build -f Containerfile --tag yarp-oidc:latest .

#podman stop yarpOidc
#podman run --name yarpOidc --env-file .env -d --rm -p 8002:8080 yarp-oidc .
