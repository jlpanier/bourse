using Repository.Entities;

namespace Repository.Dbo
{
    public class ShareDbo: BaseDbo
    {
        public static IEnumerable<ShareEntity> Get()
        {
            lock (dbLock)
            {
                return Db.Query<ShareEntity>(@"Select * from SHARE ");
            }
        }

        public static IEnumerable<ShareEntity> Get(string code)
        {
            lock (dbLock)
            {
                return Db.Query<ShareEntity>(@"Select * from SHARE WHERE CODE = ?", code);
            }
        }

        public static int RemoveById(Guid id)
        {
            lock (dbLock)
            {
                return Db.Execute(@"DELETE FROM SHARE WHERE ID = ?", id);
            }
        }
    }
}
