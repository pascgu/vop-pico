# AI Agents Guide - vop-pico Project

## 📋 Introduction

Welcome to the **vop-pico** project! This guide is designed to help AI agents understand the project structure, coding conventions, and specific processes to follow.

**vop-pico** is a MAUI (Multi-platform App UI) application for Raspberry Pi Pico that allows controlling and interacting with electronic devices through a modern user interface.

## 🗂️ Workspace Structure

This project follows a special workspace structure with two main repositories:

### 1. **vop-pico** (Main Repository)
- **Location**: `vop-pico/`
- **Content**:
  - Main MAUI application (`VopPico.App`)
  - Platform-specific code (Android, Windows, etc.)
  - Integration with Raspberry Pi Pico hardware
  - USB serial communication services

### 2. **vop-core** (Dependent Repository)
- **Location**: `vop-core/` (same level as vop-pico)
- **Content**:
  - React user interface (`ui/`)
  - Shared components and business logic
  - TypeScript types and interfaces

### 🔗 Relationship between repositories
1. **Build process**:
   - Build `vop-core` first
   - Copy content from `vop-core/dist/ui` to `vop-pico/VopPico.App/Resources/Raw/ui`
   - Then build `vop-pico`

2. **Key files**:
   - `copy-frontend.bat`: Script to copy UI files
   - `VopPico.App/Resources/Raw/ui/`: Destination for interface files

## 💻 Coding Conventions

### C# (.NET MAUI)

```csharp
// C# convention example
public interface ISerialConnection
{
    event EventHandler<EventArgs> ConnectionCreated;
    static abstract List<string> ListPorts();
    bool IsOpen { get; }
    void Connect(string portName);
    void Write(string data);
    string? Read();
    void Close();
}
```

- **Naming**: PascalCase for classes/interfaces, camelCase for variables/methods
- **Async**: All asynchronous methods must end with `Async`
- **Documentation**: XML comments for public methods
- **Error handling**: Use `try-catch` with appropriate logging

### TypeScript (React UI)

```typescript
// TypeScript convention example
interface IVopHost {
    sendMessage(type: string, data: any): Promise<void>;
    onMessage(callback: (type: string, data: any) => void): void;
    logMessage(message: string, type?: LogMessageType): void;
}
```

- **Naming**: PascalCase for interfaces/types, camelCase for variables/functions
- **Typing**: Strict typing required for all variables and functions
- **Promises**: Use `async/await` for asynchronous handling
- **Structure**: React components in `components/`, hooks in `hooks/`

## 🏗️ Technical Architecture

### Main Components

1. **VopPico.App**:
   - `Services/`: Main services (PicoJsInterface, SerialConnection, etc.)
   - `Interfaces/`: Interfaces for platform abstraction
   - `Platforms/`: Platform-specific implementations
   - `Pages/`: MAUI application pages

2. **vop-core/ui**:
   - `components/`: Reusable React components
   - `interfaces/`: TypeScript interface definitions
   - `hooks/`: Custom React hooks

### Architecture Patterns

- **Factory Pattern**: Used for creating serial connections
- **Interface-based Design**: Platform dependency abstraction
- **Event-driven**: Communication between components via events

## 🔨 Build Process

### Complete Steps

1. **Preparation**:
   ```bash
   cd ../vop-core
   npm install
   npm run build
   ```

2. **Copy UI files**:
   ```bash
   cd ../vop-pico
   copy-frontend.bat
   ```

3. **Build MAUI application**:
   ```bash
   dotnet build VopPico.App
   ```

4. **Execution**:
   ```bash
   dotnet run --project VopPico.App
   ```

## 📝 Task Management

### Using `wip/tasks.md`

The `wip/tasks.md` file is used to track ongoing tasks:

1. **Format**:
   ```markdown
   ## Current Tasks

   - [x] Completed task
   - [ ] Task in progress
   - [ ] New task to do
   ```

2. **Best practices**:
   - Update the file after each completed task
   - Add technical details if necessary
   - Use checklists for complex tasks

3. **Example**:
   ```markdown
   ## USB Serial Communication

   - [x] Implement AndroidSerialConnection
   - [x] Create ISerialConnection interface
   - [ ] Test communication with Pico
   - [ ] Optimize error handling
   ```

## 🤖 Specific Rules for AI Agents

### Important Instructions

1. **Architecture compliance**:
   - Always use interfaces rather than direct implementations
   - Follow factory pattern for platform dependencies

2. **Naming conventions**:
   - **Async**: All asynchronous methods must end with `Async`
   - **Avoid**: Method names like `GetData` if the method is asynchronous → `GetDataAsync`

3. **File management**:
   - Place new files in appropriate directories
   - Respect existing structure
   - Use `.cs` extension for C# code and `.ts`/`.tsx` for TypeScript

4. **Platform-specific code organization**:
   - For simple platform-specific functionality, use static factory classes in the `Interfaces` directory (like `SerialConnectionFactory.cs` or `ApplicationFactory.cs`)
   - Use `#if ANDROID`/`#if WINDOWS` directives within these factory classes rather than creating separate interfaces and implementations (only if the functionnality is simple, for more complex functionality, use the factory pattern with interfaces and implementations, like the ISerialConnection example)
   - This approach keeps platform-specific code centralized and follows the existing pattern in the codebase

5. **Documentation**:
   - Add comments for complex code
   - Update existing documentation if necessary
   - Use code examples in comments

6. **Testing**:
   - Verify code compiles before committing
   - Test features on target platforms
   - Document use cases

7. **Security and Sensitive Files**:
   - **IMPORTANT**: The `.claudeignore` file at the root of this project contains patterns for sensitive files (API keys, credentials, environment variables, etc.)
   - **All AI agents** (not just Claude Code) MUST respect this file and NEVER read, analyze, or display the contents of files matching these patterns
   - Common sensitive files include:
     - `.env` and environment variable files
     - Cloud provider credentials (AWS, Azure, GCP)
     - SSH/GPG keys (`.pem`, `.key`, `id_rsa`, etc.)
     - Database credentials
     - API keys and tokens
     - Mobile app configuration files (`google-services.json`, `GoogleService-Info.plist`)
   - If you need to reference a sensitive file pattern, refer to `.claudeignore` for the complete list
   - Never suggest committing or sharing these files

## 📚 Useful Resources

### Documentation

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [React Documentation](https://react.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/handbook/)

### Tools

- **Visual Studio Code**: Recommended editor
- **Visual Studio 2022**: For MAUI development
- **Android Studio**: For Android development and phone emulators
- **Node.js**: For React/TypeScript development

### Useful Commands

```bash
# Build core project
cd vop-core && npm run build

# Copy UI files
copy-frontend.bat

# Build MAUI application
dotnet build VopPico.App

# Clean generated files
dotnet clean
```

## 🎯 Best Practices

1. **Code Review**:
   - Always review code before committing
   - Check naming conventions
   - Ensure code is well documented

2. **Error Handling**:
   - Use clear error messages
   - Log errors for debugging
   - Handle exceptions appropriately

3. **Performance**:
   - Avoid blocking operations in UI thread
   - Use async for long operations
   - Optimize network calls

## 📬 Contact

For any questions or issues, consult the following files:
- `README.md`: General project information
- `wip/tasks.md`: Current tasks and planning
- `vop-core/proj-notes/`: Additional notes and documentation on the VoP project (of which VopPico is part)

---

**Last update**: 02/19/2026
**Version**: 1.1