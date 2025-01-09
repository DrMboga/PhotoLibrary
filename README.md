# PhotoLibrary

A family photo library application.
Your entire family's cherished memories, beautifully organized and easily accessible in one smart and intuitive application. Browse through generations of moments with ease, sort by date, event, or family member, and relive your most precious memories in a seamless and visually stunning way. PhotoLibrary is the perfect digital companion to ensure your family's photographic history is always at your fingertips, ready to be shared and cherished for generations to come.

[Application specification](./docs/spec.md)
[Raspberry Pi Deployment manual](./raspberry-deploy.md)

# Useful resources:

- [Hands on react tutorial](https://handsonreact.com/docs/)
- [Redux toolkit tutorial](https://redux-toolkit.js.org/tutorials/overview)
- [Keycloak js adapter](https://www.keycloak.org/docs/latest/securing_apps/index.html#_javascript_adapter)
- [Material UI library](https://mui.com/material-ui/getting-started/)
- [Material UI icons search](https://mui.com/material-ui/material-icons/)

# Frontend environment values

There is a `.env` file in the root of frontend near the package.json. It is excluded from source control and contains some environment setting which used by frontend. Here is the example of such `.env` file:

```
REACT_APP_BACKEND_URL=https://localhost:7056
REACT_APP_VERSION=$npm_package_version
```

# How to teach ML model to classify images using ML Net CLI

1. Create a folder which contain a model data to learn. Each subfolder contains a set of images, and subfolder name will be used as a label name
2. Install ML CLI

```bash
dotnet tool install --global mlnet-win-x64 --version 16.13.9
```

3. Train model. (A test console application will be generated)

```bash
mlnet image-classification --dataset "..\photo-library-lib\ML" --log-file-path "..\mlTests" --name "PhotoLibraryModel"
```

# How to regenerate proto messages on backend and frontend.

After making changes in \*.proto file, run following:

```bash
protoc --proto_path=protobuf --csharp_out=backend/PhotoLibraryBackend.Common --csharp_opt=base_namespace=PhotoLibraryBackend.Common media-info.proto
```

```bash
protoc --proto_path=protobuf --ts_out=frontend/src/model media-info.proto
```

# Install HEIC tools to host Linux OS

```bash
sudo apt install libheif-examples
```
