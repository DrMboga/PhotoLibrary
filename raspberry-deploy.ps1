class EnvironmentParameters {
    [string]$RaspberryAddress
    [string]$PhotoLibraryLocalPath
    [string]$PhotoLibraryLocalDeleteFolder
    [string]$PhotoDbConnectionString
    [string]$IdentityDbConnectionString
    [string]$PhotoLibraryBackendUrl
    [string]$PositionStackApiKey
    [string]$PhotoLibraryBackendDevUrl
}

function ReadParameters {
    $envParametersFileContent = Get-Content -Path ".\raspberry-deploy.env" | Out-String
    $envParametersLines = $envParametersFileContent -split '\r?\n'
    $envParametersObject = [EnvironmentParameters]::new()

    foreach($line in $envParametersLines) {
        $keyValue = $line -split '='
        switch ($keyValue[0]) {
            'RASPBERRY_ADDR' { 
                $envParametersObject.RaspberryAddress = $keyValue[1]
                continue
            }
            'PHOTO_LIBRARY_LOCAL_PATH' { 
                $envParametersObject.PhotoLibraryLocalPath = $keyValue[1]
                continue
            }
            'PHOTO_LIBRARY_LOCAL_DELETE_FOLDER' { 
                $envParametersObject.PhotoLibraryLocalDeleteFolder = $keyValue[1]
                continue
            }
            'PHOTO_DB_CONNECTION_STRING' { 
                # Connection string has this syntax: Host=localhost;Database=photo;Username=postgres;Password=MyDocker6
                # So, it is split and here we are assemble it back
                $connectionString = ""
                for ($i = 1; $i -lt $keyValue.Count; $i++) {
                    $connectionString = $connectionString + $keyValue[$i]
                    if ($i -lt $keyValue.Count-1) {
                        $connectionString = $connectionString + "="
                    }
                }
                $envParametersObject.PhotoDbConnectionString = $connectionString
                continue
            }
            'IDENTITY_DB_CONNECTION_STRING' { 
                # Connection string has this syntax: Host=localhost;Database=photo;Username=postgres;Password=MyDocker6
                # So, it is split and here we are assemble it back
                $connectionString = ""
                for ($i = 1; $i -lt $keyValue.Count; $i++) {
                    $connectionString = $connectionString + $keyValue[$i]
                    if ($i -lt $keyValue.Count-1) {
                        $connectionString = $connectionString + "="
                    }
                }
                $envParametersObject.IdentityDbConnectionString = $connectionString
                continue
            }
            'PHOTO_LIBRARY_BACKEND_URL' { 
                $envParametersObject.PhotoLibraryBackendUrl = $keyValue[1]
                continue
            }
            'POSITION_STACK_API_KEY' { 
                $envParametersObject.PositionStackApiKey = $keyValue[1]
                continue
            }
            'PHOTO_LIBRARY_BACKEND_DEV_URL' { 
                $envParametersObject.PhotoLibraryBackendDevUrl = $keyValue[1]
                continue
            }
        }
    }

    return $envParametersObject
}

function ChangeBackendApplicationSettings {
    param (
        [EnvironmentParameters]$EnvironmentParams,
        [string]$AppSettingsFileName
    )
    $AppSettingsContent = Get-Content -Path $AppSettingsFileName | Out-String | ConvertFrom-Json

    $PhotoLobrarySectionContent = $AppSettingsContent.PhotoLibrary
    $PhotoLobrarySectionContent.PhotoLibraryPath = $EnvironmentParams.PhotoLibraryLocalPath
    $PhotoLobrarySectionContent.PhotoLibraryDeletedFolder = $EnvironmentParams.PhotoLibraryLocalDeleteFolder
    $PhotoLobrarySectionContent.PositionStackApiKey = $EnvironmentParams.PositionStackApiKey

    $ConnectionStringsSectionContent = $AppSettingsContent.ConnectionStrings
    $ConnectionStringsSectionContent."photo-db" = $EnvironmentParams.PhotoDbConnectionString
    $ConnectionStringsSectionContent."photo-identity-db" = $EnvironmentParams.IdentityDbConnectionString

    $AppSettingsContent | ConvertTo-Json -Depth 100 | Out-File $AppSettingsFileName -Encoding utf8
}

