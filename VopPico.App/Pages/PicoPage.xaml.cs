using System.Text.Json.Serialization;
using Windows.ApplicationModel.VoiceCommands;

namespace VopPico.App.Pages;

public partial class PicoPage : ContentPage
{
    private int count = 0;

    public PicoPage()
    {
        InitializeComponent();
        hybridWebView.SetInvokeJavaScriptTarget(this);
    }
    
    public class SyncReturn
    {
        public string? Message { get; set; }
        public int Value { get; set; }
    }
    
    public class ComputationResult
    {
        public double result { get; set; }
        public string? operationName { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ComputationResult))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    internal partial class VopHybridJSContext : JsonSerializerContext
    {
        // This type's attributes specify JSON serialization info to preserve type structure
        // for trimmed builds.    
    }
    
    private async void hybridWebView_RawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        await DisplayAlert("PicoPage Raw Message Received", e.Message, "OK");
        //Dispatcher.Dispatch(() => editor.Text += Environment.NewLine + e.Message);
    }

    // public async Task<SyncReturn> DoAsyncWorkParamsReturn(int i, string s)
    // {
    //     await DisplayAlert("PicoPage message sending", $"DoAsyncWorkParamsReturn: {i}, {s}","OK");
    //     await Task.Delay(1000);
    //     return new SyncReturn
    //     {
    //         Message = "Hello from C#!" + s,
    //         Value = i
    //     };
    // }

    
    // public async Task DoAsyncWorkParams(int i, string s)
    // {
    //     await DisplayAlert("PicoPage message sending", $"DoAsyncWorkParams: {i}, {s}","OK");
    //     await Task.Delay(1000);
    // }

    
    public void SendCodeToDevice(string code)
    {
        // Handle the code sent from JavaScript
        Console.WriteLine($"Received code: {code}");
    }

    public async Task<string> GetDeviceStatus()
    {
        // Return the device status
        return "Device is ready 2";
    }

    public async Task checkCS2JS()
    {
        try
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await hybridWebView.EvaluateJavaScriptAsync("console.warn('>> CS2JS #1')");
                hybridWebView.SendRawMessage($">> CS2JS #2 nb={count++}");
                await hybridWebView.EvaluateJavaScriptAsync("window.vopHost.onRawMessageReceived('>> CS2JS #3');");
                await hybridWebView.EvaluateJavaScriptAsync("window.logMessage('>> CS2JS #4', 'warning');");
                await hybridWebView.InvokeJavaScriptAsync<object>(
                    "window.logMessage",
                    null,
                    [">> CS2JS #5", "error"],
                    [VopHybridJSContext.Default.String, VopHybridJSContext.Default.String]
                    );
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CS2JS: {ex.Message}");
        }
    }
}
