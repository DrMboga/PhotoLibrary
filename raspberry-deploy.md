# Deployment instructions

The photo library hosted on a Raspberry Pi machine and accessible via local network.
Photo library works with the HDD attached to Raspberry PI. All components of the application are run natively on a Linux OS without Docker containers.
There is a Powershell script which can rollout next versions of application from dev machine to Raspberry server.
But to make this rollout working, there are some initial setup steps should be done on the Raspberry Pi manually.

# Preparing Raspberry Pi environment

## Install PostgreSQL database engine

[Article](https://pimylifeup.com/raspberry-pi-postgresql/)

- Install

```bash
sudo apt update
sudo apt full-upgrade

sudo apt install postgresql
```

- Change postgres user

```bash
sudo su postgres

createuser pi -P --interactive
# enter password when prompted
exit
```

## Set up File Access Control for media/pi

```bash
cd /media
sudo setfacl -m u:pi:rwx pi
```

## Install NGINX

[Article](https://pimylifeup.com/raspberry-pi-nginx/)

```bash
sudo apt install nginx
sudo systemctl start nginx
```

## Install HEIC converter tool

```bash
sudo apt install imagemagick heif-gdk-pixbuf
```

## Photo library backend initial setup as service

- Change values in `appsettings.json`

- Publish backend as self-contained app. And copy it to the raspberry:

```bash
cd ./backend/PhotoLibraryBackend

dotnet publish -c release -r linux-arm64 --self-contained

scp -r ./bin/release/net8.0/linux-arm64/publish/* pi@192.168.0.65:/home/pi/projects/photo-library/backend
```

- Using ssh, add run permissions and run app (just for test, in next step, we should create a service to run it):

```bash
cd projects/photo-library/backend
chmod +x ./PhotoLibraryBackend
./PhotoLibraryBackend
```

- Copy a service file

```bash
scp -r ./photo-library.service pi@192.168.0.65:/home/pi/projects/photo-library
```

- Register a service as `systemctl`

ssh:

```bash
#Copy service file to the system dir:
sudo cp photo-library.service /etc/systemd/system/photo-library.service
# Restart daemon
sudo systemctl daemon-reload
# Start services
sudo systemctl start photo-library.service
# Enable auto start
sudo systemctl enable photo-library.service
```

- Migrate database on fitst start:

```bash
curl http://127.0.0.1:5000
curl http://127.0.0.1:5000/migrateIdentityDb
```

## Photo library frontend initial setup

- change values in `.env`
- build react app in production mode

```bash
cd frontend
npm run build
scp -r ./build/* pi@192.168.0.65:/home/pi/projects/photo-library/frontend
```

## Setup NGINX for backend and frontend

### Backend

- Setup nginx.config

```bash
cd /etc/nginx
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

        # Configuration for WebSockets
        proxy_cache off;

        # Configuration for ServerSentEvents
        proxy_buffering off;

        # Configuration for LongPolling or if your KeepAliveInterval is longer than 60 seconds
        proxy_read_timeout 100s;
    }
  }
```

```bash
sudo systemctl restart nginx.service
```

#### Access backend:

Internally:
http://127.0.0.1:5000

From outside:
http://192.168.0.65:8850/swagger/index.html

## Frontend

1. Add permissions to nginx to frontend folders:

```bash
sudo gpasswd -a www-data pi

chmod g+x /home/pi && chmod g+x /home/pi/projects && chmod g+x /home/pi/projects/photo-library && chmod g+x /home/pi/projects/photo-library/frontend && chmod g+x /home/pi/projects/photo-library/frontend/static && chmod g+x /home/pi/projects/photo-library/frontend/static/css && chmod g+x /home/pi/projects/photo-library/frontend/static/js
```

```json
server {
    listen 8860;
    server_name photo-library.com *.photo-library.com;

    access_log /var/log/nginx/photo_library_frontend.log;
    error_log  /var/log/nginx/photo_library_frontend_error.log;

    root /home/pi/projects/photo-library/frontend;
    index index.html;

    location / {
        try_files $uri /index.html;
    }
}
```

```bash
sudo systemctl restart nginx.service
```

# Rollout a new version from dev machine to running application on Raspberry

There is a [powershell script](./raspberry-deploy.ps1)
Before running the `raspberry-deploy.ps1` script, create a `raspberry-deploy.env` and fill these parameters:

```ini
RASPBERRY_ADDR=pi@192.168.0.42
PHOTO_LIBRARY_LOCAL_PATH=../../../photo-library-lib
PHOTO_LIBRARY_LOCAL_DELETE_FOLDER=../../../photo-library-lib-deleted
PHOTO_DB_CONNECTION_STRING=Host=localhost;Database=photo;Username=postgres;Password=MyDocker6
IDENTITY_DB_CONNECTION_STRING=Host=localhost;Database=photo;Username=postgres;Password=MyDocker6
PHOTO_LIBRARY_BACKEND_URL=your-backend-local-url
POSITION_STACK_API_KEY=<API Key>
PHOTO_LIBRARY_BACKEND_DEV_URL=http://localhost:5101
TELEGRAM_BOT_TOKEN=<Your bot token>
TELEGRAM_CHAT_ID=<Your chat id>
GOOGLE_MAPS_API=<Your Api Key>
```

1. ssh:

```bash
sudo systemctl stop photo-library.service
```

2. dev machine:

```bash
cd ./backend/PhotoLibraryBackend

dotnet publish -c release -r linux-arm64 --self-contained

scp -r ./bin/release/net8.0/linux-arm64/publish pi@192.168.0.65:/home/pi/projects/photo-library
```

3. ssh:

```bash
sudo systemctl daemon-reload
sudo systemctl start photo-library.service
```

# Creating the Telegram bot and chat

1. Search for the “botfather” telegram bot in the Telegram client.
2. Type `/newbot` to create a new bot. You need to specify the bot's screen name and username. If the bot is successfully created, you will see the bot's API token like `356111742:cFiWcIKXX5SsYHDRDj34oa3YE`. You must not share this token with anyone.
3. Create a _public_ channel with a suitable name (the chat name started with `@` we will need to get chat id in step 5).
4. Add your bot to the list of administrators of the created channel. At least, the bot must have permission to post messages.
5. Get the chat ID, calling the `sendMessage` method like this:
   HTTP POST `https://api.telegram.org/bot[your-bot-token]/sendMessage`
   with this body as JSON:
   ```json
   {
     "parse_mode": "Markdown",
     "text": "_Hello there_\r\nFrom *Postman*\r\nTa da",
     "chat_id": "[public chat id started with @]"
   }
   ```
   Response will contain the numeric chat id like `"id": -1001701183067`,
6. Put the API token from step 2 and numeric chat id from step 5 into appropriate parameters in `appsettings.json` or/and into env values in the `raspberry-deploy.env`
7. As soon as we got the numeric chat id, we can make a created in step 3 channel as private. We don't need that public name started from `@` anymore.

# Useful links

- [Host asp.net core in Linux with NGINX](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-sles)
- [Raspberry Pi 4 specifications](https://www.raspberrypi.com/products/raspberry-pi-4-model-b/specifications/)
  `Quad core Cortex-A72 (ARM v8) 64-bit SoC` -> ARM 64
- [RID catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)
  = [Telegram bot api](https://core.telegram.org/bots/api#available-methods)

## Linux services helpful commands

```bash
# Some service tune commands
sudo systemctl stop photo-library.service
sudo systemctl disable photo-library.service
# Service logs
sudo journalctl -u photo-library.service
# Services list
sudo systemctl list-units --type=service --all
(q for exit)
# Service status
 sudo systemctl status keycloak
 (q for exit)

# Service status last 100 rows
 sudo journalctl -u photo-library.service -n 100 --no-pager
```
