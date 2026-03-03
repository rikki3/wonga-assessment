@echo off
setlocal EnableDelayedExpansion

cd /d "%~dp0.."

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker CLI is not installed or not on PATH.
    exit /b 1
)

docker info >nul 2>nul
if errorlevel 1 (
    set "DOCKER_DESKTOP_EXE="
    call :find_docker_desktop

    if not defined DOCKER_DESKTOP_EXE (
        echo [ERROR] Docker Desktop executable not found.
        echo         Install Docker Desktop or start the Docker daemon manually.
        exit /b 1
    )

    call :confirm_open_docker
    if errorlevel 1 (
        echo [INFO] Docker Desktop start was cancelled.
        exit /b 1
    )

    echo [INFO] Starting Docker Desktop...
    start "" "!DOCKER_DESKTOP_EXE!" >nul 2>nul

    call :wait_for_docker
    if errorlevel 1 exit /b 1
)

if not exist "docker-compose.yml" (
    echo [ERROR] docker-compose.yml was not found in project root.
    exit /b 1
)

docker compose config >nul
if errorlevel 1 (
    echo [ERROR] docker compose configuration validation failed.
    exit /b 1
)

docker compose up --build %*
exit /b %errorlevel%

:find_docker_desktop
if exist "%ProgramFiles%\Docker\Docker\Docker Desktop.exe" (
    set "DOCKER_DESKTOP_EXE=%ProgramFiles%\Docker\Docker\Docker Desktop.exe"
)
if not defined DOCKER_DESKTOP_EXE if exist "%LocalAppData%\Programs\Docker\Docker\Docker Desktop.exe" (
    set "DOCKER_DESKTOP_EXE=%LocalAppData%\Programs\Docker\Docker\Docker Desktop.exe"
)
if not defined DOCKER_DESKTOP_EXE if exist "%ProgramW6432%\Docker\Docker\Docker Desktop.exe" (
    set "DOCKER_DESKTOP_EXE=%ProgramW6432%\Docker\Docker\Docker Desktop.exe"
)
exit /b 0

:confirm_open_docker
set "PROMPT_RESULT="
for /f %%I in ('powershell -NoProfile -Command "Add-Type -AssemblyName System.Windows.Forms; $result=[System.Windows.Forms.MessageBox]::Show('Docker is not running. Open Docker Desktop now?','Docker Desktop',[System.Windows.Forms.MessageBoxButtons]::YesNo,[System.Windows.Forms.MessageBoxIcon]::Question); if ($result -eq [System.Windows.Forms.DialogResult]::Yes) { 'YES' } else { 'NO' }"') do set "PROMPT_RESULT=%%I"
if /I "!PROMPT_RESULT!"=="YES" exit /b 0
exit /b 1

:wait_for_docker
echo [INFO] Waiting for Docker daemon to become ready...
for /L %%A in (1,1,60) do (
    docker info >nul 2>nul
    if not errorlevel 1 (
        echo [INFO] Docker daemon is running.
        exit /b 0
    )
    timeout /t 2 /nobreak >nul
)
echo [ERROR] Docker daemon did not become ready in time.
echo         Open Docker Desktop manually and try again.
exit /b 1
