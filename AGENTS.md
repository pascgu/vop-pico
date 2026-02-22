# AI Agents Guide - vop-pico

## Workspace Structure

**Two-repository setup:**

1. **vop-pico** (this repo): .NET MAUI application
   - `VopPico.App/`: Main application
   - `Platforms/`: Platform-specific implementations
   - `Services/`: Core services (PicoJsInterface, SerialConnection)
   - `Interfaces/`: Abstraction interfaces

2. **vop-core** (sibling directory): React UI
   - `ui/`: React TypeScript application
   - Builds to `dist/ui/` → copied to `vop-pico/VopPico.App/Resources/Raw/ui/`

**Build order:** vop-core → copy-frontend.bat → vop-pico

## Coding Conventions

### C#
- PascalCase: classes, interfaces, public members
- Async methods must end with `Async`
- Use interfaces for platform abstraction
- XML comments for public APIs

### TypeScript
- PascalCase: interfaces, types, components
- camelCase: variables, functions
- Strict typing required
- Use `async/await` for promises

## Architecture

**Patterns:**
- Factory pattern for platform-specific code (e.g., SerialConnectionFactory, ApplicationFactory)
- Interface-based design for cross-platform abstraction
- Event-driven communication between MAUI and React UI

**Key interfaces:**
- `ISerialConnection`: Platform-specific serial communication
- `IVopHost`: Bridge between MAUI WebView and React

## Build Commands

```bash
# Build UI
cd ../vop-core && npm run build

# Copy UI to MAUI
cd ../vop-pico && copy-frontend.bat

# Build/run MAUI app
dotnet build VopPico.App
dotnet run --project VopPico.App
```

## Task Management

**Plan files in `plans/`:**
- Create a new plan file for each feature: `YYYYMMDD_feature-name.md` (kebab-case)
- Use markdown checkboxes to track tasks:
  ```markdown
  - [ ] Task to do
  - [x] Completed task
  ```
- Update the same file as you progress (check off completed tasks)
- When feature is complete, move plan to `plans/completed/`
- Keep plans in git for history

**Branches and PRs:**
- For large features: create a dedicated feature branch
- Create PR when feature development is complete
- For small changes: work directly on current branch

## Agent Rules

1. **Architecture:**
   - Always use interfaces, not direct implementations
   - Follow factory pattern for platform dependencies
   - For simple platform-specific code: use static factories with `#if ANDROID`/`#if WINDOWS` (e.g., SerialConnectionFactory, ApplicationFactory)
   - For complex platform-specific code: use full interface + implementation pattern (e.g., ISerialConnection)

2. **Naming:**
   - Async methods MUST end with `Async`
   - Follow language conventions (PascalCase C#, camelCase TypeScript)

3. **Security:**
   - Respect `.claudeignore` - never read/display sensitive files
   - Common patterns: `.env`, credentials, API keys, SSH keys, `google-services.json`

## Additional Resources

- See `README.md` for project overview and setup
- See `vop-core/proj-notes/` for VoP project documentation
- See `plans/` for current and completed feature plans