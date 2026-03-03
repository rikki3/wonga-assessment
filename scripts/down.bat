@echo off
setlocal EnableDelayedExpansion

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
set "DOWN_EXIT=%errorlevel%"
if not "!DOWN_EXIT!"=="0" exit /b !DOWN_EXIT!

call :confirm_close_docker
if errorlevel 1 (
    echo [INFO] Docker Desktop will remain open.
    exit /b 0
)

echo [INFO] Closing Docker Desktop...
taskkill /IM "Docker Desktop.exe" /T /F >nul 2>nul
taskkill /IM "com.docker.backend.exe" /T /F >nul 2>nul
exit /b 0

:confirm_close_docker
set "PROMPT_RESULT="
for /f %%I in ('powershell -NoProfile -Command "Add-Type -AssemblyName System.Windows.Forms; $result=[System.Windows.Forms.MessageBox]::Show('Do you want to close Docker Desktop as well?','Docker Desktop',[System.Windows.Forms.MessageBoxButtons]::YesNo,[System.Windows.Forms.MessageBoxIcon]::Question); if ($result -eq [System.Windows.Forms.DialogResult]::Yes) { 'YES' } else { 'NO' }"') do set "PROMPT_RESULT=%%I"
if /I "!PROMPT_RESULT!"=="YES" exit /b 0
exit /b 1
