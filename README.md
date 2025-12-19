# vop-pico
A MAUI application with HybridWebView to program and control a Raspberry Pi Pico from an Android phone, Windows, and Linux via USB OTG.

# Development

## Build

- Android: `dotnet build -f net9.0-android`
- Windows: `dotnet build -f net9.0-windows10.0.19041.0`
- All: `dotnet build`

## Run

- Android: `dotnet run -f net9.0-android`
- Windows: `dotnet run -f net9.0-windows10.0.19041.0`

## copy frontend from vop-core

in vop-pico dir: `copy-frontend.bat`