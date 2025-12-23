# VopPico Development Tasks - Optimized Plan

## Phase 1: Development Environment Setup
*Objective: Install the native build environment.*
1. **Install .NET 9 SDK** (native Windows, not WSL).
2. **Install Android Studio** (for SDK, ADB tools, and emulators).
3. Install MAUI workloads: `dotnet workload install maui` (includes Android).
4. Install VS Code extensions: C#, .NET MAUI, C# Dev Kit, GitLens.
5. Validate the Android environment (environment variables, emulator, JDK).

## Phase 2: MAUI Project Skeletons
*Objective: Create the MAUI skeleton to integrate the different components.*
1. Create the .NET 9 MAUI solution: `dotnet new maui -n VopPico.App -f net9.0-android`
2. Add `.AddHybridWebView()` in `MauiProgram.cs`.
3. Define the service structure (empty C# files):
   * `src/VopPico.App/Pages/PicoPage.xaml` (HybridWebView Container)
   * `src/VopPico.App/Services/PicoJsInterface.cs` (Implements `IVopHost` from vop-core)
    * `src/VopPico.App/Services/MafWorkflowExecutor.cs`
    * `src/VopPico.App/Services/OpenTelemetryService.cs`

## Phase 3: Frontend Integration & Basic JS-C# Bridge
*Objective: Display the editor and test the simplest JSON exchange.*
1. Copy ReactFlow frontend from `../vop-core/dist/ui` into the MAUI resources folder: `VopPico.App/Resources/Raw/`
2. Configure HybridWebView to load index.html and initialize the JS-C# bridge.
3. **Implement a test function in `PicoJsInterface.cs`** (e.g., `SendJson(string json)`) and call it from JS to validate communication.

## Phase 4: MAF Backend Implementation (Business Logic)
*Objective: Set up the execution engine and tracing.*
1. Install packages: `dotnet add package Microsoft.AgentFramework` and `dotnet add package OpenTelemetry`.
2. Create the **`MafWorkflowExecutor.cs`** service.
3. **Implement OpenTelemetry Tracing** in `OpenTelemetryService.cs` to collect traces.
4. **Map VoP JSON to MAF** (Agents/Transitions).
5. Add OpenTelemetry instrumentation to MAF nodes (emit traces).
6. **Implement the communication channel** (e.g., WebSocket or HTTP endpoint) for the Frontend to receive traces from `OpenTelemetryService.cs`.

## Phase 5: USB OTG & Execution Pipeline
*Objective: Close the execution loop on the Pico.*
1. Create and implement the **USB OTG** service (Android-specific logic).
2. Refine the execution pipeline:
   * UI (JSON) → **PicoJsInterface** (C#) → **MafWorkflowExecutor** (MAF node execution & tracing) → **MicroPython Generator** (Final conversion) → **USB OTG Service** (Send to Pico).
3. Validate MicroPython code generation and sending to Pico.

## Phase 6: Visual Debugging Integration (Slow Motion Mode)
*Objective: Consume traces to animate the UI.*
1. In the frontend (ReactFlow), implement a **WebSocket/HTTP client** to connect to the trace channel from Phase 4.
2. Develop OpenTelemetry event listeners (NodeStarted/NodeCompleted/DataFlow).
3. Implement node animation (color/marker) based on received events (the "Slow Motion Mode").
4. Add debug controls to the UI (Pause, Step, Speed).

## Phase 7: Testing & Validation
1. Integration tests for the complete pipeline (JSON → Pico).
2. Validation of trace flow and visual debug animation.
3. Performance tests on Android.

## Phase 8: Advanced Features
1. Add WiFi/MQTT support.
2. Implement visualization of Pico sensor data.
3. Add logic for automatic node connection assistance.
