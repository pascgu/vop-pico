using Android.Content;
using Android.Hardware.Usb;
using System.Text;

namespace VopPico.App.Platforms.Android.Usb
{
    public class AndroidSerial
    {
        private readonly UsbManager _usbManager;
        private readonly UsbDeviceConnection _usbConnection;
        private readonly UsbEndpoint _readEndpoint;
        private readonly UsbEndpoint _writeEndpoint;
        private readonly UsbDevice _usbDevice;

        public int ReadTimeout { get; set; } = 1000; // Default timeout in milliseconds
        public int WriteTimeout { get; set; } = 1000; // Default timeout in milliseconds

        public AndroidSerial(Context context, UsbDevice device)
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
        }

        public string? Read()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _usbConnection.BulkTransfer(_readEndpoint, buffer, buffer.Length, ReadTimeout);
            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            return null;
        }

        public void Write(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            int bytesWritten = _usbConnection.BulkTransfer(_writeEndpoint, buffer, buffer.Length, WriteTimeout);
            if (bytesWritten < 0)
            {
                throw new Exception("Failed to write to USB device");
            }
        }

        public void Close()
        {
            _usbConnection.Close();
        }

        public void SetDtrRts(bool dtr, bool rts)
        {
            // control value : bit 0 = DTR, bit 1 = RTS
            int value = (dtr ? 1 : 0) | (rts ? 2 : 0);

            // Request SET_CONTROL_LINE_STATE (0x22)
            // RequestType: 0x21 (Host to Device | Class | Interface)
            _usbConnection.ControlTransfer(
                (UsbAddressing)0x21, // Request type
                0x22,                 // SET_CONTROL_LINE_STATE
                value,                // Value (DTR/RTS)
                0,                    // Index (Interface 0)
                null,                 // no data buffer
                0,                    // Length 0
                1000                  // Timeout
            );
        }
    }
}