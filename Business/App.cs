using Repository.Dbo;
using Repository.Entities;
using System.Diagnostics.Metrics;

namespace Business
{
    public class App
    {
        /// <summary>
        /// Initialisation du projet, database
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="busyTimeout"></param>
        /// <returns></returns>
        public static void Init(string databasePath, double busyTimeout = 30)
        {
            BaseDbo.Init(databasePath);
            BaseDbo.CreateTable<ShareEntity>();
            BaseDbo.CreateTable<SharePriceEntity>();
        }
    }
}