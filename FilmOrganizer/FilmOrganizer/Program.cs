using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.IO;

namespace FilmOrganizer {
    static class Program {
        public static MainWindow GUI;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GUI = new MainWindow();
            Application.Run(GUI);
        }
    }

    static class ProgramHandler {
        public static readonly FilmCollection FilmCollectionHandler = new FilmCollection(FilmCollection.ScanStyle.DoNothing);
        public static readonly StringCollection PathCollection = Properties.Settings.Default.Folders;
    }

    public interface Scanner {
        void Scan();
    }

    public class PerFolderScanner : Scanner {
        public void Scan() {
            foreach (string path in ProgramHandler.PathCollection) {
                DirectoryInfo[] filmDirectories = new DirectoryInfo(path).GetDirectories();
                int progressCounter = 0;
                foreach (DirectoryInfo folder in filmDirectories) {
                    AddFolder(folder);
                    Program.GUI.ReportImportProgress(progressCounter++, filmDirectories.Length, path);
                }
            }
        }
        private void AddFolder(DirectoryInfo folder) {
            IFilmInfoGenerator gen = new FolderInfoGenerator(folder);
            Film filmItem = new Film(gen.GetTitle(), gen.GetPath(), gen.GetYear(), gen.GetMinutes(), gen.GetMegabytes());

            if (FilmScannerFilter.Accepted(filmItem)) {
                ProgramHandler.FilmCollectionHandler.GetFilmList().AddLast(filmItem);
            }
            Console.WriteLine("Scanner: Added film " + gen.GetTitle());
        }
    }
}
