using System.IO.Ports;
using VopPico.App.Interfaces;

namespace VopPico.App;

public class WindowsSerialConnection : ISerialConnection
{
    private SerialPort? _serialPort;
    private bool _isOpen = false;
    private HybridWebView? _hwv;

    public event EventHandler<EventArgs>? ConnectionCreated;
    public bool IsOpen => _serialPort?.IsOpen ?? _isOpen;
    public string? PortName => _serialPort?.PortName;
    public HybridWebView? Hwv
    {
        get => _hwv;
        set => _hwv = value;
    }
    
    public static List<string> ListPorts()
    {
        return SerialPort.GetPortNames().ToList();
    }

    public WindowsSerialConnection(HybridWebView? hwv = null)
    {
        Hwv = hwv;
    }

    public void Connect(string portName)
    {
        _serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        _serialPort.Open();
        _serialPort.Encoding = System.Text.Encoding.UTF8;
        _serialPort.Handshake = Handshake.None;
        _serialPort.DtrEnable = true;
        _serialPort.RtsEnable = true;
        _serialPort.NewLine = "\r\n";
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;
        _serialPort.ReceivedBytesThreshold = 1;

        _isOpen = true;
        ConnectionCreated?.Invoke(this, EventArgs.Empty);
    }

    public void Write(string code)
    {
        if (!_isOpen)
            throw new InvalidOperationException("Serial port is not open.");
        _serialPort?.Write(code);
    }

    public string? Read()
    {
        if (!_isOpen)
            throw new InvalidOperationException("Serial port is not open.");
        return _serialPort?.ReadExisting();
    }

    public void Close()
    {
        _serialPort?.Close();
        _serialPort = null;
        _isOpen = false;
    }
}
