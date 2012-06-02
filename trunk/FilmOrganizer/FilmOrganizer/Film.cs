using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmOrganizer {
    public class FilmCollection {
        //private List
    }
    private class Film {
        private string Path;
        private int Year;
        private int Minutes;
        private string Title;

        public Film(string path, int year, int minutes) {
            this.Path = path;
            this.Year = year;
            this.Minutes = minutes;

            GenerateTitle();
        }

        private void GenerateTitle() {
            DummyGenerator.GenerateTitle(Path);
        }
    }

    public interface ITitleGenerator {
        public static string GenerateTitle(string path);
    }

    private class DummyGenerator : ITitleGenerator {
        public static string GenerateTitle(string path) {
            string name = path;
            return name;
        }
    }
}
