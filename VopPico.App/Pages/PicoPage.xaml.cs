namespace VopPico.App.Pages;

public partial class PicoPage : ContentPage
{
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

    public string GetDeviceStatus()
    {
        // Return the device status
        return "Device is ready 1";
    }
}
