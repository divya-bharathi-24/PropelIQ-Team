@echo off
cd /d d:\PropelIQ-Team\src\frontend

set NG_CLI_ANALYTICS=false

echo === STATE CHECK ===
if exist "node_modules" (echo node_modules: EXISTS) else (echo node_modules: MISSING)
if exist "package-lock.json" (echo package-lock.json: EXISTS) else (echo package-lock.json: MISSING)

echo === NPM INSTALL ===
call npm install --no-audit --no-fund 2>&1

echo === NPM BUILD ===
call npm run build 2>&1

echo === DONE ===
