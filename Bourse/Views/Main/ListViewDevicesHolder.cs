using Android.Widget;

namespace Bourse.Views.Main
{
    class ListViewDevicesHolder : Java.Lang.Object
    {
        public int Position { get; set; }
        public ImageView imgConnection { get; set; }
        public TextView tvName { get; set; }
        public TextView tvAddress { get; set; }
        public TextView tvBondState { get; set; }
        public TextView tvType { get; set; }
    }
}