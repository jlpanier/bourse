using Repository.Entities;
using SQLite;


namespace Repository.Dbo
{
    public abstract class BaseDbo
    {
        protected static readonly object dbLock = new object();

        /// <summary>
        /// Chemin de la base de données
        /// </summary>
        public static string DbPath = string.Empty;

        protected BaseDbo() { }

        public static SQLiteConnection Db
        {
            get
            {
                if (_db == null)
                {
                    _db = new SQLiteConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
                }
                return _db;
            }
        }
        private static SQLiteConnection? _db = null;

        /// <summary>
        /// VRAI, si le fichier de la base de données existe
        /// </summary>
        /// <returns></returns>
        public bool IsReady() => !string.IsNullOrEmpty(DbPath) && File.Exists(DbPath);

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="busyTimeout"></param>
        public void Init(string databasePath, double busyTimeout = 30)
        {
            DbPath = databasePath;
            if (!File.Exists(DbPath)) throw new FileNotFoundException("File does not exists", DbPath);
            Db.BusyTimeout = TimeSpan.FromSeconds(busyTimeout);
            CreateTable<ShareEntity>();
            AddColumn("SHARE", "AMOUNT", "REAL");
            AddColumn("SHARE", "RISK", "REAL");
            AddColumn("SHARE", "CONSENSUS", "REAL");
            AddColumn("SHARE", "RENDEMENT", "REAL");
            AddColumn("SHARE", "DATEON", "DATETIME");
        }

        public void Close()
        {
            if (_db != null)
            {
                _db.Close();
                _db = null;
            }
        }

        public void Save(BaseEntity entity)
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

        public int AddColumn(string tableName, string columnName, string type, string lenght)
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

        public int AddColumn(string tableName, string columnName, string type)
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

        public T ExecuteScalar<T>(string query, params object[] args)
        {
            lock (Db)
            {
                return Db.ExecuteScalar<T>(query, args);
            }
        }

        public int Execute(string query, params object[] args)
        {
            lock (Db)
            {
                return Db.Execute(query, args);
            }
        }

        public int Insert(object obj)
        {
            lock (Db)
            {
                return Db.Insert(obj);
            }
        }

        public int Update(object obj)
        {
            lock (Db)
            {
                return Db.Update(obj);
            }
        }

        public int Delete(object objectToDelete)
        {
            lock (Db)
            {
                return Db.Delete(objectToDelete);
            }
        }

        public int Delete<T>(object primaryKey)
        {
            lock (Db)
            {
                return Db.Delete<T>(primaryKey);
            }
        }

        public int DeleteAll<T>()
        {
            lock (Db)
            {
                return Db.DeleteAll<T>();
            }
        }

        public void CreateTable<T>() where T : class
        {
            lock (Db)
            {
                Db.CreateTable<T>();
            }
        }

        public void DropTable<T>() where T : class
        {
            lock (Db)
            {
                Db.DropTable<T>();
            }
        }

    }

}
