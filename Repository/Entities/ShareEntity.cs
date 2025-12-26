using SQLite;
using System.ComponentModel;

namespace Repository.Entities
{
    [Table("SHARE")]
    public partial class ShareEntity : BaseEntity, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            IsDirty = true;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Ignore]
        public bool IsDirty { get; set; }

        #endregion

        [PrimaryKey]
        [Column("ID")]
        public Guid ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(ID));
                }
            }
        }
        private Guid _id;

        [Column("CODE")]
        public string CODE
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    NotifyPropertyChanged(nameof(CODE));
                }
            }
        }
        private string _code;

        [Column("NAME")]
        public string NAME
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(NAME));
                }
            }
        }
        private string _name;

        [Column("URL")]
        public string URL
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    NotifyPropertyChanged(nameof(URL));
                }
            }
        }
        private string _url;

        [Column("CAC40")]
        public bool CAC40
        {
            get { return _cac40; }
            set
            {
                if (_cac40 != value)
                {
                    _cac40 = value;
                    NotifyPropertyChanged(nameof(CAC40));
                }
            }
        }
        private bool _cac40;

        [Column("DATEMAJ")]
        public DateTime DATEMAJ
        {
            get { return _datemaj; }
            set
            {
                if (_datemaj != value)
                {
                    _datemaj = value;
                    NotifyPropertyChanged(nameof(DATEMAJ));
                }
            }
        }
        private DateTime _datemaj;
    }
}
