using Business;

namespace Bourse.Views.Main
{
    internal class ShareItem
    {
        public readonly Share Item;

        public string Code => Item.Code;

        public string Name => Item.Name;

        public string Url => Item.Url;

        public bool IsCac40 => Item.IsCac40;

        public ShareItem(Share item)
        {
            Item = item;
        }
    }
}
