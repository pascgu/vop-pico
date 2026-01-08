using System.Text.Json.Serialization;
using VopPico.App.Services;

namespace VopPico.App.Pages
{
    public partial class PicoPage : ContentPage
    {
        public PicoJsInterface picoJsInterface;

        public HybridWebView HybridWebView { get => hybridWebView; }

        public PicoPage()
        {
            InitializeComponent();
            picoJsInterface = new PicoJsInterface(this);
            HybridWebView.SetInvokeJavaScriptTarget(picoJsInterface);
        }

        [JsonSourceGenerationOptions(WriteIndented = true)]
        //[JsonSerializable(typeof(PicoJsInterface.ComputationResult))]
        [JsonSerializable(typeof(double))]
        [JsonSerializable(typeof(string))]
        [JsonSerializable(typeof(Dictionary<string, string>))]
        internal partial class VopHybridJSContext : JsonSerializerContext
        {
            // This type's attributes specify JSON serialization info to preserve type structure
            // for trimmed builds.
        }

        private void hybridWebView_RawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
        {
            picoJsInterface.onHwvRawMessageReceived(sender, e);
        }

        // Add methods to handle VopFlow operations
        public async Task LoadVopFlow(string vopFlowData)
        {
            await picoJsInterface.LoadVopFlow(vopFlowData);
        }

        public async Task ExecuteVopFlow()
        {
            await picoJsInterface.ExecuteVopFlow();
        }

        public async Task OnVopFlowExecutionError(string error)
        {
            await picoJsInterface.OnVopFlowExecutionError(error);
        }
    }
}
