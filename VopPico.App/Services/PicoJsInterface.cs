using VopPico.App.Pages;
using VopPico.App.Models;
using VopPico.App.Interfaces;

namespace VopPico.App.Services;
public class PicoJsInterface
{
    private readonly PicoPage _picoPage;
    private HybridWebView Hwv { get => _picoPage.HybridWebView; }
    public int Count { get; set; }
    private List<string> previousPorts = new List<string>();
    private string _receiveBuffer = string.Empty;
    private CancellationTokenSource? _cts;
    private ISerialConnection? _serialConnection;

    public PicoJsInterface(PicoPage picoPage)
    {
        _picoPage = picoPage;
    }

    public async Task SendCodeToDeviceAsync(string code)
    {
        try
        {
            // Handle the code sent from JavaScript
            Console.WriteLine($"Sending code to Pico: {code}");

            // Ajouter un retour à la ligne pour les commandes Python si ce n'est pas déjà fait
            if (!code.EndsWith("\r\n") && !code.EndsWith("\n"))
            {
                code += "\r\n";
            }

            // Envoyer le code au Pico via la connexion série
            if (_serialConnection?.IsOpen ?? false)
            {
                _serialConnection.Write(code);
                Console.WriteLine($"Sent to Pico: {code}");
            }
            else
            {
                Console.WriteLine("No serial connection available");
                await SendLogMessageAsync("No serial connection available", LogMessageType.error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending code to device: {ex.Message}");
            await SendLogMessageAsync($"Error sending code to device: {ex.Message}", LogMessageType.error);
        }
        await Task.CompletedTask;
    }

    public async Task<string> GetDeviceStatusAsync()
    {
        // Return the device status
        await Task.CompletedTask;
        return "Device is ready 2";
    }

    public async Task JSevalAsync()
    {
        try
        {
            await Hwv.Dispatcher.DispatchAsync(async () =>
            {
                await Hwv.EvaluateJavaScriptAsync("window.logMessage('JSeval: call logMessage from C# with eval');");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JSeval: {ex.Message}");
            await SendLogMessageAsync($"Error in JSeval: {ex.Message}", LogMessageType.error);
        }
    }

    public async Task JSinvokeAsync()
    {
        try
        {
            await Hwv.Dispatcher.DispatchAsync(async () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await Hwv.InvokeJavaScriptAsync<object>(
                    "window.logMessage",
                    null, // use it if no return type (void)
                    ["JSinvoke: call logMessage from C# with invoke", ""],
                    [VopHybridJSContext.Default.String, VopHybridJSContext.Default.String]
                );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JSinvoke: {ex.Message}");
            await SendLogMessageAsync($"Error in JSinvoke: {ex.Message}", LogMessageType.error);
        }
    }

    public async Task JSrawAsync()
    {
        try
        {
            await Hwv.Dispatcher.DispatchAsync(() =>
            {
                Hwv.SendRawMessage($"JSraw : C# send a raw message");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JSraw: {ex.Message}");
            await SendLogMessageAsync($"Error in JSraw: {ex.Message}", LogMessageType.error);
        }
    }

    public async void onHwvRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        try
        {
            await Hwv.Dispatcher.DispatchAsync(async () =>
            {
                await _picoPage.DisplayAlert("Raw Message Received in C#", e.Message, "OK");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in onHwvRawMessageReceived: {ex.Message}");
            await SendLogMessageAsync($"Error in onHwvRawMessageReceived: {ex.Message}", LogMessageType.error);
        }
    }

    public class SyncReturn
    {
        public string? Message { get; set; }
        public int Value { get; set; }
    }

    // VopFlow methods
    private bool ValidateVopFlow(string vopFlowJson, out VopFlow? vopFlow, out string errorMessage)
    {
        try
        {
            vopFlow = System.Text.Json.JsonSerializer.Deserialize<VopFlow>(vopFlowJson);
            if (vopFlow == null)
            {
                errorMessage = "Invalid VopFlow JSON";
                return false;
            }

            if (string.IsNullOrEmpty(vopFlow.version) ||
                string.IsNullOrEmpty(vopFlow.name) ||
                vopFlow.nodes == null ||
                vopFlow.edges == null ||
                vopFlow.metadata == null)
            {
                vopFlow = null;
                errorMessage = "Missing required fields in VopFlow";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
        catch (System.Text.Json.JsonException ex)
        {
            vopFlow = null;
            errorMessage = $"JSON parsing error: {ex.Message}";
            return false;
        }
    }

    public async Task<string> OnLoadingVopFlowAsync(string vopFlowJson)
    {
        if (!ValidateVopFlow(vopFlowJson, out VopFlow? vopFlow, out string errorMessage))
        {
            return errorMessage;
        }

        try
        {
            if (vopFlow == null)
            {
                return "Failed to deserialize VopFlow";
            }

            // Serialize the object to a JSON string with indentation
            string jsonString = System.Text.Json.JsonSerializer.Serialize(vopFlow, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("OnLoadingVopFlow called with data: " + jsonString);
            await Task.CompletedTask;
            return jsonString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnLoadingVopFlow: {ex.Message}");
            return $"Error processing VopFlow data while loading: {ex.Message}";
        }
    }

    public async Task<string> OnSavingVopFlowAsync(string vopFlowJson)
    {
        if (!ValidateVopFlow(vopFlowJson, out VopFlow? vopFlow, out string errorMessage))
        {
            return errorMessage;
        }

        try
        {
            if (vopFlow == null)
            {
                return "Failed to deserialize VopFlow";
            }

            // Serialize the object to a JSON string with indentation
            string jsonString = System.Text.Json.JsonSerializer.Serialize(vopFlow, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("OnSavingVopFlow called with data: " + jsonString);
            await Task.CompletedTask;
            return jsonString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnSavingVopFlow: {ex.Message}");
            return $"Error processing VopFlow data while saving: {ex.Message}";
        }
    }

    public async Task ExecuteVopFlowAsync()
    {
        // Implement the logic to execute the current VopFlow
        Console.WriteLine("Executing VopFlow");
        await Task.CompletedTask;
    }

    public async Task OnVopFlowExecutionErrorAsync(string error)
    {
        // Implement the logic to handle errors during VopFlow execution
        Console.WriteLine($"VopFlow execution error: {error}");
        await Task.CompletedTask;
    }

    public async Task<List<string>> ListSerialPortsAsync()
    {
        var currentPorts = new List<string>();
        var resultPorts = new List<string>();

        try
        {
            List<string> serial_port_names = SerialConnectionFactory.ListPorts();
            foreach (string port in serial_port_names)
            {
                var portDetails = "";
                if (previousPorts.Count > 0 && !previousPorts.Contains(port))
                    portDetails = " (new)";
                currentPorts.Add(port);
                resultPorts.Add($"{port}{portDetails}");
            }
            Console.WriteLine($"Serial ports: {string.Join(", ", resultPorts)}");
            if (resultPorts.Count == 0)
                await SendLogMessageAsync("No port found", LogMessageType.warning);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing serial ports: {ex.Message}");
            await SendLogMessageAsync($"Error listing serial ports: {ex.Message}", LogMessageType.error);
        }

        previousPorts = currentPorts;
        await Task.CompletedTask;
        return resultPorts;
    }

    public async Task<string> SelectSerialPortAsync(string portName)
    {
        try
        {
            Console.WriteLine($"Selected serial port: {portName}");

            // Close actual Serial connection if opened
            await CloseSerialConnectionAsync();

            if (string.IsNullOrEmpty(portName))
                return ""; // Return empty string if no port is selected

            // Create new Serial connection
            _serialConnection = SerialConnectionFactory.Create();
            _serialConnection.ConnectionCreated += OnSerialConnectionCreated;
            _serialConnection.Connect(portName);

            return portName;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting serial port: {ex.Message}");
            await SendLogMessageAsync($"Error selecting serial port: {ex.Message}", LogMessageType.error);
            await SendLogMessageAsync($"Stack trace: {ex.StackTrace}", LogMessageType.error);
            return "";
        }
    }

    private async void OnSerialConnectionCreated(object? sender, EventArgs e)
    {
        if (_serialConnection != null)
            _serialConnection.ConnectionCreated -= OnSerialConnectionCreated;
        try
        {
            Console.WriteLine("Serial connection created");
            StartSerialMonitoring();
            await SendLogMessageAsync($"Port connected: {_serialConnection?.PortName}");
            await SendCodeToDeviceAsync("print('Pico connected to VoP')");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnSerialConnectionCreated: {ex.Message}");
            await SendLogMessageAsync($"Error in OnSerialConnectionCreated: {ex.Message}", LogMessageType.error);
        }
    }

    public async Task CloseSerialConnectionAsync()
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            if (_serialConnection != null)
            {
                _serialConnection.ConnectionCreated -= OnSerialConnectionCreated;
                _serialConnection.Close();
                _serialConnection = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloseSerialConnection error : {ex.Message}");
            await SendLogMessageAsync($"CloseSerialConnection error : {ex.Message}", LogMessageType.warning);
        }
    }

    private void StartSerialMonitoring()
    {
        // Cancel any previous task
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    string? fragment = null;

                    if (_serialConnection != null && _serialConnection.IsOpen)
                    {
                        fragment = _serialConnection.Read();
                    }

                    if (!string.IsNullOrEmpty(fragment))
                    {
                        _receiveBuffer += fragment;

                        // Manage all complete lines in the buffer
                        while (_receiveBuffer.Contains("\n"))
                        {
                            int lineEndIndex = _receiveBuffer.IndexOf("\n");
                            string completeLine = _receiveBuffer.Substring(0, lineEndIndex + 1);
                            string cleanLine = completeLine.TrimEnd('\r', '\n');
                            if (!string.IsNullOrEmpty(cleanLine))
                            {
                                // send data to frontend
                                await SendDataReceivedAsync(cleanLine);
                            }

                            // Remove the processed line from the buffer
                            _receiveBuffer = _receiveBuffer.Substring(lineEndIndex + 1);
                        }

                        // Manage specific MicroPython REPL prompt (without \n)
                        if (_receiveBuffer == ">>> ")
                        {
                            await SendDataReceivedAsync(_receiveBuffer);
                            _receiveBuffer = string.Empty;
                        }
                    }

                    // Pause for processor to avoid overloading
                    await Task.Delay(20, token);
                }
            }
            catch (System.OperationCanceledException) { /* Normal stop */ }
            catch (Exception ex)
            {
                Console.WriteLine($"Error monitoring serial messages: {ex.Message}");
                await SendLogMessageAsync($"Error monitoring serial connection: {ex.Message}", LogMessageType.error);
                await SendLogMessageAsync($"Stack trace: {ex.StackTrace}", LogMessageType.error);
            }
        }, token);
    }

    private async Task SendDataReceivedAsync(string message, LogMessageType? type = LogMessageType.code)
    {
        try
        {
            await JsTools.InvokeJsAsync(Hwv, "window.vopHost.receiveDataFromDevice", message, type?.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data received: {ex.Message}");
        }
    }

    private async Task SendLogMessageAsync(string message, LogMessageType? type = null)
    {
        try
        {
            await JsTools.InvokeJsAsync(Hwv, "window.logMessage", message, type?.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log message: {ex.Message}");
        }
    }
}