using Android.Content;
using Android.Mtp;
using Android.Runtime;
using Bourse.Views.Main;
using Business;
using Repository.Entities;

namespace Bourse.Views.ShareHistory;


[Activity(Label = "@string/app_name", MainLauncher = false)]
public class ShareHistoryActivity : Activity
{
    private ListviewSharePricesAdapter _adapter;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.share_history);

         ListView lvSharePrices = FindViewById<ListView>(Resource.Id.lvSharePrices);
        _adapter = new ListviewSharePricesAdapter(this);
        lvSharePrices.Adapter = _adapter;
        lvSharePrices.ScrollbarFadingEnabled = true;

    }

    protected override void OnResume()
    {
        base.OnResume();
        Load();
    }



    private void Load()
    {
        Share share = Share.GetAll(Intent.GetStringExtra(BaseActivity.ParamCode));

        List<SharePricesItem> sharepricesitems = new List<SharePricesItem>();
        foreach (SharePriceEntity price in share.ItemPrices)
        {
            sharepricesitems.Add(new SharePricesItem(share, price));
        }
        _adapter.Reset(sharepricesitems.OrderBy(_=>_.DateOn));
    }


}