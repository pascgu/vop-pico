using VopPico.App;

namespace VopPico.App.Interfaces;

public static class SerialConnectionFactory
{
    public static ISerialConnection Create(HybridWebView? hwv = null)
    {
#if ANDROID
        return new AndroidSerialConnection(hwv);
#elif WINDOWS
        return new WindowsSerialConnection(hwv);
#else
        throw new PlatformNotSupportedException();
#endif
    }

    public static List<string> ListPorts()
    {
#if ANDROID
        return AndroidSerialConnection.ListPorts();
#elif WINDOWS
        return WindowsSerialConnection.ListPorts();
#else
        throw new PlatformNotSupportedException();
#endif
    }
}