$ErrorActionPreference = "Stop"

# Demo defaults. Override these environment variables for your own SonarQube instance.
if (-not $env:SONAR_HOST_URL) { $env:SONAR_HOST_URL = "http://localhost:9000" }
if (-not $env:SONAR_TOKEN) {
    Write-Error "Set SONAR_TOKEN first: `$env:SONAR_TOKEN = '<your-local-sonarqube-token>'"
}

dotnet tool install --global dotnet-sonarscanner 2>$null
if ($LASTEXITCODE -ne 0) {
    dotnet tool update --global dotnet-sonarscanner
    if ($LASTEXITCODE -ne 0) { throw "Could not install/update dotnet-sonarscanner." }
}

dotnet sonarscanner begin `
  /k:"employee-console-demo" `
  /d:sonar.host.url="$env:SONAR_HOST_URL" `
  /d:sonar.token="$env:SONAR_TOKEN"
if ($LASTEXITCODE -ne 0) { throw "SonarScanner begin failed." }

dotnet restore .\EmployeeConsoleSonarDemo.csproj
if ($LASTEXITCODE -ne 0) { throw "Restore failed." }

dotnet build .\EmployeeConsoleSonarDemo.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

dotnet sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
if ($LASTEXITCODE -ne 0) { throw "SonarScanner analysis failed." }
