using Android.Bluetooth;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Bourse.Common;
using Bourse.Views.DialogShare;
using Bourse.Views.ShareHistory;
using Bourse.Views.Web;
using Business;
using Newtonsoft.Json;
using Repository.Entities;
using System.ComponentModel;
using static Android.Widget.AdapterView;

namespace Bourse.Views.Main
{
    [Activity(Label = "@string/app_name", MainLauncher = false)]
    public class MainActivity : BluetoothBaseActivity  //BaseActivity
    {
        public const int RequestCodeSharePrice = 1000;
        public const int RequestCodeWebView = 1001;

        private enum Display { Shares, SharesPrices, Visuel, Bluetooth }

        private Display _display = Display.Visuel;

        /// <summary>
        /// Layout de notre activité
        /// </summary>
        protected override int LayoutResourceId => Resource.Layout.main;

        private ListviewSharePricesAdapter _adapterSharePrices;

        private ListviewShareAdapter _adapterShares;

        private ListViewDevicesAdaptator _boundedDevicesAdaptator = null;

        /// <summary>
        /// Adapter de la liste des messages des communications entre les PDA 
        /// </summary>
        private ArrayAdapter<String> _conversationArrayAdapter;

        public EventHandler<int> NotifyChangedFetch { get; set; }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResourceId);

            #region bottom button 

            FindViewById<ToggleButton>(Resource.Id.tbShare).Click += (s, e) =>
            {
                _display = Display.Shares;
                OnButtonMenu();
            };
            FindViewById<ToggleButton>(Resource.Id.tbSharePrices).Click += (s, e) =>
            {
                _display = Display.SharesPrices;
                OnButtonMenu();
            };
            FindViewById<ToggleButton>(Resource.Id.tbBluetooth).Click += (s, e) =>
            {
                _display = Display.Bluetooth;
                OnButtonMenu();
            };
            FindViewById<ToggleButton>(Resource.Id.tbVisuel).Click += (s, e) =>
            {
                _display = Display.Visuel;
                OnButtonMenu();
            };
            FindViewById<LinearLayout>(Resource.Id.btnShare).Click += (s, e) =>
            {
                _display = Display.Shares;
                OnButtonMenu();
            };
            FindViewById<LinearLayout>(Resource.Id.btnSharePrice).Click += (s, e) =>
            {
                _display = Display.SharesPrices;
                OnButtonMenu();
            };
            FindViewById<LinearLayout>(Resource.Id.btnBluetooth).Click += (s, e) =>
            {
                _display = Display.Bluetooth;
                OnButtonMenu();
            };
            FindViewById<LinearLayout>(Resource.Id.btnVisuel).Click += (s, e) =>
            {
                _display = Display.Visuel;
                OnButtonMenu();
            };

            #endregion

            #region shares

            FindViewById<ImageView>(Resource.Id.btnAddShares).Click += (s, e) =>
            {
                DlgShare dialog = new DlgShare();
                dialog.NotifyChanged += (s, e) =>
                {
                    Load();
                };
                dialog.Show(FragmentManager, null);
            };

            _adapterShares = new ListviewShareAdapter(this);
            ListView lvShares = FindViewById<ListView>(Resource.Id.lvShares);
            lvShares.Adapter = _adapterShares;
            lvShares.ScrollbarFadingEnabled = false;
            lvShares.ItemClick += OnItemClickShare;

            #endregion

            #region share prices

            FindViewById<ImageView>(Resource.Id.btnLoadShares).Click += (s, e) =>
            {
                ProgressBar pg = FindViewById<ProgressBar>(Resource.Id.pbLoading);
                pg.Min = 0;
                pg.Max = _adapterShares.CurrentItems.Count+1;
                pg.Indeterminate = false;
                pg.Visibility =Android.Views.ViewStates.Visible;

                var backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(DoWork);
                backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(OnWorkerCompleted);
                //backgroundWorker.ProgressChanged +=new ProgressChangedEventHandler( WorkerProgressChanged);
                backgroundWorker.WorkerSupportsCancellation = false;
                NotifyChangedFetch += OnNotifyChanged;
                backgroundWorker.RunWorkerAsync(2000);



            };
            
            FindViewById<RadioGroup>(Resource.Id.radiogroupOrder).CheckedChange += (s, e) =>
            {
                Refresh();
            };


            ListView lvSharePrices = FindViewById<ListView>(Resource.Id.lvSharePrices);
            _adapterSharePrices = new ListviewSharePricesAdapter(this);
            lvSharePrices.Adapter = _adapterSharePrices;
            lvSharePrices.ScrollbarFadingEnabled = false;
            lvSharePrices.ItemClick += OnItemClickSharePrice;
            lvSharePrices.ItemLongClick += OnItemLongClickSharePrice;
            #endregion

            #region bluetooth

            FindViewById<TextView>(Resource.Id.tvNoBondedDevices).Visibility = Android.Views.ViewStates.Visible;

