@echo off
echo === DOWNLOADING dotnet-install.ps1 via PowerShell ===
powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile 'd:\PropelIQ-Team\dotnet-install.ps1' -UseBasicParsing" 2>&1
if errorlevel 1 (
    echo Download FAILED
    exit /b 1
)
echo Download complete.

echo === INSTALLING .NET 8 SDK ===
powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -File d:\PropelIQ-Team\dotnet-install.ps1 -Channel 8.0 -InstallDir "%USERPROFILE%\.dotnet" 2>&1

echo === VERIFYING ===
"%USERPROFILE%\.dotnet\dotnet.exe" --version 2>&1
"%USERPROFILE%\.dotnet\dotnet.exe" --list-sdks 2>&1

echo === DONE ===
