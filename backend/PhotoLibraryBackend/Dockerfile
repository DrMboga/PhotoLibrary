FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhotoLibraryBackend/PhotoLibraryBackend.csproj", "PhotoLibraryBackend/"]
COPY ["PhotoLibraryBackend.Common/PhotoLibraryBackend.Common.csproj", "PhotoLibraryBackend.Common/"]
COPY ["PhotoLibraryBackend.MediaReader/PhotoLibraryBackend.MediaReader.csproj", "PhotoLibraryBackend.MediaReader/"]
COPY ["PhotoLibraryBackend.Data/PhotoLibraryBackend.Data.csproj", "PhotoLibraryBackend.Data/"]
RUN dotnet restore "PhotoLibraryBackend/PhotoLibraryBackend.csproj"
COPY . .
WORKDIR "/src/PhotoLibraryBackend"
RUN dotnet build "./PhotoLibraryBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PhotoLibraryBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Native tools and libraries
RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    imagemagick \
    heif-gdk-pixbuf

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhotoLibraryBackend.dll"]
