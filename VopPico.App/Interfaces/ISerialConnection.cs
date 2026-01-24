namespace VopPico.App.Interfaces;

public interface ISerialConnection
{
    event EventHandler<EventArgs> ConnectionCreated;
    static abstract List<string> ListPorts();
    bool IsOpen { get; }
    void Connect(string portName);
    void Write(string data);
    string? Read();
    void Close();
}