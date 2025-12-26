using Repository.Entities;
using SQLite;


namespace Repository.Dbo
{
    public abstract class BaseDbo
    {
        protected static readonly object dbLock = new object();

        private static string dbPath = null;

        private static SQLiteConnection _db = null;

        protected BaseDbo() { }

        public static SQLiteConnection Db
        {
            get
            {
                if (_db == null)
                {
                    _db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
                }
                return _db;
            }
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="busyTimeout"></param>
        public static void Init(string databasePath, double busyTimeout = 30)
        {
            dbPath = databasePath;
            Db.BusyTimeout = TimeSpan.FromSeconds(busyTimeout);
        }

        public static void Close()
        {
            if (_db != null)
            {
                _db.Close();
                _db = null;
            }
        }

        public static void Save(BaseEntity entity)
        {
            Db.InsertOrReplace(entity);
        }

        public void Save(IEnumerable<BaseEntity> entities)
        {
            lock (dbLock)
            {
                Db.RunInTransaction(() =>
                {
                    foreach (var e in entities)
                    {
                        e.Save(Db);
                    }
                });
            }
        }

        public void Remove(params BaseEntity[] entities)
        {
            Remove((IEnumerable<BaseEntity>)entities);
        }

        public void Remove(IEnumerable<BaseEntity> entities)
        {
            lock (dbLock)
            {
                Db.RunInTransaction(() =>
                {
                    foreach (var e in entities)
                    {
                        e.Remove(Db);
                    }
                });
            }
        }

        public static int AddColumn(string tableName, string columnName, string type, string lenght)
        {
            lock (dbLock)
            {
                try
                {
                    return Db.Execute($"alter table {tableName} add column {columnName} {type} ({lenght})");
                }
                catch
                {
                    // Nothing
                }
                return -1;
            }
        }

        public static int AddColumn(string tableName, string columnName, string type)
        {
            lock (dbLock)
            {
                try
                {
                    return Db.Execute($"alter table {tableName} add column {columnName} {type}");
                }
                catch
                {
                    // Nothing
                }
                return -1;
            }
        }

        public static T ExecuteScalar<T>(string query, params object[] args)
        {
            lock (Db)
            {
                return Db.ExecuteScalar<T>(query, args);
            }
        }

        public static int Execute(string query, params object[] args)
        {
            lock (Db)
            {
                return Db.Execute(query, args);
            }
        }

        public static int Insert(object obj)
        {
            lock (Db)
            {
                return Db.Insert(obj);
            }
        }

        public static int Update(object obj)
        {
            lock (Db)
            {
                return Db.Update(obj);
            }
        }

        public static int Delete(object objectToDelete)
        {
            lock (Db)
            {
                return Db.Delete(objectToDelete);
            }
        }

        public static int Delete<T>(object primaryKey)
        {
            lock (Db)
            {
                return Db.Delete<T>(primaryKey);
            }
        }

        public static int DeleteAll<T>()
        {
            lock (Db)
            {
                return Db.DeleteAll<T>();
            }
        }

        public static void CreateTable<T>() where T : class
        {
            lock (Db)
            {
                Db.CreateTable<T>();
            }
        }

        public static void DropTable<T>() where T : class
        {
            lock (Db)
            {
                Db.DropTable<T>();
            }
        }

    }

}
