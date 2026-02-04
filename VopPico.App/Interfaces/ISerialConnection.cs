namespace VopPico.App.Interfaces;

public interface ISerialConnection
{
    event EventHandler<EventArgs> ConnectionCreated;
    static abstract List<string> ListPorts();
    bool IsOpen { get; }
    string? PortName { get; }
    HybridWebView? Hwv { get; set; }
    void Connect(string portName);
    void Write(string data);
    string? Read();
    void Close();
}