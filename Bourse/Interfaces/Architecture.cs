using FFImageLoading.Helpers;
using Repository.Dbo;
using Repository.Entities;
using System.Runtime.InteropServices;

namespace Bourse.Interfaces
{
    /// <summary>
    /// Répertoires de l'application
    /// </summary>
    public class Architecture: IArchitecture
    {
        /// <summary>
        /// Appelé une fois pour initialisation des répertoires de l'application
        /// </summary>
        public void Init()
        {
            AppPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DbPath = Path.Combine(AppPath, "db");
            ImagePath = Path.Combine(AppPath, "images");
            FilePath = Path.Combine(AppPath, "file");
            TmpPath = Path.Combine(AppPath, "tmp");
            MapPath = Path.Combine(AppPath, "maps");

            Directory.CreateDirectory(AppPath);
            Directory.CreateDirectory(DbPath);
            Directory.CreateDirectory(FilePath);
            Directory.CreateDirectory(TmpPath);
            Directory.CreateDirectory(ImagePath);
            Directory.CreateDirectory(MapPath);

            // Le nom de la base de données est de facto celui des resources/Raw
            ShareDbo.Instance.Init(Path.Combine(DbPath,"BOURSE.sqlite"));
        }

        /// <summary>
        /// Répertoire interne de l'application
        /// </summary>
        public string AppPath { get; private set;  } = "";

        /// <summary>
        /// Répertoire de stockage des cartes
        /// </summary>
        public string MapPath { get; private set; } = "";

        /// <summary>
        /// Chemin des images 
        /// </summary>
        public string ImagePath { get; private set; } = "";

        /// <summary>
        /// Chemin de la base de données
        /// </summary>
        public string DbPath { get; private set; } = "";

        /// <summary>
        /// Chemin complet de la base de données
        /// </summary>
        public string DbFilePath { get; private set; } = "";

        /// <summary>
        /// Chemin de partage des fichiers
        /// </summary>
        public string FilePath { get; private set; } = "";

        /// <summary>
        /// Chemin temporaire des fichiers
        /// </summary>
        public string TmpPath { get; private set; } = "";
    }
}