function IncreaseBackendVersion {
    $BackendProjectFile = ".\backend\PhotoLibraryBackend\PhotoLibraryBackend.csproj"

    $newContent = foreach($line in Get-Content $BackendProjectFile) {
        if($line -match '<Version>'){
            # Get Version
            $VersionFound = $line -match '\d{1,}\.\d{1,}\.\d{1,}\.\d{1,}'
            if ($VersionFound) {
                $VersionString = $matches[0]
                $Versions = $VersionString -split '\.'
                $BuildNumber = [int32]$Versions[3]
                $NewBuildNumber = $BuildNumber + 1
                $NewVersion = "$($Versions[0]).$($Versions[1]).$($Versions[2]).$($NewBuildNumber)"
                Write-Host "New backend version: $($NewVersion)"
                $line -replace $VersionString, $NewVersion
            } else {
                $line
            }
        }
        else {
            # leave the line unmodified
            $line
        }
    }
    
    $newContent | Set-Content -Path $BackendProjectFile
}

function ChangeFrontendApplicationSettings {
    param (
        [EnvironmentParameters]$EnvironmentParams,
        [string]$EnvFileName,
        [bool]$IsProduction
    )
    # 'REACT_APP_BACKEND_URL'
    $newContent = foreach($line in Get-Content $EnvFileName) {
        if($line -match 'REACT_APP_BACKEND_URL'){
            $ParamKeyValue = $line -split '='
            if ($ParamKeyValue.Length -eq 2) {
                $NewUrl
                if ($IsProduction) {
                    $NewUrl = $EnvironmentParams.PhotoLibraryBackendUrl
                }
                else {
                    $NewUrl = $EnvironmentParams.PhotoLibraryBackendDevUrl
                }
                Write-Host "Frontend environment. Replacing backend URL from '$($ParamKeyValue[1])' to '$($NewUrl)'"
                $line -replace $ParamKeyValue[1], $NewUrl
            } else {
                $line
            }
        }
        else {
            # leave the line unmodified
            $line
        }
    }

    $newContent | Set-Content -Path $EnvFileName
}

function IncreaseFrontendVersion {
    $FrontendPackageJsonFile = '.\frontend\package.json'

    $newContent = foreach($line in Get-Content $FrontendPackageJsonFile) {
        if($line -match '"version":'){
            # Get Version
            $VersionFound = $line -match '\d{1,}\.\d{1,}\.\d{1,}\.\d{1,}'
            if ($VersionFound) {
                $VersionString = $matches[0]
                $Versions = $VersionString -split '\.'
                $BuildNumber = [int32]$Versions[3]
                $NewBuildNumber = $BuildNumber + 1
                $NewVersion = "$($Versions[0]).$($Versions[1]).$($Versions[2]).$($NewBuildNumber)"
                Write-Host "New frontend version: $($NewVersion)"
                $line -replace $VersionString, $NewVersion
            } else {
                $line
            }
        }
        else {
            # leave the line unmodified
            $line
        }
    }
    
    $newContent | Set-Content -Path $FrontendPackageJsonFile
}

#--Start--
Clear-Host
# 1. Read parameters
Write-Host "Reading deployment parameters..."
$EnvironmentParams = ReadParameters
$EnvironmentParams

#2. Change backend settings to prod settings
ChangeBackendApplicationSettings $EnvironmentParams '.\backend\PhotoLibraryBackend\appsettings.json'

#3. Increase backend version
IncreaseBackendVersion

# #4. Build backend
# Write-Host '--------------'
# dotnet publish .\backend\PhotoLibraryBackend\PhotoLibraryBackend.csproj -c release -r linux-arm64 --self-contained
# Write-Host '--------------'
# Write-Host

# #5. Copy backend to Raspberry
# Write-Host 'Please open ssh session and run `sudo systemctl stop photo-library.service`'
# Read-Host 'Hit Enter when ready'
# #TODO: Use $EnvironmentParams.RaspberryAddress
# scp -r ./backend/PhotoLibraryBackend/bin/release/net8.0/linux-arm64/publish/* pi@192.168.0.65:/home/pi/projects/photo-library/backend

#6. Update frontend .env file
ChangeFrontendApplicationSettings $EnvironmentParams '.\frontend\.env' $true

#7. Increase frontend version
IncreaseFrontendVersion

# #8. Build frontend
# Set-Location frontend
# npm run build
# Set-Location ..

# #9. Copy frontend to Raspberry
# scp -r ./frontend/build/* pi@192.168.0.65:/home/pi/projects/photo-library/frontend

#10. Revert frontend Env
ChangeFrontendApplicationSettings $EnvironmentParams '.\frontend\.env' $false

#11. Revert Backend appsettings
git checkout -- backend/PhotoLibraryBackend/appsettings.json

#12. Commit new versions to git
git add backend\PhotoLibraryBackend\PhotoLibraryBackend.csproj
git add frontend\package.json
git commit -a -m "Automatic rollout a new version to raspberry Pi"
git push