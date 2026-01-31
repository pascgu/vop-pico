# VopPico Development Tasks - Next Steps

## Additional Tasks
1. ✅ Implement PicoJsInterface and update the SetInvokeJavaScriptTarget object to point to this object, move associated functions from PicoPage.
2. ✅ Replace all occurrences of workflow with vopflow.
3. ✅ Change 1 test CS2JS button to 4 distinct buttons and retest / make it work on my Android phone.
4. ✅ Implement saveVopFlow. This should download a .json file. It should pass through a method style onSavingVopFlow on the backend (to be added in IVopHost) which currently does nothing in voppico but could serve as an entry point. This function should take the .json as a parameter and return it as is for now.
5. ✅ Implement loadVopFlow which allows uploading a .json.
6. ✅ Create a vopFlow (the .json) test to execute a basic program on the pico (make an LED blink).

## Phase 4: USB OTG & Execution Pipeline
*Objective: Convert the VopFlow JSON file into MicroPython commands and execute them sequentially via USB.*

1. ✅ Implement serial port listing and selection.
2. ✅ Monitor and display messages sent by the Pico on the serial port and display them with logMessage.
3. ✅ logMessage issue when sending multiple messages at once, Python-side error case
4. ✅ if working, rewrite platform-agnostic class/functions (decorator?) and remove #if ANDROID as much as possible
5. ✅ handle encodedMessage by platform, apparently different across platforms, so create interface and platform-specific implementation. => no finally create a common static JsTools class that keep #if ANDROID but only for that file
6. if only 1 device found in the list, connect automatically
7. add a button to quit properly the MAUI application (check it on both windows and android)
8. on Android, can we use UsbReceiver and know when a device is connected/disconnected to update the device list?
9. when plugging in the pico, it asks to open the app directly, it would be good if it directly connects to the newly plugged-in device
10. Convert VopFlow to MicroPython commands.
11. Execute commands via USB.
12. Validate MicroPython code generation and sending to Pico.

## Phase 5: MAF Backend Implementation (Business Logic)
*Objective: Set up the execution engine and tracing.*

1. Install packages: `dotnet add package Microsoft.AgentFramework` and `dotnet add package OpenTelemetry`.
2. Create the **`MafWorkflowExecutor.cs`** service.
3. **Implement OpenTelemetry Tracing** in `OpenTelemetryService.cs` to collect traces.
4. **Map VoP JSON to MAF** (Agents/Transitions).
5. Add OpenTelemetry instrumentation to MAF nodes (emit traces).
6. **Implement the communication channel** (e.g., WebSocket or HTTP endpoint) for the Frontend to receive traces from `OpenTelemetryService.cs`.

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
