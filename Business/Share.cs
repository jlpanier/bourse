using Newtonsoft.Json;
using Repository.Dbo;
using Repository.Entities;
using System.Xml.Linq;
using System;
using WsBoursorama;
using Bourse.Views.Main;

namespace Business
{
    public class Share
    {
        #region Proprietes
        
        public ShareEntity ItemShare { get; private set; }

        public List<SharePriceEntity> ItemPrices { get; private set; }

        public Guid Id => ItemShare.ID;

        public string Code => ItemShare.CODE;
        
        public string Name => ItemShare.NAME;

        public string Url => ItemShare.URL;

        public DateTime DateMaj => ItemShare.DATEMAJ;

        public bool IsCac40 => ItemShare.CAC40;

        public bool ExistingData => ItemPrices.Any(_=> _.DATEON.Date == DateTime.Now.Date);

        #endregion

        public static List<Share> GetAll()
        {
            IEnumerable<ShareEntity> shareitems = ShareDbo.Get();
            IEnumerable<SharePriceEntity> pricesitems = SharePriceDbo.Get();

            List<Share> result = new List<Share>();
            foreach (ShareEntity item in shareitems)
            {
                result.Add(new Share(item));
            }

            foreach (Share shareitem in result)
            {
                IEnumerable<SharePriceEntity> prices = pricesitems.Where(_ => _.ID_SHARE == shareitem.Id);
                if (prices != null && prices.Any())
                {
                    shareitem.ItemPrices.AddRange(prices);
               }
            }
            return result;
        }

        public static Share GetAll(string code)
        {
            Share result = null;
            IEnumerable<ShareEntity> shareitems = ShareDbo.Get(code);
            if (shareitems.Any())
            {
                result = new Share(shareitems.First());
                IEnumerable<SharePriceEntity> pricesitems = SharePriceDbo.GetByShareId(result.Id);
                if (pricesitems.Any())
                {
                    result.ItemPrices.AddRange(pricesitems);
                }
            }
            return result;
        }

        public static string GetAllXml()
        {
            List<Share> shares = GetAll();
            XmlData data = new XmlData();
            data.Shares = new List<XmlShare>();
            foreach(Share share in shares)
            {
                XmlShare xmlShare = new XmlShare()
                {
                    CAC40 = share.IsCac40,
                    Code = share.Code,
                    DateMaj=share.DateMaj,
                    Name=share.Name,
                    Url=share.Url,
                    Prices = new List<XmlPrice>()
                };
                foreach(var prices in share.ItemPrices)
                {
                    xmlShare.Prices.Add(new XmlPrice()
                    {
                       Amount=prices.AMOUNT,
                       Concensus = prices.CONSENSUS,
                       DateMaj=prices.DATEMAJ,
                       DateOn=prices.DATEON,
                       Rendement=prices.RENDEMENT,
                       Risk=prices.RISK,
                    });
                }
                data.Shares.Add(xmlShare);
            }

            return JsonConvert.SerializeObject(data) + "\n";
        }

        public string GetXml()
        {
            XmlData data = new XmlData();
            data.Shares = new List<XmlShare>();

            XmlShare xmlShare = new XmlShare()
            {
                CAC40 = IsCac40,
                Code = Code,
                DateMaj = DateMaj,
                Name = Name,
                Url = Url,
                Prices = new List<XmlPrice>()
            };

            foreach (var prices in ItemPrices)
            {
                xmlShare.Prices.Add(new XmlPrice()
                {
                    Amount = prices.AMOUNT,
                    Concensus = prices.CONSENSUS,
                    DateMaj = prices.DATEMAJ,
                    DateOn = prices.DATEON,
                    Rendement = prices.RENDEMENT,
                    Risk = prices.RISK,
                });
            }
            data.Shares.Add(xmlShare);

            return JsonConvert.SerializeObject(data) + "\n";
        }

