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

        public bool IsCac40 => Item.CAC40;

        public double Amount => Item.AMOUNT;

        public double Consensus => Item.CONSENSUS;

        public double Rendement => Item.RENDEMENT;

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

        public void Fetch()
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
