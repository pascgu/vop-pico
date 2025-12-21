@echo off
set SOURCE=..\vop-core\dist\ui
set DEST=.\VopPico.App\Resources\Raw\ui

if not exist "%SOURCE%" (
    echo Directory not found: %SOURCE%
    exit /b 1
)

if exist "%DEST%" (
    rmdir /s /q "%DEST%"
)

mkdir "%DEST%"

xcopy /E /Y "%SOURCE%\*" "%DEST%"

echo ReactFlow assets copied successfully to %DEST%
