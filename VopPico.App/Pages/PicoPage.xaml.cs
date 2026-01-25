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


        private void hybridWebView_RawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
        {
            picoJsInterface.onHwvRawMessageReceived(sender, e);
        }

        public async Task ExecuteVopFlowAsync()
        {
            await picoJsInterface.ExecuteVopFlowAsync();
        }

        public async Task OnVopFlowExecutionErrorAsync(string error)
        {
            await picoJsInterface.OnVopFlowExecutionErrorAsync(error);
        }
    }
}
