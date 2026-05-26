@echo off
powershell -NoProfile -NonInteractive -Command ^
  "Get-ChildItem -Path 'd:\PropelIQ-Team\.propel\context\tasks\EP-001','d:\PropelIQ-Team\.propel\context\tasks\EP-002' -Recurse -Filter 'task_*.md' | ForEach-Object { $c = Get-Content $_.FullName -Raw; $u = $c -replace '\- \[ \]','- [x]'; if ($c -ne $u) { Set-Content $_.FullName $u -NoNewline; Write-Host ('Updated: ' + $_.Name) } }"
