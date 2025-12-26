using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using static Bourse.Views.Main.BluetoothBaseActivity;

namespace Bourse.Views.Main
{
    internal class ListViewDevicesAdaptator : Bourse.Views.SimpleListAdapter<BluetoothDeviceItem>
    {
        public ListViewDevicesAdaptator(Activity context, IEnumerable<BluetoothDevice> devices) : base(context)
        {
            var items = new List<BluetoothDeviceItem>();
            foreach (BluetoothDevice device in devices)
            {
                items.Add(new BluetoothDeviceItem(device));
            }
            Reset(items);
        }

        public ListViewDevicesAdaptator(Activity context, IEnumerable<BluetoothDeviceItem> devices) : base(context)
        {
            Reset(devices);
        }

        public ListViewDevicesAdaptator(Activity context) : base(context)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_device, parent, false);
            }

            if (_animationBluetoothConnecting==null)
            {
                _animationBluetoothConnecting = (Android.Graphics.Drawables.AnimationDrawable)view.Resources.GetDrawable(Resource.Drawable.bluetooth_connecting);
            }
            ListViewDevicesHolder holder = view.Tag as ListViewDevicesHolder;

            if (holder == null)
            {
                holder = new ListViewDevicesHolder();
                holder.imgConnection = view.FindViewById<ImageView>(Resource.Id.imgConnection);
                holder.tvName = view.FindViewById<TextView>(Resource.Id.tvName);
                holder.tvAddress = view.FindViewById<TextView>(Resource.Id.tvAddress);
                holder.tvBondState = view.FindViewById<TextView>(Resource.Id.tvBondState);
                holder.tvType = view.FindViewById<TextView>(Resource.Id.tvType);
                view.Tag = holder;
            }

            BluetoothDeviceItem item = CurrentItems[position];
            holder.Position = position;
            switch (item.State)
            {
                case ServiceStates.Disconnected:
                    //if (_animationBluetoothConnecting!=null)
                    //{
                    //    _animationBluetoothConnecting.Stop();
                    //    _animationBluetoothConnecting.Dispose();
                    //}
                    holder.imgConnection.SetBackgroundResource(Resource.Drawable.bluetooth_disconnected);
                    holder.imgConnection.SetImageDrawable(null);
                    break;
                case ServiceStates.Listen:
                case ServiceStates.Connecting:

                    holder.imgConnection.SetBackgroundResource(Resource.Drawable.bluetooth_disconnected);
                    holder.imgConnection.SetImageDrawable(_animationBluetoothConnecting);
                    _animationBluetoothConnecting.Start();
                    break;
                case ServiceStates.Connected:
                    //if (_animationBluetoothConnecting!=null)
                    //{
                    //    _animationBluetoothConnecting.Stop();
                    //    _animationBluetoothConnecting.Dispose();
                    //}
                    holder.imgConnection.SetBackgroundResource(Resource.Drawable.bluetooth_connected);
                    holder.imgConnection.SetImageDrawable(null);
                    break;
            }
            holder.tvName.Text = item.Name;
            holder.tvAddress.Text = string.IsNullOrEmpty(item.Address) ? "None" : item.Address;
            holder.tvBondState.Text = item.BondState == Bond.None ? "None" : item.BondState.ToString();
            switch (item.Device.Type)
            {
                case BluetoothDeviceType.Classic:
                    holder.tvType.Text = "Classic";
                    break;
                case BluetoothDeviceType.Dual:
                    holder.tvType.Text = "Dual";
                    break;
                case BluetoothDeviceType.Le:
                    holder.tvType.Text = "Le";
                    break;
                case BluetoothDeviceType.Unknown:
                    holder.tvType.Text = "Unknown";
                    break;
                default:
                    holder.tvType.Text = "???";
                    break;
            }

            return view;
        }

        private AnimationDrawable _animationBluetoothConnecting;

        public void StatusChanged(BluetoothDevice device, ServiceStates state)
        {
            if (device != null && CurrentItems!=null)
            {
                // Chnagement de statut - l'ancienne connection est du coup desactivee
                CurrentItems.ForEach(_=>_.State = ServiceStates.Disconnected);

                BluetoothDeviceItem item = CurrentItems.FirstOrDefault(_ => _.Address == device.Address);
                if (item != null)
                {
                    item.State = state;
                }
                NotifyDataSetChanged();
            }
        }
    }
}