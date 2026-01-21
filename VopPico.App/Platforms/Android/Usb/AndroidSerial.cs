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

            UsbDeviceConnection? usbConnection = _usbManager.OpenDevice(_usbDevice);
            if (usbConnection == null)
                throw new Exception("Failed to open USB device");
            _usbConnection = usbConnection;

            _usbDevice = device;
            if (_usbDevice.InterfaceCount < 1)
                throw new Exception("No interfaces found");

            UsbInterface usbInterface = _usbDevice.GetInterface(0);
            if (usbInterface == null || usbInterface.EndpointCount < 1)
                throw new Exception("No endpoints found");

            if (!_usbManager.HasPermission(_usbDevice))
            {
                throw new Exception("No permission for USB device");
            }

            if (!_usbConnection.ClaimInterface(usbInterface, true))
            {
                _usbConnection.Close();
                throw new Exception("Failed to claim interface");
            }
            
            SetDtrRts(true,true);

            UsbEndpoint? readEndpoint = usbInterface.GetEndpoint(0);
            UsbEndpoint? writeEndpoint = usbInterface.GetEndpoint(1);
            if (readEndpoint == null)
                throw new Exception("No read endpoint found");
            if (writeEndpoint == null)
                throw new Exception("No write endpoint found");
            _readEndpoint = readEndpoint;
            _writeEndpoint = writeEndpoint;
        }

        public string? Read()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _usbConnection.BulkTransfer(_readEndpoint, buffer, buffer.Length, 1000);
            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            return null;
        }

        public void Write(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            int bytesWritten = _usbConnection.BulkTransfer(_writeEndpoint, buffer, buffer.Length, 1000);
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