using Android.Content;
using Android.Views;
using Business;
using System.Globalization;

namespace Bourse.Views.Main
{
    internal class ListviewSharePricesAdapter : SimpleListAdapter<SharePricesItem>
    {
        private readonly Context _context;

        private readonly CultureInfo _culture;

        public ListviewSharePricesAdapter(Context context):base(context)
        {
            _context = context;
            _culture = CultureInfo.CreateSpecificCulture("fr-FR");

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ListviewSharePricesHolder holder = null;

            if (view!=null) holder = view.Tag as ListviewSharePricesHolder;

            if (holder == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_shareprices, parent, false);
                
                holder = new ListviewSharePricesHolder();
                holder.tvCode = view.FindViewById<TextView>(Resource.Id.tvCode);
                holder.tvName = view.FindViewById<TextView>(Resource.Id.tvName);
                holder.tvAmount = view.FindViewById<TextView>(Resource.Id.tvAmount);
                holder.tvDate = view.FindViewById<TextView>(Resource.Id.tvDate);
                holder.tvRisk = view.FindViewById<TextView>(Resource.Id.tvRisk);
                holder.tvConsensus = view.FindViewById<TextView>(Resource.Id.tvConsensus);
                holder.tvRendement = view.FindViewById<TextView>(Resource.Id.tvRendement);

                view.Tag = holder;
            }
            var item = CurrentItems[position];
            holder.Position = position;
            holder.tvCode.Text = item.Code;
            holder.tvName.Text = item.Name;
            holder.tvAmount.Text = item.Amount.ToString("N", _culture);
            holder.tvDate.Text = item.DateOn.ToString("dd/MM/yyyy");
            holder.tvRisk.Text = item.Risk;
            holder.tvConsensus.Text = item.Consensus.ToString("N", _culture);
            holder.tvRendement.Text = item.Rendement.ToString("N", _culture) + "%";
            return view;
        }
    }
}
