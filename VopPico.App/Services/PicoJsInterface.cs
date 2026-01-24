using VopPico.App.Pages;
using static VopPico.App.Pages.PicoPage;
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

    public void SendCodeToDevice(string code)
    {
        try
        {
            // Handle the code sent from JavaScript
            Console.WriteLine($"Received code: {code}");

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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending code to device: {ex.Message}");
        }
    }

    public async Task<string> GetDeviceStatus()
    {
        // Return the device status
        await Task.CompletedTask;
        return "Device is ready 2";
    }

    public async Task JSeval()
    {
        try
        {
            await _picoPage.Dispatcher.DispatchAsync(async () =>
            {
                await Hwv.EvaluateJavaScriptAsync("window.logMessage('JSeval: call logMessage from C# with eval');");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JSeval: {ex.Message}");
        }
    }

    public async Task JSinvoke()
    {
        try
        {
            await _picoPage.Dispatcher.DispatchAsync(async () =>
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
        }
    }

    public async Task JSraw()
    {
        try
        {
            await _picoPage.Dispatcher.DispatchAsync(() =>
            {
                Hwv.SendRawMessage($"JSraw : C# send a raw message");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JSraw: {ex.Message}");
        }
    }

    public async void onHwvRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        try
        {
            await _picoPage.Dispatcher.DispatchAsync(async () =>
            {
                await _picoPage.DisplayAlert("Raw Message Received in C#", e.Message, "OK");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in onHwvRawMessageReceived: {ex.Message}");
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

    public async Task<string> OnLoadingVopFlow(string vopFlowJson)
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
            return $"Error processing VopFlow data: {ex.Message}";
        }
    }

    public async Task<string> OnSavingVopFlow(string vopFlowJson)
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
            return $"Error processing VopFlow data: {ex.Message}";
        }
    }

    public async Task ExecuteVopFlow()
    {
        // Implement the logic to execute the current VopFlow
        Console.WriteLine("Executing VopFlow");
        await Task.CompletedTask;
    }

    public async Task OnVopFlowExecutionError(string error)
    {
        // Implement the logic to handle errors during VopFlow execution
        Console.WriteLine($"VopFlow execution error: {error}");
        await Task.CompletedTask;
    }

    public async Task<List<string>> ListSerialPorts()
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing serial ports: {ex.Message}");
        }

        previousPorts = currentPorts;
        await Task.CompletedTask;
        return resultPorts;
    }

    public async Task<string> SelectSerialPort(string portName)
    {
        try
        {
            Console.WriteLine($"Selected serial port: {portName}");

            // Close actual Serial connection if opened
            CloseSerialConnection();

            // Create new Serial connection
            _serialConnection = SerialConnectionFactory.Create();
            _serialConnection.ConnectionCreated += OnSerialConnectionCreated;
            _serialConnection.Connect(portName);

            await Task.CompletedTask;
            return portName;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting serial port: {ex.Message}");
            SendLogMessage($"Error selecting serial port: {ex.Message}", LogMessageType.error);
            SendLogMessage($"Stack trace: {ex.StackTrace}", LogMessageType.error);
            return "";
        }
    }

    private void OnSerialConnectionCreated(object? sender, EventArgs e)
    {
        try
        {
            StartSerialMonitoring();
            SendCodeToDevice("print('Pico connected to VoP')");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnSerialConnectionCreated: {ex.Message}");
            SendLogMessage($"Error in OnSerialConnectionCreated: {ex.Message}", LogMessageType.error);
        }
    }

    public void CloseSerialConnection()
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _serialConnection?.Close();
            _serialConnection = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloseSerialConnection error : {ex.Message}");
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
                                SendDataReceived(cleanLine);
                            }

                            // Remove the processed line from the buffer
                            _receiveBuffer = _receiveBuffer.Substring(lineEndIndex + 1);
                        }

                        // Manage specific MicroPython REPL prompt (without \n)
                        if (_receiveBuffer == ">>> ")
                        {
                            SendDataReceived(_receiveBuffer);
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
                SendLogMessage($"Error monitoring serial connection: {ex.Message}", LogMessageType.error);
                SendLogMessage($"Stack trace: {ex.StackTrace}", LogMessageType.error);
            }
        }, token);
    }

    private void SendDataReceived(string message, LogMessageType? type = LogMessageType.code)
    {
        try
        {
            _picoPage.Dispatcher.DispatchAsync(async () =>
            {
                Console.WriteLine($"Sending data received to JS: {message}");
                string encodedMessage = message;
#if !ANDROID
                encodedMessage = HttpUtility.JavaScriptStringEncode(message);
#endif
                try
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    await Hwv.InvokeJavaScriptAsync<object>(
                        "window.vopHost.receiveDataFromDevice",
                        null, // use it if no return type (void)
                        [encodedMessage, type?.ToString()],
                        [VopHybridJSContext.Default.String, VopHybridJSContext.Default.String]
                    );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data received InvokeJavaScriptAsync: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data received: {ex.Message}");
        }
    }

    private void SendLogMessage(string message, LogMessageType? type = null)
    {
        try
        {
            _picoPage.Dispatcher.DispatchAsync(async () =>
            {
                Console.WriteLine($"Sending log message to JS: {message}");
                string encodedMessage = message;
#if !ANDROID
                encodedMessage = HttpUtility.JavaScriptStringEncode(message);
#endif
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await Hwv.InvokeJavaScriptAsync<object>(
                    "window.logMessage",
                    null, // use it if no return type (void)
                    [encodedMessage, type?.ToString()],
                    [VopHybridJSContext.Default.String, VopHybridJSContext.Default.String]
                );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log message: {ex.Message}");
        }
    }
}