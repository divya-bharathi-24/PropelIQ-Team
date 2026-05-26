@echo off
set DOTNET_ROOT=C:\Users\DivyaBharathiRavikum\.dotnet
set PATH=C:\Users\DivyaBharathiRavikum\.dotnet;%PATH%

cd /d d:\PropelIQ-Team\src\backend

echo === DOTNET VERSION ===
dotnet --version

echo === DOTNET RESTORE ===
dotnet restore HealthPlatform.sln 2>&1

echo === DOTNET BUILD ===
dotnet build HealthPlatform.sln --no-restore 2>&1

echo === DONE ===
