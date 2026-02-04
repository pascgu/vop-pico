using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;

namespace VopPico.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private UsbReceiver _usbReceiver = new UsbReceiver();
    private UsbManager? _usbManager;
    private PendingIntent? _permissionIntent;
    private const int VENDOR_ID_RASPBERRY_PICO = 0x2E8A; // Vendor ID for Raspberry Pi Pico
    public const string ACTION_USB_PERMISSION = "android.hardware.usb.action.USB_PERMISSION";

    public static event EventHandler<UsbDevice>? UsbPermissionGranted;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Register the BroadcastReceiver
        var filter = new IntentFilter();
        filter.AddAction(ACTION_USB_PERMISSION);
        if (OperatingSystem.IsAndroidVersionAtLeast(34))
        {
            // Use RECEIVER_EXPORTED because the system must be able to send the permission intent to this receiver.
            RegisterReceiver(_usbReceiver, filter, ReceiverFlags.Exported);
        }
        else
            RegisterReceiver(_usbReceiver, filter);

        // Get the UsbManager service
        _usbManager = (UsbManager?)GetSystemService(UsbService);
        // Create a PendingIntent for USB permission requests
        PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            flags = PendingIntentFlags.Mutable;
        }
        Intent intent = new Intent(ACTION_USB_PERMISSION);
        intent.SetPackage(PackageName); // explicit intent to only sent to this app
        _permissionIntent = PendingIntent.GetBroadcast(this, 0, intent, flags);
    }

    public void RequestUsbPermission(UsbDevice device)
    {
        if (_usbManager == null || _permissionIntent == null)
        {
            Console.WriteLine("USB Manager or Permission Intent is null");
            return;
        }
        if (_usbManager!.HasPermission(device))
        {
            // Si on a déjà la permission, on peut notifier le service directement
            NotifyPermissionGranted(device);
        }
        else
        {
            _usbManager.RequestPermission(device, _permissionIntent);
        }
    }

    private void NotifyPermissionGranted(UsbDevice device)
    {
        UsbPermissionGranted?.Invoke(this, device);
    }

    public static void NotifyPermissionGrantedStatic(UsbDevice device)
    {
        UsbPermissionGranted?.Invoke(null, device);
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnregisterReceiver(_usbReceiver);
        //_usbReceiver?.Dispose(); // usefull ?
    }
}

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { MainActivity.ACTION_USB_PERMISSION })]
[MetaData(MainActivity.ACTION_USB_PERMISSION, Resource = "@xml/device_filter")]
public class UsbReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent == null)
            return;
        else if (intent.Action == MainActivity.ACTION_USB_PERMISSION)
        {
            Console.WriteLine($"USB_PERMISSION intent received");

            bool granted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
            if (granted)
            {
                // Handle USB permission
                string extra_device = UsbManager.ExtraDevice;
                UsbDevice? device;
                if (OperatingSystem.IsAndroidVersionAtLeast(33))
                {
                    device = intent.GetParcelableExtra(extra_device, Java.Lang.Class.FromType(typeof(UsbDevice))) as UsbDevice;
                }
                else
                {
                    device = intent.GetParcelableExtra(extra_device) as UsbDevice;
                }
                if (device != null)
                {
                    Console.WriteLine($"Device USB detected: {device.DeviceName}");
                    // Add here the logic to open the serial port once the permission is obtained
                    
                    // Launch the static event to notify the C# service that the permission is granted
                    MainActivity.NotifyPermissionGrantedStatic(device);
                }
            }
        }
    }
}


