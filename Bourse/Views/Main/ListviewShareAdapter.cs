using Android.Content;
using Android.Graphics;
using Android.Views;
using Business;

namespace Bourse.Views.Main
{
    internal class ListviewShareAdapter : SimpleListAdapter<ShareItem>
    {
        public ListviewShareAdapter(Context context):base(context)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ListviewShareHolder holder = null;

            if (view!=null) holder = view.Tag as ListviewShareHolder;

            if (holder == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_share, parent, false);
                
                holder = new ListviewShareHolder();
                holder.tvCode = view.FindViewById<TextView>(Resource.Id.tvCode);
                holder.tvName = view.FindViewById<TextView>(Resource.Id.tvName);
                holder.imgShare = view.FindViewById<ImageView>(Resource.Id.imgShare);
                holder.imgCac40 = view.FindViewById<ImageView>(Resource.Id.imgCac40);

                view.Tag = holder;
            }
            var item = CurrentItems[position];
            holder.Position = position;
            holder.tvCode.Text = item.Code;
            holder.tvName.Text = item.Name;
            holder.imgCac40.Visibility = item.IsCac40 ? ViewStates.Visible : ViewStates.Gone;

            string filename = System.IO.Path.Combine(Setup.ImagePath, item.Code + ".png");
            if (File.Exists(filename))
            {
                using (Bitmap bm = BitmapFactory.DecodeFile(filename))
                {
                    holder.imgShare.SetImageBitmap(bm);
                }
            }
            return view;
        }
    }
}
