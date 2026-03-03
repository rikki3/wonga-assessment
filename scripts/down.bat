@echo off
setlocal

cd /d "%~dp0.."

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker CLI is not installed or not on PATH.
    exit /b 1
)

if not exist "docker-compose.yml" (
    echo [ERROR] docker-compose.yml was not found in project root.
    exit /b 1
)

docker compose down %*
exit /b %errorlevel%
