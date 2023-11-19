# PhotoLibrary

A family photo library application.
Your entire family's cherished memories, beautifully organized and easily accessible in one smart and intuitive application. Browse through generations of moments with ease, sort by date, event, or family member, and relive your most precious memories in a seamless and visually stunning way. PhotoLibrary is the perfect digital companion to ensure your family's photographic history is always at your fingertips, ready to be shared and cherished for generations to come.

[Application specification](./docs/spec.md)

# Useful resources:

- [Hands on react tutorial](https://handsonreact.com/docs/)
- [Redux toolkit tutorial](https://redux-toolkit.js.org/tutorials/overview)
- [Keycloak js adapter](https://www.keycloak.org/docs/latest/securing_apps/index.html#_javascript_adapter)
- [Material UI library](https://mui.com/material-ui/getting-started/)
- [Material UI icons search](https://mui.com/material-ui/material-icons/)

## Keycloack setup

### On Dev machine

[Spec](https://www.keycloak.org/getting-started/getting-started-docker)

### 1.

```bash
docker run -p 8070:8080 -e KEYCLOAK_ADMIN=<username> -e KEYCLOAK_ADMIN_PASSWORD=<password> quay.io/keycloak/keycloak:22.0.5 start-dev
```

### 2. Add user

1. Go to `http://localhost:8070/admin/`
2. Create new realm (`photo-library`)
3. Create new user (set e-mail verified)
4. Go to credentials and set password for created user

### 3. Add app to the keycloak

1. Open admin console. Go to clients, click Create
2. Set up id (`photo-library-app`), add name (optional), choose OpenID
3. Setup valid redirect URL (`http://localhost:3000` or prod root url)
4. Setup valid origins (`http://localhost:3000` or prod root url)

# Deploy backend to Raspberry PI

[Host asp.net core in Linux with NGINX](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-sles)
[Raspberry Pi 4 specifications](https://www.raspberrypi.com/products/raspberry-pi-4-model-b/specifications/)
`Quad core Cortex-A72 (ARM v8) 64-bit SoC` -> ARM 64

## 1. Publish backend

[RID catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

- Publish backend as self-contained app. And copy it to the raspberry:

```bash
cd ./backend/PhotoLibraryBackend

dotnet publish -c release -r linux-arm64 --self-contained

scp -r ./bin/release/net8.0/linux-arm64/publish pi@192.168.0.65:/home/pi/projects/photo-library
```

= Using ssh, add run permissions and run app (just for test, in point 3, we should create a service to run it):

```bash
cd projects/photo-library/publish
chmod +x ./PhotoLibraryBackend
./PhotoLibraryBackend
```

## 2. Install and setup nginx

[instructions](https://pimylifeup.com/raspberry-pi-nginx/)

```bash
sudo apt install nginx
sudo systemctl start nginx
```

- Setup nginx.config

```bash
cd etc/nginx
sudo nano nginx.conf
```

Add this to http section:

```json
  map $http_connection $connection_upgrade {
    "~*Upgrade" $http_connection;
    default keep-alive;
  }

  server {
    listen        8850;
    server_name   example.com *.example.com;
    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection $connection_upgrade;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
  }
```

TODO: add settings for SignalR [like here](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-8.0#linux-with-nginx)

## 3. Run backed as linux service

1. Create a service file
2. Register a service as `systemctl`
