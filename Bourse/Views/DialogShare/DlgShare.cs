using Acr.UserDialogs;
using Android.Views;
using Business;


namespace Bourse.Views.DialogShare
{
    /// <summary>
    /// Boite de dialogue permettant à l'utilisateur d'ajouer une nouvelle action
    /// </summary>
    public class DlgShare : DialogFragment
    {
        #region event handler

        /// <summary>
        /// Evènement suite à validation
        /// </summary>
        public EventHandler<Share> NotifyChanged { get; set; }

        #endregion

        public DlgShare()
        {
            _item = null;
        }

        public DlgShare(Share item)
        {
            _item = item;
        }

        private readonly Share _item;

        #region life cycle

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dlgshare, container, false);

            view.FindViewById<Button>(Resource.Id.btnDel).Click += (s, e) => { Delete(); };
            view.FindViewById<Button>(Resource.Id.btnValider).Click += (s, e) => { Save(); };
            view.FindViewById<Button>(Resource.Id.btnAnnuler).Click += (s, e) => { this.Dismiss(); };

            Dialog.SetCancelable(false);
            Dialog.SetCanceledOnTouchOutside(false);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            EditText edUrl = View.FindViewById<EditText>(Resource.Id.edUrl);
            if (_item!=null)
            {
                View.FindViewById<EditText>(Resource.Id.edCode).Text = _item.Code;
                View.FindViewById<EditText>(Resource.Id.edName).Text = _item.Name;
                edUrl.Text = _item.Url;
                View.FindViewById<CheckBox>(Resource.Id.cbCac40).Checked = _item.IsCac40;
            }
            if (string.IsNullOrEmpty(edUrl.Text.Trim()))
            {
                edUrl.Text = "https://www.boursorama.com/cours/";
            }
        }


        #endregion

        private void Save()
        {
            try
            {
                string code = View.FindViewById<EditText>(Resource.Id.edCode).Text.Trim();
                string name = View.FindViewById<EditText>(Resource.Id.edName).Text.Trim();
                string url = View.FindViewById<EditText>(Resource.Id.edUrl).Text.Trim();
                bool cac40 = View.FindViewById<CheckBox>(Resource.Id.cbCac40).Checked;

                Share item =  Share.CreateOrUpdate(code, name, url, cac40);
                NotifyChanged?.Invoke(this, item);
                this.Dismiss();
            }
            catch (MessageException ex) 
            {
                UserDialogs.Instance.Alert(ex.Message, Application.Context.GetString(Resource.String.Alerte));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.Message, Application.Context.GetString(Resource.String.Exception));
            }
        }

        private void Delete()
        {
            try
            {
                string code = View.FindViewById<EditText>(Resource.Id.edCode).Text.Trim();
                Share item = Share.Get(code);
                if (item!=null)
                {
                    item.Remove();
                    NotifyChanged?.Invoke(this, item);
                    this.Dismiss();
                }
            }
            catch (MessageException ex)
            {
                UserDialogs.Instance.Alert(ex.Message, Application.Context.GetString(Resource.String.Alerte));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.Message, Application.Context.GetString(Resource.String.Exception));
            }
        }
    }
}
