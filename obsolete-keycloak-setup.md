# Keycloak setup on Raspberry

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
hostname=http://192.168.0.65:8845
```

NOTE: Create database `keycloak` manually

### Create admin user

Admin user can be added by accessing the portal locally. Or by specifying it with first run as environment variables.
So, to create it, we will run keycloak locally in development mode:

```bash
export KEYCLOAK_ADMIN=mike-admin //<username>
export KEYCLOAK_ADMIN_PASSWORD=WrhMZWXcYc6Q8Js //<password>
./kc.sh start-dev
```

NOTE: After running keycloak in dev mode, run:

```bash
sudo ./kc.sh build
```

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

```
[Unit]
Description=The Keycloak Server
After=syslog.target network.target
Before=httpd.service
[Service]
Environment=LAUNCH_JBOSS_IN_BACKGROUND=1
User=keycloak
Group=keycloak
LimitNOFILE=102642
PIDFile=/var/run/keycloak/keycloak.pid
ExecStart=/home/pi/projects/keycloak/bin/kc.sh start
WorkingDirectory=/home/pi/projects/keycloak/bin/
Restart=on-failure
RestartSec=10
KillMode=process
[Install]
WantedBy=multi-user.target
```

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

# On Dev machine (docker)

[Spec](https://www.keycloak.org/getting-started/getting-started-docker)

## 1.

```bash
docker run -p 8070:8080 -e KEYCLOAK_ADMIN=<username> -e KEYCLOAK_ADMIN_PASSWORD=<password> quay.io/keycloak/keycloak:22.0.5 start-dev
```

## 2. Add user

1. Go to `http://localhost:8070/admin/`
2. Create new realm (`photo-library`)
3. Create new user (set e-mail verified)
4. Go to credentials and set password for created user

## 3. Add app to the keycloak

1. Open admin console. Go to clients, click Create
2. Set up id (`photo-library-app`), add name (optional), choose OpenID
3. Setup valid redirect URL (`http://localhost:3000` or prod root url)
4. Setup valid origins (`http://localhost:3000` or prod root url)
