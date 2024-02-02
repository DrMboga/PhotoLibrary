class EnvironmentParameters {
    [string]$RaspberryAddress
    [string]$RaspberryPwd
    [string]$PhotoLibraryLocalPath
    [string]$PhotoLibraryLocalDeleteFolder
    [string]$PhotoDbConnectionString
    [string]$IdentityDbConnectionString
    [string]$PhotoLibraryBackendUrl
    [string]$PositionStackApiKey
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
            'RASPBERRY_PWD' { 
                $envParametersObject.RaspberryPwd = $keyValue[1]
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

#--Start--
Clear-Host
# Read parameters
Write-Host "Reading deployment parameters..."
$EnvironmentParams = ReadParameters
$EnvironmentParams

#Read .\backend\PhotoLibraryBackend\appsettings.json
ChangeBackendApplicationSettings $EnvironmentParams ".\backend\PhotoLibraryBackend\appsettings.json"