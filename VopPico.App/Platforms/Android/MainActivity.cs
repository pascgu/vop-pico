using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using VopPico.App.Platforms.Android.Usb;

namespace VopPico.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private UsbReceiver _usbReceiver = new UsbReceiver();
    private UsbManager? _usbManager;
    private PendingIntent? _permissionIntent;
    private const int VENDOR_ID_RASPBERRY_PICO = 0x2E8A; // Vendor ID for Raspberry Pi Pico
    public const string ACTION_USB_PERMISSION = "android.hardware.usb.action.USB_PERMISSION";
    //public const string ACTION_USB_PERMISSION = "com.companyname.voppico.app.USB_PERMISSION";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Register the BroadcastReceiver
        var filter = new IntentFilter(UsbManager.ActionUsbDeviceAttached);
        filter.AddAction(UsbManager.ActionUsbDeviceDetached);
        filter.AddAction(ACTION_USB_PERMISSION);
        RegisterReceiver(_usbReceiver, filter);

        // Get the UsbManager service
        _usbManager = (UsbManager?)GetSystemService(UsbService);
        // Create a PendingIntent for USB permission requests
        _permissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
    }

    private bool HasOrRequestUsbPermission(UsbDevice device)
    {
        if (_usbManager == null || _permissionIntent == null)
        {
            Console.WriteLine("USB Manager or Permission Intent is null");
            return false;
        }
        if (_usbManager.HasPermission(device))
            return true;
        _usbManager.RequestPermission(device, _permissionIntent);
        return _usbManager.HasPermission(device);
    }

    public List<UsbDevice> ListUsbDevices()
    {
        if (_usbManager == null)
        {
            Console.WriteLine("USB Manager is null");
            return new List<UsbDevice>();
        }
        var usbDevices = _usbManager.DeviceList?.Values
            .Where(d => d.VendorId == VENDOR_ID_RASPBERRY_PICO)
            .ToList();
        return usbDevices ?? new List<UsbDevice>();
    }
    
    public void ConnectToSerialDevice(UsbDevice device)
    {
        try
        {
            if (_usbManager == null)
            {
                Console.WriteLine("USB Manager is null");
                return;
            }

            if (!HasOrRequestUsbPermission(device))
                return;

            // Create an instance of AndroidSerial for communication
            var serial = new AndroidSerial(this, device);

            // Add logic here to read messages from Pico
            // For example, start a thread to monitor incoming data
            Console.WriteLine($"Connected to USB device: {device.DeviceName}");

            // Close the connection when done
            // serial.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to USB device: {ex.Message}");
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnregisterReceiver(_usbReceiver);
        //_usbReceiver?.Dispose(); // usefull ?
    }
}

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached })]
[IntentFilter(new[] { MainActivity.ACTION_USB_PERMISSION })]
[MetaData(MainActivity.ACTION_USB_PERMISSION, Resource = "@xml/device_filter")]
public class UsbReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent == null)
            return;
        if (intent.Action == UsbManager.ActionUsbDeviceAttached)
        {
            // Handle USB device attached
            Console.WriteLine($"USB device attached");
        }
        else if (intent.Action == UsbManager.ActionUsbDeviceDetached)
        {
            // Handle USB device detached
            Console.WriteLine($"USB device detached");
        }
        else if (intent.Action == MainActivity.ACTION_USB_PERMISSION)
        {
            Console.WriteLine($"USB_PERMISSION intent received");
            // Handle USB permission
            string extra_device = UsbManager.ExtraDevice;
            UsbDevice? device;
            if ((int)Build.VERSION.SdkInt >= 33)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                device = intent.GetParcelableExtra(extra_device, Java.Lang.Class.FromType(typeof(UsbDevice))) as UsbDevice;
#pragma warning restore CA1416
            }
            else
            {
#pragma warning disable CA1422 // Validate platform compatibility
                device = intent.GetParcelableExtra(extra_device) as UsbDevice;
#pragma warning restore CA1422
            }
            if (device != null)
            {
                Console.WriteLine($"Device USB detected: {device.DeviceName}");
                // Add here the logic to open the serial port once the permission is obtained
            }
        }
    }
}


