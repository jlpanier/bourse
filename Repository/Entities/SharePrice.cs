using SQLite;
using System.ComponentModel;

namespace Repository.Entities
{
    [Table("PRICE")]
    public partial class SharePriceEntity : BaseEntity, INotifyPropertyChanged
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

        [Column("ID_SHARE")]
        public Guid ID_SHARE
        {
            get { return _id_share; }
            set
            {
                if (_id_share != value)
                {
                    _id_share = value;
                    NotifyPropertyChanged(nameof(ID_SHARE));
                }
            }
        }
        private Guid _id_share;
        
        [Column("AMOUNT")]
        public double AMOUNT
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged(nameof(AMOUNT));
                }
            }
        }
        private double _amount;

        [Column("DATEON")]
        public DateTime DATEON
        {
            get { return _dateon; }
            set
            {
                if (_dateon != value)
                {
                    _dateon = value;
                    NotifyPropertyChanged(nameof(DATEON));
                }
            }
        }
        private DateTime _dateon;

        [Column("RISK")]
        public string RISK
        {
            get { return _risk; }
            set
            {
                if (_risk != value)
                {
                    _risk = value;
                    NotifyPropertyChanged(nameof(RISK));
                }
            }
        }
        private string _risk;

        [Column("CONSENSUS")]
        public double CONSENSUS
        {
            get { return _consensus; }
            set
            {
                if (_consensus != value)
                {
                    _consensus = value;
                    NotifyPropertyChanged(nameof(CONSENSUS));
                }
            }
        }
        private double _consensus;

        [Column("RENDEMENT")]
        public double RENDEMENT
        {
            get { return _rendement; }
            set
            {
                if (_rendement != value)
                {
                    _rendement = value;
                    NotifyPropertyChanged(nameof(CONSENSUS));
                }
            }
        }
        private double _rendement;

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
