using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace FilmOrganizer {
    public class FilmCollection {
        public enum ScanStyle { DoNothing, ImportAll, ImportDummy, ImportChanged };
        private LinkedList<Film> Films = new LinkedList<Film>();

        public FilmCollection(ScanStyle initialization) {
            if (initialization != ScanStyle.DoNothing) {
                Populate(initialization);
            }
            switch (initialization) {
                case ScanStyle.ImportAll:
                    Populate(ScanStyle.ImportAll);
                    break;
                case ScanStyle.ImportDummy:
                    Films.AddLast(new Film("Film1", "C:/Film1", 1991, 93, 700));
                    Films.AddLast(new Film("Film2", "D:/DL/Film2", 2011, 121, 12000));
                    Films.AddLast(new Film("ShortFilm1", "F:/Films/ShortFilm1", 1923, 16, 1400));
                    break;
            }
        }

        public void Populate(ScanStyle scanSettings) {
            if (scanSettings == ScanStyle.ImportDummy) {
                Films.AddLast(new Film("Film1", "C:/Film1", 1991, 93, 700));
                Films.AddLast(new Film("Film2", "D:/DL/Film2", 2011, 121, 12000));
                Films.AddLast(new Film("ShortFilm1", "F:/Films/ShortFilm1", 1923, 16, 1400));
            } else if (scanSettings == ScanStyle.ImportAll) {
                Films.Clear();
                new PerFolderScanner().Scan();
            } else if (scanSettings == ScanStyle.ImportChanged) {
                // TODO
            }
        }

        public LinkedList<Film> GetFilmList() {
            return Films;
        }

    }
    public class Film {
        public readonly string Path;
        public readonly int Year;
        public readonly int Minutes;
        public readonly long Megabytes;
        public readonly string Title;

        public Film(string title, string path, int year, int minutes, long megabytes) {
            this.Title = title;
            this.Path = path;
            this.Year = year;
            this.Minutes = minutes;
            this.Megabytes = megabytes;
        }
    }

    public interface IFilmInfoGenerator {
        string GetTitle();
        int GetYear();
        int GetMinutes();
        string GetPath();
        long GetMegabytes();
    }

    public class DummyGenerator : IFilmInfoGenerator {
        private readonly string Path;
        public DummyGenerator(string path) {
            this.Path = path;    
        }

        public string GetTitle() {
            return Path;
        }

        public int GetYear() {
            return 0;
        }

        public int GetMinutes() {
            return 0;
        }

        public string GetPath() {
            return Path;
        }

        public long GetMegabytes() {
            return 0;
        }
    }

    public class FolderInfoGenerator : IFilmInfoGenerator {
        private readonly string Path;
        private readonly string Title;
        private readonly int Year;
        private readonly int Minutes;
        private readonly long Megabytes;

        public FolderInfoGenerator(DirectoryInfo path) {
            this.Path = path.FullName;
            this.Title = path.ToString();
            this.Year = 0;
            this.Minutes = 0;
            this.Megabytes = 0;

            MatchCollection yearMatch = Regex.Matches(this.Path, "(20\\d{2}|19\\d{2})");
            if (yearMatch.Count > 0) {
                string lastYearMatch = yearMatch[yearMatch.Count - 1].ToString();
                this.Year = Convert.ToInt16(lastYearMatch);
                this.Title = Title.Substring(0, Title.IndexOf(lastYearMatch));
            }

            StringBuilder titleBuilder = new StringBuilder();
            foreach (char c in Title) {
                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)) {
                    titleBuilder.Append(c);
                } else if (Char.IsPunctuation(c)) {
                    titleBuilder.Append(' ');
                }
            }
            this.Title = titleBuilder.ToString().Trim();

            FileInfo largestFile = null;
            long largestFileSize = 0;
            foreach (FileInfo folderFile in path.EnumerateFiles("*", SearchOption.AllDirectories)) {
                long fileSize = folderFile.Length;
                Megabytes += (fileSize / 1000000);
                if (fileSize > largestFileSize) {
                    largestFile = folderFile;
                    largestFileSize = fileSize;
                }
            }

            this.Minutes = (int)(MediaInfoWrapper.GetFilmDuration(GetFilmFilePath(largestFile))/60000);
        }

        private string GetFilmFilePath(FileInfo largestFile) {
            string filmFilePath = null;
            string extension = largestFile.Extension.ToLower();

            if (extension == ".vob") {
                string filePath = largestFile.FullName;
                filmFilePath = filePath.Substring(0, filePath.LastIndexOf("_")) + "_0.IFO";
            } else if (extension == ".iso" || extension == ".img") {
            } else {
                filmFilePath = largestFile.FullName;
            }
            return filmFilePath;
        }

        public long GetMegabytes() {
            return Megabytes;
        }

        public string GetTitle() {
            return Title;
        }

        public int GetYear() {
            return Year;
        }

        public int GetMinutes() {
            return Minutes;
        }

        public string GetPath() {
            return Path;
        }
    }

    public class FilmScannerFilter {
        public static bool Accepted(Film f) {
            return true;
        }
    }

    public class MediaInfoWrapper {
        private static Process MediaInfo;
        private static readonly string DurationArguments = "--Inform=Video;%Duration% ";

        private static void InitializeInstance() {
            MediaInfo = new Process();
            MediaInfo.StartInfo.FileName = "MediaInfo.exe";
            MediaInfo.StartInfo.UseShellExecute = false;
            MediaInfo.StartInfo.CreateNoWindow = true;
            MediaInfo.StartInfo.RedirectStandardOutput = true;
        }

        public static long GetFilmDuration(string path) {
            if (MediaInfo == null) {
                InitializeInstance();
            }
            if (path == null) {
                return 0;
            } else {
                MediaInfo.StartInfo.Arguments = DurationArguments + "\"" + path + "\"";
                MediaInfo.Start();

                string output = MediaInfo.StandardOutput.ReadToEnd();
                MediaInfo.WaitForExit();
                long parsed;
                return long.TryParse(output.Trim(), out parsed) ? parsed : 0;
            }
        }

    }
}