        public static Response Update(XmlData xmlData)
        {
            Response response = new Response();

            List<Share> shares = GetAll();
            foreach(XmlShare xmlShare in xmlData.Shares)
            {
                Share share = shares.FirstOrDefault(_=>_.Code == xmlShare.Code);
                if (share == null)
                {
                    share = Share.CreateOrUpdate(xmlShare.Code, xmlShare.Name, xmlShare.Url, xmlShare.CAC40);
                    response.NewShare++;
                }
                else if (share.DateMaj < xmlShare.DateMaj)
                {
                    share = Share.CreateOrUpdate(xmlShare.Code, xmlShare.Name, xmlShare.Url, xmlShare.CAC40, xmlShare.DateMaj);
                    response.UpdatedShare++;
                }
                foreach (XmlPrice xmlPrice in xmlShare.Prices)
                {
                    SharePriceEntity price= share.ItemPrices.FirstOrDefault(_=>_.DATEON.Date == xmlPrice.DateOn.Date);
                    if (price == null)
                    {
                        price = new SharePriceEntity()
                        {
                            AMOUNT = xmlPrice.Amount,
                            CONSENSUS = xmlPrice.Concensus,
                            DATEMAJ = xmlPrice.DateMaj,
                            DATEON = xmlPrice.DateOn,
                            ID = Guid.NewGuid(),
                            ID_SHARE = share.Id,
                            RENDEMENT = xmlPrice.Rendement,
                            RISK = xmlPrice.Risk,
                        };
                        SharePriceDbo.Save(price);
                        response.NewSharePrice++;
                    }
                    else if (price.AMOUNT != xmlPrice.Amount || price.CONSENSUS != xmlPrice.Concensus || price.RENDEMENT != xmlPrice.Rendement || price.RISK != xmlPrice.Risk)
                    {
                        price.AMOUNT = xmlPrice.Amount;
                        price.CONSENSUS = xmlPrice.Concensus;
                        price.DATEMAJ = xmlPrice.DateMaj;
                        price.DATEON = xmlPrice.DateOn;
                        price.RENDEMENT = xmlPrice.Rendement;
                        price.RISK = xmlPrice.Risk;
                        SharePriceDbo.Save(price);
                        response.UpdatedSharePrice++;
                    }
                }
            }
            return response;
        }

        public static Share Get(string code)
        {
            Share result = null;
            IEnumerable<ShareEntity> items = ShareDbo.Get(code);
            if (items.Any()) result = new Share(items.FirstOrDefault());
            return result;
        }

        public static Share CreateOrUpdate(string code, string name, string url, bool cac40, DateTime? datemaj = null)
        {
            DateTime dt = datemaj ?? DateTime.Now;

            Share item;
            IEnumerable<ShareEntity> items = ShareDbo.Get(code);
            if (items.Any())
            {
                item = new Share(items.FirstOrDefault());
                item.ItemShare.NAME = name;
                item.ItemShare.URL = url;
                item.ItemShare.CAC40 = cac40;
                item.ItemShare.DATEMAJ = dt;
            }
            else
            {
                item = new Share();
                item.ItemShare = new ShareEntity()
                {
                    DATEMAJ = dt,
                    NAME = name,
                    CAC40 = cac40,
                    CODE = code,
                    ID = Guid.NewGuid(),
                    URL = url
                };
            }
            ShareDbo.Save(item.ItemShare);
            return item;
        }

        private Share(ShareEntity item = null) 
        {
            ItemShare = item;
            ItemPrices = new List<SharePriceEntity>();
        }

        public void Remove()
        {
            ShareDbo.RemoveById(ItemShare.ID);
        }

        public void Fetch()
        {
            if (!ExistingData)
            {
                BoursoramaResponse response = WsBoursorama.WsBoursorama.WebSite(Url);
                if (response != null) 
                {
                    SharePriceEntity item = new SharePriceEntity()
                    {
                        AMOUNT = response.Amount,
                        CONSENSUS = response.Consensus,
                        DATEMAJ = DateTime.Now,
                        DATEON = DateTime.Now,
                        ID = Guid.NewGuid(),
                        ID_SHARE = Id,
                        RENDEMENT = response.Rendement,
                        RISK = response.Risk,
                    };
                    SharePriceDbo.Save(item);
                }
            }
        }
     }
}
