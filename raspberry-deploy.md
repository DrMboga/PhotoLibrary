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

## Install Keycloak

[Article](https://www.keycloak.org/getting-started/getting-started-zip)
[Article](https://medium.com/@hasnat.saeed/setup-keycloak-server-on-ubuntu-18-04-ed8c7c79a2d9)

### Install JDK

```bash
java -version
sudo apt-get update
sudo apt install openjdk-17-jre
```

### Download and Extract Keycloak Server

```bash
cd /projects/keycloak
sudo wget https://github.com/keycloak/keycloak/releases/download/23.0.4/keycloak-23.0.4.zip
unzip keycloak-23.0.4.zip
rm keycloak-23.0.4.zip
cd keycloak-23.0.4/
cp -r . ..
cd ..
rm -r keycloak-23.0.4/
```

As result - unzipped keycloak app will be in the `/projects/keycloak` folder

### Configure keycloak

```bash
cd ../home/pi/projects/keycloak/conf/
sudo nano keycloak.conf
```

```
db=postgres
db-username=postgres
db-password=MyDocker6
proxy=edge
```

NOTE: Create database `keycloak` manually

### Create a user for running the keycloak service

- Create a group `keycloak`

```bash
sudo groupadd keycloak
```

- Create a system user `keycloak` with home directory `/projects/keycloak` (`-r` means system user, `-s` - login shell option)

```bash
sudo useradd -r -g keycloak -d /home/pi/projects/keycloak -s /sbin/nologin -c "Keycloak service user" keycloak
```

- Give new user ownership and permissions to Keycloak installation folder

```bash
# under `/projects` folder. This command gives an ownership to user `keycloak` (first) to folder `keycloak` (last)
sudo chown -R keycloak: keycloak

sudo chmod o+x /home/pi/projects/keycloak/bin/
```

### Create systemD service for keycloak

- Copy a service file

```bash
scp -r ./keycloak.service pi@192.168.0.65:/home/pi/projects
```

- Register a service as `systemctl`

ssh:

```bash
#Copy service file to the system dir:
sudo cp keycloak.service /etc/systemd/system/keycloak.service
# Restart daemon
sudo systemctl daemon-reload
# Start services
sudo systemctl start keycloak.service
# Enable auto start
sudo systemctl enable keycloak.service
```

## Install NGINX

[Article](https://pimylifeup.com/raspberry-pi-nginx/)

```bash
sudo apt install nginx
sudo systemctl start nginx
```

## Photo library backend initial setup as service

- Change values in `appsettings.json`

- Publish backend as self-contained app. And copy it to the raspberry:

```bash
cd ./backend/PhotoLibraryBackend

dotnet publish -c release -r linux-arm64 --self-contained

scp -r ./bin/release/net8.0/linux-arm64/publish pi@192.168.0.65:/home/pi/projects/photo-library
```

- Using ssh, add run permissions and run app (just for test, in next step, we should create a service to run it):

```bash
cd projects/photo-library/publish
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

## Photo library backend initial setup

## Setup NGINX for backend and frontend

### Keycloak

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
    listen        8845;
    server_name   example.com *.example.com;
    location / {
        proxy_pass         http://127.0.0.1:8080;
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

```bash
sudo systemctl restart nginx.service
```

#### Access keycloak:

Internally:
http://127.0.0.1:8080

From outside:
http://192.168.0.65:8845

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
    }
  }
```

TODO: add settings for SignalR [like here](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-8.0#linux-with-nginx)

#### Access backend:

Internally:
http://127.0.0.1:5000

From outside:
http://192.168.0.65:8850/swagger/index.html

# Rollout a new version from dev machine to running application on Raspberry

There is a [powershell script](./raspberry-deploy.ps1)
Before running the `raspberry-deploy.ps1` script, create a `raspberry-deploy.env` and fill these parameters:

```ini
RASPBERRY_ADDR=pi@192.168.0.42
RASPBERRY_PWD=$ecReT
KEYCLOAK_AUTH_SERVER=your-keycloak-service-address
KEYCLOAK_REALM=your-keycloak-realm
KEYCLOAK_RESOURCE=your-keycloak-resource
KEYCLOAK_CLIENT_ID=your-keycloak-client-id
KEYCLOAK_REDIRECT_URL=your-local-frontend-url
PHOTO_LIBRARY_LOCAL_PATH=../../../photo-library-lib
PHOTO_LIBRARY_LOCAL_DELETE_FOLDER=../../../photo-library-lib-deleted
PHOTO_DB_CONNECTION_STRING=Host=localhost;Database=photo;Username=postgres;Password=MyDocker6
PHOTO_LIBRARY_BACKEND_URL=your-backend-local-url
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

# Useful links

- [Host asp.net core in Linux with NGINX](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-sles)
- [Raspberry Pi 4 specifications](https://www.raspberrypi.com/products/raspberry-pi-4-model-b/specifications/)
  `Quad core Cortex-A72 (ARM v8) 64-bit SoC` -> ARM 64
- [RID catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

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
```
