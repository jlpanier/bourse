using Android.Bluetooth;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Views.Main
{
    public class BluetoothBroadcastReceiver : BroadcastReceiver
    {
        public event EventHandler<BluetoothScanModeEventArgs> BluetoothScanModeChanged;

        public event EventHandler<BluetoothDevice> BluetoothDeviceFound;

        public event EventHandler BluetoothDiscoveryFinished;

        public BluetoothBroadcastReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (action == BluetoothDevice.ActionFound)
            {
                BluetoothDeviceFound?.Invoke(this, (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice));
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                BluetoothDiscoveryFinished?.Invoke(this, EventArgs.Empty);
            }
            else if (action == BluetoothAdapter.ActionScanModeChanged)
            {
                BluetoothScanModeChanged?.Invoke(this, new BluetoothScanModeEventArgs()
                {
                    ScanMode = (ScanMode)intent.GetIntExtra(BluetoothAdapter.ExtraScanMode, -1),
                    ConnectionState = intent.GetStringExtra(BluetoothAdapter.ExtraConnectionState),
                    DiscoverableDuration = intent.GetStringExtra(BluetoothAdapter.ExtraDiscoverableDuration),
                    LocalName = intent.GetStringExtra(BluetoothAdapter.ExtraLocalName),
                    PreviousConnectionState = intent.GetStringExtra(BluetoothAdapter.ExtraPreviousConnectionState),
                    PreviousScanMode = intent.GetStringExtra(BluetoothAdapter.ExtraPreviousScanMode),
                    PreviousState = intent.GetStringExtra(BluetoothAdapter.ExtraPreviousState),
                    State = intent.GetStringExtra(BluetoothAdapter.ExtraState),
                });
            }
        }
    }
}

