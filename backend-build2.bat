@echo off
set DOTNET_ROOT=C:\Users\DivyaBharathiRavikum\.dotnet
set PATH=C:\Users\DivyaBharathiRavikum\.dotnet;%PATH%
set DOTNET_CLI_TELEMETRY_OPTOUT=1

cd /d d:\PropelIQ-Team\src\backend

echo === DOTNET RESTORE ===
dotnet restore HealthPlatform.sln 2>&1

echo === DOTNET BUILD ===
dotnet build HealthPlatform.sln --no-restore 2>&1

echo === DONE ===
