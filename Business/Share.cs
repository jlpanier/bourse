using Repository.Dbo;
using Repository.Entities;
using System.Drawing;
using WsBoursorama;

namespace Business
{
    public class Share
    {
        #region Proprietes

        public ShareEntity Item { get; private set; }

        public Guid Id => Item.ID;

        public string Code => Item.CODE;
        
        public string Name => Item.NAME;

        public string Url => Item.URL;

        public DateTime DateMaj => Item.DATEMAJ;

        public DateTime DateOn => Item.DATEON;

        public bool IsCac40 => Item.CAC40;

        public double Amount => Item.AMOUNT;

        public double Consensus => Item.CONSENSUS;

        public double Rendement => Item.RENDEMENT/100;

        public double Risk => Item.RISK;

        #endregion

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

        public Share? GetByCode(string code)
        {
            Share? result = null;
            var shareitems = ShareDbo.Instance.Get(code).FirstOrDefault();
            if (shareitems != null)
            {
                result = new Share(shareitems);
            }
            return result;
        }

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

        public void Remove()
        {
            ShareDbo.Instance.RemoveById(Item.ID);
        }

        /// <summary>
        /// VRAI, si la valeur de l'action doit être téléchargée
        /// </summary>
        public bool ShouldUpdate
        {
            get
            {
                DateTime dt;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    dt = DateTime.Now.AddDays(-1);
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                {
                    dt = DateTime.Now.AddDays(-2);
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                }
                else if (DateTime.Now.Hour < 8)
                {
                    dt = DateTime.Now.AddDays(-1);
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                }
                else
                {
                    dt = DateTime.Now;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 17, 30, 0);
                }
                return dt > DateOn;
            }
        }

        public void Fetch()
        {
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
            }
        }
     }
}
