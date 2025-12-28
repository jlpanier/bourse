
namespace Bourse.Interfaces
{
    public interface IArchitecture
    {
        /// <summary>
        /// Appelé une fois pour initialisation des répertoires de l'application
        /// </summary>
        public void Init();

        /// <summary>
        /// Répertoire interne de l'application
        /// </summary>
        public string AppPath { get; }

        /// <summary>
        /// Répertoire de stockage des cartes
        /// </summary>
        public string MapPath { get; } 

        /// <summary>
        /// Chemin des images 
        /// </summary>
        public string ImagePath { get; } 

        /// <summary>
        /// Chemin de la base de données
        /// </summary>
        public string DbPath { get; } 

        /// <summary>
        /// Chemin complet de la base de données
        /// </summary>
        public string DbFilePath { get;  } 

        /// <summary>
        /// Chemin de partage des fichiers
        /// </summary>
        public string FilePath { get;  } 

        /// <summary>
        /// Chemin temporaire des fichiers
        /// </summary>
        public string TmpPath { get; } 
    }
}
