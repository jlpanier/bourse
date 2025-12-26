using Repository.Entities;

namespace Repository.Dbo
{
    public class SharePriceDbo:BaseDbo
    {
        public static IEnumerable<SharePriceEntity> Get()
        {
            lock (dbLock)
            {
                return Db.Query<SharePriceEntity>(@"Select * from PRICE ");
            }
        }

        public static IEnumerable<SharePriceEntity> GetByShareId(Guid idshare)
        {
            lock (dbLock)
            {
                return Db.Query<SharePriceEntity>(@"Select * from PRICE WHERE ID_SHARE = ?", idshare);
            }
        }
    }
}
