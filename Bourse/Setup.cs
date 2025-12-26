using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse
{
    public static class Setup
    {
        /// <summary>
        /// Répertoire interne de l'application
        /// </summary>
        public static string AppPath { get; private set; } = "";

        /// <summary>
        /// Chemin des images 
        /// </summary>
        public static string ImagePath { get; private set; } = "";

        /// <summary>
        /// Chemin de la base de données
        /// </summary>
        public static string DbPath { get; private set; } = "";

        /// <summary>
        /// Chemin complet de la base de données
        /// </summary>
        public static string DbFilePath { get; private set; } = "";

        /// <summary>
        /// Chemin de partage des fichiers
        /// </summary>
        public static string FilePath { get; private set; } = "";

        /// <summary>
        /// Chemin temporaire des fichiers
        /// </summary>
        public static string TmpPath { get; private set; } = "";

        /// <summary>
        /// Initialisation du projet
        /// </summary>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public static void Init(string projectname)
        {
            var rootdir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            AppPath = Path.Combine(rootdir, projectname);
            DbPath = Path.Combine(AppPath, "db");
            DbFilePath = Path.Combine(DbPath, $"{projectname.ToUpper()}.sqlite");
            ImagePath = Path.Combine(AppPath, "images");
            FilePath = Path.Combine(AppPath, "file");
            TmpPath = Path.Combine(AppPath, "tmp");

            Directory.CreateDirectory(AppPath);
            Directory.CreateDirectory(DbPath);
            Directory.CreateDirectory(FilePath);
            Directory.CreateDirectory(TmpPath);
            Directory.CreateDirectory(ImagePath);

            App.Init(DbFilePath);
        }
    }
}
