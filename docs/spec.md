# PhotoLibrary

## 1. Introduction and Goals

This document describes the `PhotoLibrary` application. It used as home photo library observer. A family photos was collected during 15 years just as non-organized folder on a USB hard drive and takes up 200Gb. The purpose of the application is to organize photos, make them easy accessible from any device which connected to the local home network.

### 1.1 Requirements Overview

The following goals have been established for this system:
| | |
| --- | --- |
| 1. | Whole library should still be stored in USB hard drive |
| 2. | Library should be accessed from any device connected to the local home network |
| 3. | Photos can be filtered by date, location, some tags |
| 4. | Import mechanism. New photos can be added to the library from iPhones |
| 5. | UI should be adaptive to the desktop or mobile screen |
| 6. | App should be accessible only for logged-in users |

### 1.2. UI design

![MainScreen](./img/MainScreen.png)

### 1.3. Use cases

### 1.3.1. Spotlight screen

![SpotlightScreen](./img/Spotlight%20screen.png)

1. Random photo shows a random photo which has tag `People`. It is shown in a Carousel with 3 photos before and 3 after sorted by date from the random photo and also have `People` tag
2. This dat by years shows a list of carousels with the random photo shoot this day with the same logic as random photo. Each carousel in the list shows phot of the day ib hte different year

### 1.3.2. Library screen

![LibraryScreen](./img/LibraryScreen.png)

1. Photos shown in the list ordered by date from latest photo.
2. Photos loading by chunks while user scrolls.
3. While scrolling the appropriate chip in the top becomes active.
4. By clicking the chip in the top, photos shows selected year - sorted from bottom tot top
5. By clicking the photo, selected photo becomes bigger right in the list

TODO: To be added

## 2. Architecture Constraints

Application can not be accessible outside of the local home network

### 2.1. Making the thumbnails from videos

There is a way to make a thumbnail from video file using the `NReco.VideoConverter` nuget library. But `NReco.VideoConverter` is only for Windows. There is an alternative lib - `NReco.VideoConverter.LT`, but there is a license needed (75$).

But fortunately, the Raspberry PI OS contains the `ffmpeg` utility by default. And there is a way to make thumbnails using this command:

```bash
ffmpeg -i IMG_6976.MOV -ss 00:00:01.000 -vframes 1 _thumbnail_IMG_6976.jpg
```

We need this thumbnails while ASP.NET backend indexing (importing) photo library. The backend runs under the docker container.
Solution would be like this:

1. For video file, write the line to some file in the host OS (Raspberry PI)
2. Mark in the DB that this file is sent to host OS to make a thumbnail and save how the thumbnail file should be named.
3. In the host OS make some bash script which will read this file, execute command `ffmpeg ...` and save thumbnails somewhere in host file system
4. Make a cron job in host OS which will run that script by some timetable
5. In the backend make a separate long-run task which will check if thumbnails are created, then grab them to the DB and delete them from file system

| :memo: | Actually, if backend runs under the OS without docker, it would be much simpler - we can run bash shell with `ffmpeg` right from the code and the file access to the attached HDD would be much easier |
| ------ | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |

## 3. Context and Scope

## 4. Solution Strategy

1. Raspberry Pi device is used as a home server. USB hard drive should be connected to it and Raspberry Pi device is connected to the local home server
2. There should be 2 layers of application - Backend and frontend.
3. Backend is implemented via .Net web Application and it has access to USB Hard drive file system.
4. Backend using the PostgreSQL database instance which installed on the same Raspberry Pi home server.
5. Backend is deployed as .net app to the Raspberry Pi server. (the asp.net runtime should be installed in the server and script on the dev machine should publish the web api app and copy to server)
6. Frontend is implemented as React web application
7. Frontend is deployed as Docker Container? Or maybe it is easier to install `nginx` on a raspberry and deploy the client the same way as backend - build on dev machine and copy `dist` to the Raspberry.
8. [Keycloack](https://www.keycloak.org/) is used as the identity provider. It is also installed to the Raspberry Pi as Docker container.

## 5. Building Block View

High level architecture
![High level architecture](./img/HighLevel.png)

## 6. Runtime View

## 7. Deployment view

## 8. Crosscutting Concepts

Backend sends media files one by one via SignalR. The message structure should be single for both Backend and Client app. For this purpose the `Protobuf` structure used.

To generate a c# class from protobuf message use following:

```bash
protoc --proto_path=protobuf --csharp_out=backend/PhotoLibraryBackend.Common --csharp_opt=base_namespace=PhotoLibraryBackend.Common media-info.proto
```

## 9. Architecture Decisions

## 10. Quality Requirements

## 11. Risks and Technical Debt
