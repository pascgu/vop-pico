using VopPico.App.Pages;
using static VopPico.App.Pages.PicoPage;
using VopPico.App.Models;
using System.IO.Ports;

namespace VopPico.App.Services
{
    public class PicoJsInterface
    {
        private readonly PicoPage _picoPage;
        private HybridWebView Hwv { get => _picoPage.HybridWebView; }
        public int Count { get; set; }
        private List<string> previousPorts = new List<string>();

        public PicoJsInterface(PicoPage picoPage)
        {
            _picoPage = picoPage;
        }

        public void SendCodeToDevice(string code)
        {
            // Handle the code sent from JavaScript
            Console.WriteLine($"Received code: {code}");
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
                foreach (var port in SerialPort.GetPortNames())
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
                // Implement logic to select the serial port
                Console.WriteLine($"Selected serial port: {portName}");
                await Task.CompletedTask;
                return portName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting serial port: {ex.Message}");
                return "";
            }
        }
    }
}