            _boundedDevicesAdaptator = new ListViewDevicesAdaptator(this, BluetoothBondedDevices);
            var lvBondedDevices = FindViewById<ListView>(Resource.Id.lvBondedDevices);
            lvBondedDevices.Adapter = _boundedDevicesAdaptator;
            lvBondedDevices.ItemClick += (s, e) =>
            {
                if (e.Position >= 0)
                {
                    BluetoothConnectBond(_boundedDevicesAdaptator[e.Position].Device);
                }
            };
            lvBondedDevices.Visibility = BluetoothBondedDevices.Any() ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.tvNoBondedDevices).Visibility = BluetoothBondedDevices.Any() ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;

            FindViewById<ImageView>(Resource.Id.btnSend).Click += (sender, e) =>
            {
                var shares = Share.GetAllXml();
                SendBluetoothMessage(shares);
                _conversationArrayAdapter.Add($"Me: envoi de {shares.Count()/1024:N0} k.bytes");
            };

            _conversationArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.message);
            FindViewById<ListView>(Resource.Id.lvText).Adapter = _conversationArrayAdapter;


            _animationBluetoothConnecting = (Android.Graphics.Drawables.AnimationDrawable)Resources.GetDrawable(Resource.Drawable.bluetooth_connecting);

            ImageView asteroidImage = FindViewById<ImageView>(Resource.Id.bluetooth_connecting);
            asteroidImage.SetImageDrawable(_animationBluetoothConnecting);
            _animationBluetoothConnecting.Start();
            

            #endregion
        }


        private AnimationDrawable _animationBluetoothConnecting;


        protected override void OnResume()
        {
            base.OnResume();
            OnButtonMenu();
            Load();
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data == null) return;
            if (resultCode != Result.Ok) return;

            switch (requestCode)
            {
                case RequestCodeSharePrice:
                    break;

            }
        }


        private void OnButtonMenu()
        {
            switch (_display)
            {
                case Display.Shares:
                    FindViewById<RelativeLayout>(Resource.Id.idshares).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<LinearLayout>(Resource.Id.idshareprices).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idblueooth).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idvisuel).Visibility = Android.Views.ViewStates.Gone;

                    FindViewById<ToggleButton>(Resource.Id.tbShare).Checked = true;
                    FindViewById<ToggleButton>(Resource.Id.tbSharePrices).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbBluetooth).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbVisuel).Checked = false;
                    break;
                case Display.SharesPrices:
                    FindViewById<RelativeLayout>(Resource.Id.idshares).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idshareprices).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<LinearLayout>(Resource.Id.idblueooth).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idvisuel).Visibility = Android.Views.ViewStates.Gone;
                    
                    FindViewById<ToggleButton>(Resource.Id.tbShare).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbSharePrices).Checked = true;
                    FindViewById<ToggleButton>(Resource.Id.tbBluetooth).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbVisuel).Checked = false;
                    break;
                case Display.Bluetooth:
                    FindViewById<RelativeLayout>(Resource.Id.idshares).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idshareprices).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idblueooth).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<LinearLayout>(Resource.Id.idvisuel).Visibility = Android.Views.ViewStates.Gone;

                    FindViewById<ToggleButton>(Resource.Id.tbShare).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbSharePrices).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbBluetooth).Checked = true;
                    FindViewById<ToggleButton>(Resource.Id.tbVisuel).Checked = false;
                    break;
                case Display.Visuel:
                    FindViewById<RelativeLayout>(Resource.Id.idshares).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idshareprices).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idblueooth).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.idvisuel).Visibility = Android.Views.ViewStates.Visible;
                    
                    FindViewById<ToggleButton>(Resource.Id.tbShare).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbSharePrices).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbBluetooth).Checked = false;
                    FindViewById<ToggleButton>(Resource.Id.tbVisuel).Checked = true;
                    break;
            }
        }


        private void OnItemClickShare(object? sender, AdapterView.ItemClickEventArgs e)
        {
            DlgShare dialog = new DlgShare(_adapterShares[e.Position].Item);
            dialog.NotifyChanged += (s, e) =>
            {
                Load();
            };
            dialog.Show(FragmentManager, null);
        }

        private void OnItemClickSharePrice(object? sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(ApplicationContext, typeof(ShareHistoryActivity));
            intent.SetFlags(ActivityFlags.NewTask);
            intent.PutExtra(BaseActivity.ParamCode, _adapterSharePrices[e.Position].Code);
            StartActivityForResult(intent, RequestCodeSharePrice);
        }

        private void OnItemLongClickSharePrice(object? sender, ItemLongClickEventArgs e)
        {
            var intent = new Intent(ApplicationContext, typeof(WebViewActivity));
            intent.SetFlags(ActivityFlags.NewTask);
            intent.PutExtra(BaseActivity.ParamUrl, _adapterSharePrices[e.Position].Url);
            StartActivityForResult(intent, RequestCodeWebView);

        }


        private void Load()
        {
            List<Share> shares = Share.GetAll();

            List<ShareItem> shareitems = new List<ShareItem>();
            shares.ForEach(_ => shareitems.Add(new ShareItem(_)));
            _adapterShares.Reset(shareitems.OrderBy(_ => _.Name));

            List<SharePricesItem> sharepricesitems = new List<SharePricesItem>();
            foreach (Share share in shares)
            {
                foreach(SharePriceEntity price in share.ItemPrices)
                {
                    sharepricesitems.Add(new SharePricesItem(share, price));
                }
            }
            Refresh(sharepricesitems);
        }

        private void Refresh(List<SharePricesItem> sharepricesitems = null)
        {
            var it = sharepricesitems == null ? new List<SharePricesItem>(_adapterSharePrices.CurrentItems) : new List<SharePricesItem>(sharepricesitems);

            var items = FindViewById<Switch>(Resource.Id.swCac40).Checked ? it.Where(_ => _.IsCac40) : it;

            List<SharePricesItem> lOrders = null;
            if (FindViewById<RadioButton>(Resource.Id.radioRisk).Checked)
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenBy(_ => _.Risk));
            }
            else if (FindViewById<RadioButton>(Resource.Id.radioConsensus).Checked)
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenBy(_ => _.Consensus));
            }
            else if (FindViewById<RadioButton>(Resource.Id.radioRendement).Checked)
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenByDescending(_ => _.Rendement));
            }
            else if (FindViewById<RadioButton>(Resource.Id.radioCode).Checked)
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenBy(_ => _.Code));
            }
            else if (FindViewById<RadioButton>(Resource.Id.radioName).Checked)
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenBy(_ => _.Name));
            }
            else
            {
                lOrders = new List<SharePricesItem>(items.OrderByDescending(_ => _.DateOn.Date).ThenBy(_ => _.Code));
            }
            _adapterSharePrices.Reset(lOrders);
        }

        #region bluetooth area

        /// <summary>
        /// Broadcast actionn bluetooth : indicates the bluetooth scan mode of the local adapter has changed
        /// </summary>
        protected override void OnBluetoothScanModeChanged(BluetoothScanModeEventArgs scanmode)
        {
            base.OnBluetoothScanModeChanged(scanmode);
            //Message(scanmode.ToString());
            _conversationArrayAdapter.Add($"{ConnectedDevice?.Name}: Scan mode {scanmode}");
        }

        /// <summary>
        /// Bluetooth : Reception message
        /// </summary>
        protected override void OnBluetoothReceive(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                XmlData xmlData;
                try
                {
                    completjson += json;
                    xmlData = JsonConvert.DeserializeObject<XmlData>(completjson.Trim());
                    var response = Share.Update(xmlData);
                    _conversationArrayAdapter.Add($"{ConnectedDevice?.Name}: {response.NewShare} nouvelles actions, {response.UpdatedShare} mise a jour");
                    _conversationArrayAdapter.Add($"{ConnectedDevice?.Name}: {response.NewSharePrice} nouveaux cours, {response.UpdatedSharePrice} mise a jour");
                    completjson = "";
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private string completjson = "";

        /// <summary>
        /// Bluetooth : Send message
        /// </summary>
        protected override void OnBluetoothSend(string message)
        {
            base.OnBluetoothSend(message);
            _conversationArrayAdapter.Add($"Me: envoi des donnees");
        }

        /// <summary>
        /// Bluetooth : Bluetooth change status connexion
        /// </summary>
        protected override void OnBluetoothStatusChanged(BluetoothDevice device, ServiceStates state)
        {
            _boundedDevicesAdaptator.StatusChanged(device, state);
            if (device!=null)
            {
                _conversationArrayAdapter.Add($"Me: change statut {device.Name} {device.Address} - {state.GetStringValue()}");
            }
        }

        /// <summary>
        /// Bluetooth : send a message to other device
        /// </summary>
        /// <param name="message"></param>
        protected override void SendBluetoothMessage(string message)
        {
            base.SendBluetoothMessage(message);
            //FindViewById<EditText>(Resource.Id.edit_text_out).Text = string.Empty;
        }

        #endregion


        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;

            // Extract the argument.
            int arg = (int)e.Argument;

            // Start the time-consuming operation.
            int progress = 1;
            foreach (ShareItem share in _adapterShares.CurrentItems)
            {
                share.Item.Fetch();
                NotifyChangedFetch.Invoke(this, progress++);
            }

            e.Result = null;
        }

        private void OnWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            FindViewById<ProgressBar>(Resource.Id.pbLoading).Visibility = Android.Views.ViewStates.Gone;
            Load();
        }


        private void OnNotifyChanged(object sender, int process)
        {
            FindViewById<ProgressBar>(Resource.Id.pbLoading).SetProgress(process, true);
        }
        
    }
}