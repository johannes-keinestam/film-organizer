using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace FilmOrganizer {
    public partial class MainWindow : Form {
        private SettingsDialog SettingsForm = null;
        private ListViewColumnSorter Sorter = new ListViewColumnSorter();
        private FilmDisplayFilter FilmListFilter = new FilmDisplayFilter();
        private bool filteringEnabled = false;
        public enum Columns { Film, Year, Size, Length, Path }

        public MainWindow() {
            InitializeComponent();
            this.filmListView.ListViewItemSorter = Sorter;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (SettingsForm == null) {
                SettingsForm = new SettingsDialog();
                SettingsForm.StartPosition = FormStartPosition.CenterParent;
            }
            SettingsForm.ShowDialog();
        }

        private void filmListView_SelectedIndexChanged(object sender, EventArgs e) {
            if (filmListView.SelectedItems.Count > 0) {
                groupBox1.Text = filmListView.SelectedItems[0].Text;
            } else {
                groupBox1.Text = null;
                // TODO change info at bottom to empty 
            }
        }

        private void filmListView_ItemActivate(Object sender, EventArgs e) {
            string path = (sender as ListView).SelectedItems[0].SubItems[(int)Columns.Path].Text;
            Process.Start(path);
//             Process mediaInfoInstance = new Process();
//             mediaInfoInstance.StartInfo.FileName = "MediaInfo.exe";
//             mediaInfoInstance.StartInfo.Arguments = "--Inform=Video;%Duration% " + @"D:\De.Gröna.Slaktarna.2003.Swesub.DVDrip.Royskatt.avi";
//             mediaInfoInstance.StartInfo.UseShellExecute = false;
//             mediaInfoInstance.StartInfo.CreateNoWindow = true;
//             mediaInfoInstance.StartInfo.RedirectStandardOutput = true;
//             mediaInfoInstance.Start();
// 
//             string output = mediaInfoInstance.StandardOutput.ReadToEnd();
//             mediaInfoInstance.WaitForExit();
//             Console.WriteLine(output);
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            this.toolStripStatusLabel1.Text = "Initializing import...";
            toolStripButton2.Enabled = false;
            toolStripProgressBar1.Visible = true;
            filmListView.Items.Clear();
            backgroundWorker1.RunWorkerAsync();
        }

        private void thread_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
            this.toolStripStatusLabel1.Text = "Importing path "+e.UserState+"...";
        }

        private void thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            PopulateFilmList();
            toolStripProgressBar1.Visible = false;
            toolStripButton2.Enabled = true;
        }

        private void PopulateFilmList() {
            filmListView.Items.Clear();
            int filteredFilms = 0;
            foreach (Film f in ProgramHandler.FilmCollectionHandler.GetFilmList()) {
                if (!filteringEnabled || (filteringEnabled && FilmListFilter.Accepted(f))) {
                    ListViewItem filmItem = new ListViewItem(f.Title);
                    filmItem.SubItems.Add(Convert.ToString(f.Year));
                    filmItem.SubItems.Add(Convert.ToString(f.Megabytes) + " MB");
                    filmItem.SubItems.Add(Convert.ToString(f.Minutes) + " min");
                    filmItem.SubItems.Add(f.Path);

                    filmListView.Items.Add(filmItem);
                } else {
                    filteredFilms++;
                }

            }
            toolStripStatusLabel1.Text = filmListView.Items.Count + " films";
            if (filteringEnabled) {
                toolStripStatusLabel1.Text += " (" + filteredFilms + " hidden)";
            }

            filmListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            Sorter.SortColumn = (int)Columns.Film;
            Sorter.Order = SortOrder.Ascending;
            this.filmListView.Sort();
        }

        private void filmListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            if (e.Column == Sorter.SortColumn) {
                if (Sorter.Order == SortOrder.Ascending) {
                    Sorter.Order = SortOrder.Descending;
                } else {
                    Sorter.Order = SortOrder.Ascending;
                }
            } else {
                Sorter.SortColumn = e.Column;
                Sorter.Order = SortOrder.Ascending;
            }

            this.filmListView.Sort();
        }

        public void ReportImportProgress(int progress, int max, string fromPath) {
            backgroundWorker1.ReportProgress(progress * 100 / max, fromPath);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            ProgramHandler.FilmCollectionHandler.Populate(FilmCollection.ScanStyle.ImportAll);
        }

        private void toolStripButton3_Click(object sender, EventArgs e) {
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            foreach (Control c in panel1.Controls) {
                c.Enabled = filterEnabledCheckbox.Checked;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            AddWordFilterDialog dialog = new AddWordFilterDialog();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog() == DialogResult.OK) {
                ListViewItem filterItem = new ListViewItem(dialog.FilterText);
                filterItem.SubItems.Add(dialog.FilterType);
                filterListView.Items.Add(filterItem);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            foreach (ListViewItem selected in filterListView.SelectedItems) {
                selected.Remove();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (filterEnabledCheckbox.Checked) {
                filteringEnabled = true;
                FilmListFilter.MaximumLength = Convert.ToInt32(maxLengthTick.Value);
                FilmListFilter.MinimumLength = Convert.ToInt32(minLengthTick.Value);
                FilmListFilter.MaximumYear = Convert.ToInt32(maxYearTick.Value);
                FilmListFilter.MinimumYear = Convert.ToInt32(minYearTick.Value);
                FilmListFilter.MaximumSize = Convert.ToInt32(maxSizeTick.Value);
                FilmListFilter.MinimumSize = Convert.ToInt32(minSizeTick.Value);

                FilmListFilter.Paths.Clear();
                FilmListFilter.Titles.Clear();
                foreach (ListViewItem filterItem in filterListView.Items) {
                    bool isTitleFilter = (filterItem.SubItems[0].Text == "Title");
                    if (isTitleFilter) {
                        FilmListFilter.Titles.Add(filterItem.Text);
                    } else {
                        FilmListFilter.Paths.Add(filterItem.Text);
                    }
                }
                
            } else {
                filteringEnabled = false;
            }

            PopulateFilmList();
        }
    }

    class ListViewColumnSorter : IComparer {
        /** Adapted from Microsoft's website, http://support.microsoft.com/kb/319401 */
        private int ColumnToSort;
        private SortOrder OrderOfSort;
        private CaseInsensitiveComparer ObjectCompare;

        public ListViewColumnSorter() {
            ColumnToSort = 0;
            OrderOfSort = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        public int Compare(object x, object y) {
            int compareResult;
            ListViewItem listviewX, listviewY;

            listviewX = x as ListViewItem;
            listviewY = y as ListViewItem;

            if (ColumnToSort == (int)MainWindow.Columns.Path || ColumnToSort == (int)MainWindow.Columns.Film) {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            } else {
                int xValue = int.Parse(System.Text.RegularExpressions.Regex.Match(listviewX.SubItems[ColumnToSort].Text, @"\d+").Value);
                int yValue = int.Parse(System.Text.RegularExpressions.Regex.Match(listviewY.SubItems[ColumnToSort].Text, @"\d+").Value);
                compareResult = xValue > yValue ? 1 : -1;
                compareResult = xValue == yValue ? 0 : compareResult;
            }
            

            if (OrderOfSort == SortOrder.Ascending) {
                return compareResult;
            } else if (OrderOfSort == SortOrder.Descending) {
                return (-compareResult);
            } else {
                return 0;
            }
        }

        public int SortColumn {
            set {
                ColumnToSort = value;
            }
            get {
                return ColumnToSort;
            }
        }
        public SortOrder Order {
            set {
                OrderOfSort = value;
            }
            get {
                return OrderOfSort;
            }
        }

    }

    class FilmDisplayFilter {
        public List<string> Titles = new List<string>();
        public List<string> Paths = new List<string>();
        public int MinimumSize = 0;
        public int MaximumSize = int.MaxValue;
        public int MinimumYear = 1900;
        public int MaximumYear = 2099;
        public int MinimumLength = 0;
        public int MaximumLength = int.MaxValue;

        public bool Accepted(Film film) {
            foreach (string filter in Titles) {
                if (!film.Title.Contains(filter)) {
                    return false;
                }
            }
            foreach (string filter in Paths) {
                if (!film.Path.Contains(filter)) {
                    return false;
                }
            }

            if (film.Minutes > MaximumLength || film.Minutes < MinimumLength) {
                return false;
            }

            if (film.Year > MaximumYear || film.Year < MinimumYear) {
                return false;
            }

            if (film.Megabytes > MaximumSize || film.Megabytes < MinimumSize) {
                return false;
            }

            return true;
        }
    }
}
