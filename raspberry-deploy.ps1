class EnvironmentParameters {
    [string]$RaspberryAddress
    [string]$RaspberryPwd
    [string]$KeycloakAuthServer
    [string]$KeycloakRealm
    [string]$KeycloakResource
    [string]$KeycloakClientId
    [string]$KeycloakRedirectUrl
    [string]$PhotoLibraryLocalPath
    [string]$PhotoLibraryLocalDeleteFolder
    [string]$PhotoDbConnectionString
    [string]$PhotoLibraryBackendUrl
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
            'KEYCLOAK_AUTH_SERVER' { 
                $envParametersObject.KeycloakAuthServer = $keyValue[1]
                continue
            }
            'KEYCLOAK_REALM' { 
                $envParametersObject.KeycloakRealm = $keyValue[1]
                continue
            }
            'KEYCLOAK_RESOURCE' { 
                $envParametersObject.KeycloakResource = $keyValue[1]
                continue
            }
            'KEYCLOAK_CLIENT_ID' { 
                $envParametersObject.KeycloakClientId = $keyValue[1]
                continue
            }
            'KEYCLOAK_REDIRECT_URL' { 
                $envParametersObject.KeycloakRedirectUrl = $keyValue[1]
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
            'PHOTO_LIBRARY_BACKEND_URL' { 
                $envParametersObject.PhotoLibraryBackendUrl = $keyValue[1]
                continue
            }
        }
    }

    return $envParametersObject
}

Clear-Host
# Read parameters
Write-Host "Reading deployment parameters..."
$EnvironmentParams = ReadParameters
$EnvironmentParams