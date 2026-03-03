@echo off
setlocal

cd /d "%~dp0.."

where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERROR] .NET SDK is not installed or not on PATH.
    exit /b 1
)

dotnet test backend\Wonga.Api.Tests\Wonga.Api.Tests.csproj %*
exit /b %errorlevel%
