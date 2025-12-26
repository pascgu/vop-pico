using VopPico.App.Pages;
using static VopPico.App.Pages.PicoPage;

namespace VopPico.App.Services
{
    public class PicoJsInterface
    {
        private readonly PicoPage _picoPage;
        private HybridWebView Hwv { get => _picoPage.HybridWebView; }
        public int Count { get; set; }

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

        public async Task checkCS2JS()
        {
            try
            {
                await _picoPage.Dispatcher.DispatchAsync(async () =>
                {
                    await Hwv.EvaluateJavaScriptAsync("console.warn('>> CS2JS #1 : log a warning in console')");
                    Hwv.SendRawMessage($">> CS2JS #2 nb={Count++} : C# send a raw message");
                    await Hwv.EvaluateJavaScriptAsync("window.HybridWebView.SendRawMessage('>> CS2JS #3 : JS send a raw message');");
                    await Hwv.EvaluateJavaScriptAsync("window.logMessage('>> CS2JS #4 : call logMessage from C# with eval', 'warning');");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    await Hwv.InvokeJavaScriptAsync<object>(
                        "window.logMessage",
                        null, // use it if no return type (void)
                        [">> CS2JS #5 : call logMessage from C# with invoke", "error"],
                        [VopHybridJSContext.Default.String, VopHybridJSContext.Default.String]
                    );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CS2JS: {ex.Message}");
            }
        }

        public async void onHwvRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
        {
            await _picoPage.DisplayAlert("Raw Message Received in C#", e.Message, "OK");
        }

        public class SyncReturn
        {
            public string? Message { get; set; }
            public int Value { get; set; }
        }

        // VopFlow methods
        public async Task LoadVopFlow(string vopFlowData)
        {
            // Implement the logic to load a VopFlow
            Console.WriteLine($"Loading VopFlow: {vopFlowData}");
            await Task.CompletedTask;
        }

        public async Task SaveVopFlow()
        {
            // Implement the logic to save the current VopFlow
            Console.WriteLine("Saving VopFlow");
            await Task.CompletedTask;
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
    }
}
