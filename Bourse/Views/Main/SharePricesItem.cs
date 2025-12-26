using Business;
using Repository.Entities;


namespace Bourse.Views.Main
{
    internal class SharePricesItem
    {
        public readonly Share Item;

        public readonly SharePriceEntity Price;

        public string Code => Item.Code;

        public string Name => Item.Name;

        public string Url => Item.Url;

        public bool IsCac40 => Item.IsCac40;

        public DateTime DateOn => Price.DATEON;

        public double Consensus => Price.CONSENSUS;

        public double Amount => Price.AMOUNT;

        public double Rendement => Price.RENDEMENT;

        public string Risk => Price.RISK;

        public SharePricesItem(Share item, SharePriceEntity price)
        {
            Item = item;
            Price = price;
        }
    }
}
