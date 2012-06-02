using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmOrganizer {
    public class FilmCollection {
        public enum InitStyle { None, ImportAll, ImportDummy };
        private LinkedList<Film> films = new LinkedList<Film>();

        public FilmCollection(InitStyle initialization) {
            switch (initialization) {
                case InitStyle.ImportAll:
                    Populate();
                    break;
                case InitStyle.ImportDummy:
                    films.AddLast(new Film("C:/Film1", 1991, 93, 700));
                    films.AddLast(new Film("D:/DL/Film2", 2011, 121, 12000));
                    films.AddLast(new Film("F:/Films/ShortFilm1", 1923, 16, 1400));
                    break;
            }
        }

        private void Populate() {
            // TODO, add from paths
        }

        public LinkedList<Film> GetFilmList() {
            return films;
        }

    }
    public class Film {
        public readonly string Path;
        public readonly int Year;
        public readonly int Minutes;
        public readonly int Megabytes;
        public readonly string Title;

        public Film(string path, int year, int minutes, int megabytes) {
            this.Path = path;
            this.Year = year;
            this.Minutes = minutes;
            this.Megabytes = megabytes;

            Title = new DummyGenerator().GenerateTitle(Path);
        }
    }

    public interface ITitleGenerator {
        string GenerateTitle(string path);
    }

    public class DummyGenerator : ITitleGenerator {
        public string GenerateTitle(string path) {
            string name = path;
            return name;
        }
    }
}
