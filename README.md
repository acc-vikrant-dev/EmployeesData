# Employee Console CRUD — SonarQube Demo

A small .NET 8 C# console app that lists, views, creates, updates, and deletes employee records. Data is stored in a local JSON file; no database server or NuGet package is required.

## Features

- CRUD operations for employees (ID, name, department, email, salary)
- Three dummy employees are seeded automatically on first run
- Basic input validation
- JSON persistence
- Local SonarScanner for .NET script
- GitHub Actions workflow template for build and SonarQube analysis

## Run locally

Prerequisite: .NET 8 SDK.

```powershell
dotnet restore
dotnet run --project .\EmployeeConsoleSonarDemo.csproj
```

The application creates `Data\employees.json` under the application's output directory (`bin\Debug\net8.0\Data\employees.json`) on first run. The checked-in `Data\employees.json` is a sample seed file; runtime persistence is performed against the output-directory copy.

## SonarQube setup (dummy demo values)

1. Start a SonarQube Community Build instance and create a project.
2. Use project key `employee-console-demo` (or update the scripts/workflow to match your project).
3. Generate a token in SonarQube.
4. For local scanning, set your token in PowerShell:

```powershell
$env:SONAR_HOST_URL = "http://localhost:9000"
$env:SONAR_TOKEN = "<YOUR_LOCAL_SONARQUBE_TOKEN>"
.\scripts\scan-local.ps1
```

The URL and project key are demo defaults. Do not commit a real token to source control.

## GitHub Actions

The workflow is `.github/workflows/sonarqube.yml`. In GitHub, add repository Actions secrets:

- `SONAR_HOST_URL` — reachable URL of your SonarQube server
- `SONAR_TOKEN` — a token created for the project

The workflow uses the free SonarScanner for .NET tool. A GitHub-hosted runner cannot access `http://localhost:9000` on your personal PC. For a private/local SonarQube server, use a properly configured self-hosted runner or a server reachable by GitHub Actions.

This workflow scans pushes to `main` and supports manual runs. SonarQube Community Build does not provide paid-edition pull-request analysis. The sample workflow is a starting point and does not enforce a quality gate as a required GitHub check.

## Scan before local commit

Run `scripts\scan-local.ps1` before `git commit`. To enforce a local check, you can later add a Git pre-commit hook, but remember that local hooks can be bypassed; use CI checks for repository enforcement.

## Notes

- This is a demo, not a production employee database.
- JSON storage is appropriate for a small local demonstration; concurrent writes and multi-user access are not handled.
- Dummy employee records use example.com email addresses.
