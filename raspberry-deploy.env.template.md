This is a template for `raspberry-deploy.env` file.
Before running the `raspberry-deploy.ps1` script, create a `raspberry-deploy.env` and fill these parameters:
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
