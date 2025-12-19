namespace VopPico.App.Pages;

public partial class PicoPage : ContentPage
{
    public PicoPage()
    {
        InitializeComponent();
        hybridWebView.SetInvokeJavaScriptTarget(this);
    }

    private async void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        await DisplayAlert("Raw Message Received", e.Message, "OK");
    }

    // private void OnRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    // {
    //     // Traiter les messages du frontend
    //     var message = e.Message;
    //     Console.WriteLine($"Message reçu: {message}");

    //     // Désérialiser et traiter
    //     //var data = JsonSerializer.Deserialize<WorkflowData>(message);
    //     //ProcessWorkflow(data);

    //     // InvokeJavaScriptAsync
    //     // EvaluateJavaScriptAsync
    // }

    //private void ProcessWorkflow(WorkflowData data)
    //{
    //    // Logique métier ici
    //}

}
