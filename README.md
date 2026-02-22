# vop-pico

**vop-pico** is a cross-platform MAUI (Multi-platform App UI) application designed to program and control Raspberry Pi Pico microcontrollers from Android phones, Windows, and Linux devices via USB OTG. It features a modern React-based user interface embedded in a HybridWebView, enabling seamless interaction with electronic devices through USB serial communication.

## 📋 Features

- **Cross-platform**: Runs on Android, Windows, and Linux
- **USB Serial Communication**: Direct communication with Raspberry Pi Pico via USB
- **Modern UI**: React TypeScript interface with real-time updates
- **Bi-directional Communication**: MAUI ↔ React via JavaScript bridge
- **Platform Abstraction**: Clean interface-based architecture for platform-specific code

## 🏗️ Project Structure

This project is part of a two-repository workspace:

### vop-pico (this repository)
- **.NET MAUI Application**: Main application framework
- **Platform-specific implementations**: Android, Windows serial communication
- **Services**: PicoJsInterface, SerialConnection management
- **HybridWebView Integration**: Hosts the React UI

### vop-core (sibling repository)
- **React TypeScript UI**: User interface components
- **Shared Business Logic**: Common functionality
- **Type Definitions**: TypeScript interfaces

The React UI from `vop-core` is built and copied into `vop-pico/VopPico.App/Resources/Raw/ui/` before building the MAUI application.

## 🛠️ Prerequisites

### Required Tools

- **[.NET 9 SDK](https://dotnet.microsoft.com/download)**: For MAUI development
- **[Node.js](https://nodejs.org/)**: For building the React UI (vop-core)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** (recommended for MAUI) or **[Visual Studio Code](https://code.visualstudio.com/)**
- **[Android Studio](https://developer.android.com/studio)**: For Android development and emulators (if targeting Android)

### Platform-specific Requirements

- **Android**: Android SDK (installed via Visual Studio or Android Studio)
- **Windows**: Windows 10/11 SDK

## 🚀 Getting Started

### 1. Clone Both Repositories

```bash
# Clone vop-pico
git clone <vop-pico-url>

# Clone vop-core in the same parent directory
git clone <vop-core-url>
```

Your directory structure should look like:
```
parent-directory/
├── vop-pico/
└── vop-core/
```

### 2. Build the React UI (vop-core)

```bash
cd vop-core
npm install
npm run build
```

### 3. Copy UI to MAUI Project

```bash
cd ../vop-pico
copy-frontend.bat
```

### 4. Build the MAUI Application

```bash
# Build for all platforms
dotnet build

# Build for specific platform
dotnet build -f net9.0-android
dotnet build -f net9.0-windows10.0.19041.0
```

### 5. Run the Application

```bash
# Run on Android
dotnet run -f net9.0-android

# Run on Windows
dotnet run -f net9.0-windows10.0.19041.0
```

## 💻 Development Workflow

1. **Make changes to React UI** in `vop-core/ui/`
2. **Build vop-core**: `npm run build` in vop-core directory
3. **Copy UI files**: Run `copy-frontend.bat` in vop-pico directory
4. **Build MAUI app**: `dotnet build VopPico.App`
5. **Test**: Run the application on your target platform

### Quick Development Commands

```bash
# Complete rebuild workflow
cd vop-core && npm run build && cd ../vop-pico && copy-frontend.bat && dotnet build

# Clean build artifacts
dotnet clean
```

## 📚 Documentation

### Official Documentation
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [React Documentation](https://react.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/handbook/)

### Project Documentation
- **[AGENTS.md](AGENTS.md)**: Technical guide for AI agents (architecture, conventions, rules)
- **[plans/](plans/)**: Feature development plans and task tracking
- **[vop-core/proj-notes/](../vop-core/proj-notes/)**: Additional VoP project documentation

## 🎯 Best Practices

### Code Quality
- **Code Review**: Always review code before committing
- **Naming Conventions**: Follow C# (PascalCase) and TypeScript (camelCase) conventions
- **Documentation**: Add XML comments for public C# APIs, JSDoc for TypeScript
- **Async Methods**: All async C# methods must end with `Async`

### Error Handling
- Use clear, descriptive error messages
- Log errors appropriately for debugging
- Handle exceptions at appropriate levels
- Avoid silent failures

### Performance
- Avoid blocking operations on the UI thread
- Use `async/await` for long-running operations
- Optimize network and serial communication calls
- Profile before optimizing

## 🤝 Contributing

### Task Management
Feature development is tracked using plan files in the `plans/` directory. See [AGENTS.md](AGENTS.md) for details on the planning workflow.

### For AI Agents
If you're an AI agent working on this project, please refer to [AGENTS.md](AGENTS.md) for specific technical guidelines, architecture patterns, and rules to follow.

## 📬 Support

- **Issues**: Check existing issues or documentation first
- **Additional Notes**: See `vop-core/proj-notes/` for detailed VoP project information
- **Task Tracking**: Current development plans are in `plans/`

## 📄 License

[TODO]
