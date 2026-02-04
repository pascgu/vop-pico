using VopPico.App;

namespace VopPico.App.Interfaces;

public static class ApplicationFactory
{
    /// <summary>
    /// Quits the application using platform-specific methods.
    /// Handles WebView2 cleanup issues on Windows and uses appropriate quit methods for each platform.
    /// </summary>
    public static void Quit()
    {
        try
        {
            // We use Dispatcher to ensure we don't interrupt WebView2 while it's processing a JS call
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // A very short delay (50ms) is enough to let the JS-C# bridge release
                await Task.Delay(50);
#if WINDOWS
                // For WinUI 3 / Windows :
                Microsoft.UI.Xaml.Window? window = Microsoft.Maui.Controls.Application.Current?.Windows[0].Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                window?.Close();
#else
                // For Android and others :
                Microsoft.Maui.Controls.Application.Current?.Quit();
#endif
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ApplicationFactory.Quit: {ex.Message}");
            // Fallback to environment exit if all else fails
            System.Environment.Exit(0);
        }
    }

    /// <summary>
    /// Gets the current application instance in a platform-specific way.
    /// </summary>
    /// <returns>The current application instance or null if not available</returns>
    public static object? GetCurrentApplication()
    {
#if ANDROID
        return Microsoft.Maui.Controls.Application.Current;
#elif WINDOWS
        return Microsoft.UI.Xaml.Application.Current;
#else
        return Application.Current;
#endif
    }
}