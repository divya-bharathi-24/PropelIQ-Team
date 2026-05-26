@echo off
echo === DOTNET CHECK ===
where dotnet 2>nul
if errorlevel 1 (
    echo dotnet: NOT FOUND
    echo === INSTALLING .NET 8 SDK ===
    winget install Microsoft.DotNet.SDK.8 --accept-package-agreements --accept-source-agreements --silent
    echo === INSTALL COMPLETE - CHECK PATH ===
    set "PATH=%PATH%;C:\Program Files\dotnet"
    dotnet --version 2>nul || echo dotnet STILL NOT FOUND after install
) else (
    echo dotnet: FOUND
    dotnet --version
)
