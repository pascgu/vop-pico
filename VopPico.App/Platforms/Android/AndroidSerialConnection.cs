
using VopPico.App.Interfaces;
using Android.Hardware.Usb;
using Android.Content;
using System.Text;

namespace VopPico.App;

public class AndroidSerialConnection : ISerialConnection
{
    private UsbManager? _usbManager;
    private UsbDeviceConnection? _usbConnection;
    private UsbEndpoint? _readEndpoint;
    private UsbEndpoint? _writeEndpoint;
    private UsbDevice? _usbDevice;

    public int ReadTimeout { get; set; } = 1000; // Default timeout in milliseconds
    public int WriteTimeout { get; set; } = 1000; // Default timeout in milliseconds

    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    public string? PortName { get; private set; } = null;

    public event EventHandler<EventArgs>? ConnectionCreated;

    public static List<string> ListPorts()
    {
        var usbManager = (UsbManager?)Android.App.Application.Context.GetSystemService(Context.UsbService);
        var deviceList = usbManager?.DeviceList;
        return deviceList?.Values.Select(d => d.DeviceName).ToList() ?? new List<string>();
    }

    public AndroidSerialConnection()
    {
    }

    public void Connect(string portName)
    {
        PortName = null; // will be defined at the end of InitSerialConnection
        var usbManager = (UsbManager?)Android.App.Application.Context.GetSystemService(Context.UsbService);
        var device = usbManager?.DeviceList?.Values.FirstOrDefault(d => d.DeviceName == portName);

        if (device != null)
        {
            MainActivity? mainActivity = Platform.CurrentActivity as MainActivity;
            MainActivity.UsbPermissionGranted -= OnUsbPermissionGranted;
            MainActivity.UsbPermissionGranted += OnUsbPermissionGranted;

            mainActivity?.RequestUsbPermission(device);
        }
    }

    private void OnUsbPermissionGranted(object? sender, UsbDevice device)
    {
        MainActivity.UsbPermissionGranted -= OnUsbPermissionGranted;
        try
        {
            InitSerialConnection(Android.App.Application.Context, device);
            _isOpen = true;
            ConnectionCreated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating serial connection: {ex.Message}");
        }
    }


    private void InitSerialConnection(Context context, UsbDevice device)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        UsbManager? usbManager = (UsbManager?)context.GetSystemService(Context.UsbService);
        if (usbManager == null)
            throw new Exception("USB service not available");
        _usbManager = usbManager;

        UsbDeviceConnection? usbConnection = _usbManager.OpenDevice(device);
        if (usbConnection == null)
            throw new Exception("Failed to open USB device");
        _usbConnection = usbConnection;

        if (device.InterfaceCount < 1)
            throw new Exception("No interfaces found");

        if (!_usbManager.HasPermission(device))
        {
            throw new Exception("No permission for USB device");
        }

        UsbInterface? dataInterface = null;
        for (int i = 0; i < device.InterfaceCount; i++)
        {
            var inter = device.GetInterface(i);
            // Looking for the interface with 2 endpoints (often interface 1 on Pico)
            if (inter.EndpointCount >= 2) 
            {
                dataInterface = inter;
                break;
            }
        }

        UsbInterface usbInterface = dataInterface ?? device.GetInterface(0);

        if (!_usbConnection.ClaimInterface(usbInterface, true))
        {
            _usbConnection.Close();
            throw new Exception("Failed to claim interface");
        }
        
        SetDtrRts(true,true); // activate DTR and RTS

        UsbEndpoint? readEndpoint = null;
        UsbEndpoint? writeEndpoint = null;
        for (int i = 0; i < usbInterface.EndpointCount; i++)
        {
            UsbEndpoint? ep = usbInterface.GetEndpoint(i);
            if (ep?.Type == UsbAddressing.XferBulk)
            {
                if (ep.Direction == UsbAddressing.In)
                    readEndpoint = ep;
                else
                    writeEndpoint = ep;
            }
        }
        if (readEndpoint == null)
            throw new Exception("No read endpoint found");
        if (writeEndpoint == null)
            throw new Exception("No write endpoint found");
        _readEndpoint = readEndpoint;
        _writeEndpoint = writeEndpoint;

        _usbDevice = device;
        PortName = device.DeviceName;
    }
    
    public void SetDtrRts(bool dtr, bool rts)
    {
        // control value : bit 0 = DTR, bit 1 = RTS
        int value = (dtr ? 1 : 0) | (rts ? 2 : 0);

        // Request SET_CONTROL_LINE_STATE (0x22)
        // RequestType: 0x21 (Host to Device | Class | Interface)
        _usbConnection?.ControlTransfer(
            (UsbAddressing)0x21, // Request type
            0x22,                 // SET_CONTROL_LINE_STATE
            value,                // Value (DTR/RTS)
            0,                    // Index (Interface 0)
            null,                 // no data buffer
            0,                    // Length 0
            1000                  // Timeout
        );
    }


    public void Write(string data)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Connection is not open");
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        int bytesWritten = _usbConnection!.BulkTransfer(_writeEndpoint, buffer, buffer.Length, WriteTimeout);
        if (bytesWritten < 0)
        {
            throw new Exception("Failed to write to USB device");
        }
    }

    public string? Read()
    {
        if (!IsOpen)
            throw new InvalidOperationException("Connection is not open");
        byte[] buffer = new byte[1024];
        int bytesRead = _usbConnection!.BulkTransfer(_readEndpoint, buffer, buffer.Length, ReadTimeout);
        if (bytesRead > 0)
        {
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        return null;
    }

    public void Close()
    {
        _usbConnection?.Close();
        _usbConnection = null;
        _isOpen = false;
    }
}
