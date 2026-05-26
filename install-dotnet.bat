@echo off
echo === DOWNLOADING dotnet-install.ps1 ===
curl -L -o d:\PropelIQ-Team\dotnet-install.ps1 https://dot.net/v1/dotnet-install.ps1 2>&1

echo === INSTALLING .NET 8 SDK ===
powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -File d:\PropelIQ-Team\dotnet-install.ps1 -Channel 8.0 -InstallDir "%USERPROFILE%\.dotnet" 2>&1

echo === VERIFYING ===
"%USERPROFILE%\.dotnet\dotnet.exe" --version 2>&1
"%USERPROFILE%\.dotnet\dotnet.exe" --list-sdks 2>&1

echo === DONE ===
