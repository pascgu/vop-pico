# VopPico Development Tasks - Next Steps

## Additional Tasks
1. ✅ Implémenter PicoJsInterface et mettre à jour l'objet SetInvokeJavaScriptTarget pour pointer vers cet objet, déplacer les fonctions associées de PicoPage.
2. ✅ Remplacer toutes les occurrences à workflow par vopflow.
2. ✅ Changer 1 seul bouton test CS2JS en 4 distincts et retester / faire fonctionner sur mon tel android.
3. ✅ Implémenter saveVopFlow. Cela doit download un fichier .json. Il faut passer par une méthode style onSavingVopFlow côté backend (à ajouter dans IVopHost) qui ne fait rien pour l'instant dans voppico mais qui pourrai servir de point d'entrée. Cette fonction doit prendre en paramètre le .json et le retourner tel quel pour l'instant.
4. ✅ Implémenter loadVopFlow qui permet d'upload un .json.
5. ✅ Créer un vopFlow (le .json) de test pour exécuter un vrai programme de base sur le pico (faire clignoter une led).

## Phase 4: USB OTG & Execution Pipeline
*Objective: Convert the VopFlow JSON file into MicroPython commands and execute them sequentially via USB.*

1. ✅ Implement serial port listing and selection.
2. Monitor and display messages sent by the Pico on the serial port and display them with logMessage.
3. Convert VopFlow to MicroPython commands.
4. Execute commands via USB.
5. Validate MicroPython code generation and sending to Pico.

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
