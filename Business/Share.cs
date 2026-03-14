using Repository.Dbo;
using Repository.Entities;
using WsBoursorama;

namespace Business
{
    public class Share
    {
        #region Proprietes

        /// <summary>
        /// Référence à l'entité de la base de données
        /// </summary>
        public ShareEntity Item { get; private set; }

        /// <summary>
        /// Code de l'action
        /// </summary>
        public string Code => Item.CODE;

        /// <summary>
        /// Nom de l'action
        /// </summary>
        public string Name => Item.NAME;

        /// <summary>
        /// URL Boursorama
        /// </summary>
        public string Url => Item.URL;

        /// <summary>
        /// Date de chargement des valeurs de l'api boursorama
        /// </summary>
        public DateTime DateOn => Item.DATEON;

        /// <summary>
        /// VRAI si action du CAC 40
        /// </summary>
        public bool IsCac40 => Item.CAC40;

        /// <summary>
        /// Montant de l'action
        /// </summary>
        public double Amount => Item.AMOUNT;

        /// <summary>
        /// Concensus boursorama de l'action
        /// </summary>
        public double Consensus => Item.CONSENSUS;

        /// <summary>
        /// Rendement de l'action
        /// </summary>
        public double Rendement => Item.RENDEMENT/100;

        /// <summary>
        /// Risque associé à l'action
        /// </summary>
        public double Risk => Item.RISK;

        /// <summary>
        /// VRAI, si la valeur de l'action doit être téléchargée
        /// </summary>
        private DateTime NextDownloadDate
        {
            get
            {
                if (_nextDate==null)
                {
                    DateTime dt;
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = DateTime.Now.AddDays(-1);
                        _nextDate = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                    }
                    else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = DateTime.Now.AddDays(-2);
                        _nextDate = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                    }
                    else if (DateTime.Now.Hour < 8)
                    {
                        dt = DateTime.Now.AddDays(-1);
                        _nextDate = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                    }
                    else
                    {
                        dt = DateTime.Now;
                        _nextDate = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                    }
                }
                return _nextDate ?? DateTime.Now;
            }
        }
        private DateTime? _nextDate;

        #endregion

        /// <summary>
        /// Liste toutes les actions
        /// </summary>
        public static List<Share> Load()
        {
            IEnumerable<ShareEntity> items = ShareDbo.Instance.Get();
 
            List<Share> result = new List<Share>();
            foreach (ShareEntity item in items)
            {
                result.Add(new Share(item));
            }

            return result;
        }

        /// <summary>
        /// Créer dans la base de données une nouvelle action
        /// </summary>
        public static Share Create(string code, string name, string url, bool cac40)
        {
            Share result;
            var item = ShareDbo.Instance.Get(code).FirstOrDefault();
            if (item != null)
            {
                result = new Share(new ShareEntity()
                {
                    NAME = name,
                    URL = url,
                    CAC40 = cac40,
                    DATEMAJ = DateTime.Now,
                });
            }
            else
            {
                result = new Share(new ShareEntity()
                {
                    ID = Guid.NewGuid(),
                    CODE = code,
                    NAME = name,
                    URL = url,
                    CAC40 = cac40,
                    DATEMAJ = DateTime.Now,
                });
            }
            ShareDbo.Instance.Save(result.Item);
            return result;
        }

        /// <summary>
        /// Mise à jour de l'action
        /// </summary>
        public void Update(string code, string name, string url, bool cac40)
        {
            Item.CODE = code;
            Item.NAME = name;
            Item.URL = url;
            Item.CAC40 = cac40;
            Item.DATEMAJ = DateTime.Now;
            ShareDbo.Instance.Save(Item);
        }

        private Share(ShareEntity item) 
        {
            Item = item;
        }

        /// <summary>
        /// VRAI, si la valeur de l'action doit être téléchargée
        /// </summary>
        public bool ShouldUpdate => NextDownloadDate > DateOn;

        /// <summary>
        /// Charge les valeurs de l'action en consultant la page boursorama
        /// </summary>
        /// <returns>VRAI, si mise à jour effectuée</returns>
        public bool Fetch()
        {
            var updated = false;
            if(ShouldUpdate)
            {
                BoursoramaResponse response = WsBoursorama.WsBoursorama.WebSite(Url);
                if (response != null)
                {
                    Item.AMOUNT = response.Amount;
                    Item.CONSENSUS = response.Consensus;
                    Item.DATEON = DateTime.Now;
                    Item.RENDEMENT = response.Rendement;
                    Item.RISK = response.Risk;
                    ShareDbo.Instance.Save(Item);
                }
                updated = true;
            }
            return updated;
        }
     }
}
