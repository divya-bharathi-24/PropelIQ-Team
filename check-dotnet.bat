@echo off
if exist "C:\Program Files\dotnet\dotnet.exe" (
    echo DOTNET EXISTS
    "C:\Program Files\dotnet\dotnet.exe" --version
    "C:\Program Files\dotnet\dotnet.exe" --list-sdks
) else (
    echo DOTNET NOT IN PROGRAM FILES
)
echo ---
dir "C:\Program Files\dotnet\sdk" 2>nul || echo NO SDK DIRECTORY
