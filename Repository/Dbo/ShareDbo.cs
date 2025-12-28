using Repository.Entities;

namespace Repository.Dbo
{
    public class ShareDbo: BaseDbo
    {
        public static ShareDbo Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new ShareDbo();
                }
                return _instance;
            }
        }
        private static ShareDbo? _instance;

        public IEnumerable<ShareEntity> Get()
        {
            lock (dbLock)
            {
                return Db.Query<ShareEntity>(@"Select * from SHARE ");
            }
        }

        public IEnumerable<ShareEntity> Get(string code)
        {
            lock (dbLock)
            {
                return Db.Query<ShareEntity>(@"Select * from SHARE WHERE CODE = ?", code);
            }
        }

        public int RemoveById(Guid id)
        {
            lock (dbLock)
            {
                return Db.Execute(@"DELETE FROM SHARE WHERE ID = ?", id);
            }
        }
    }
}
