using VopPico.App;

namespace VopPico.App.Interfaces;

public static class SerialConnectionFactory
{
    public static ISerialConnection Create()
    {
#if ANDROID
        return new AndroidSerialConnection();
#elif WINDOWS
        return new WindowsSerialConnection();
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