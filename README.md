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

# Deploy to Rasberry PI

## 1. Install .Net

[source](https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian)

For the first time, add the Microsoft package signing key to your list of trusted keys and add the package repository.:

```bash
wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

Install the runtime

```bash
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0
```

Update runtime:

```bash
sudo apt-get update
sudo apt-get upgrade
```

## 2. Publish backend

```bash
dotnet publish -c release -r linux-x64
```
