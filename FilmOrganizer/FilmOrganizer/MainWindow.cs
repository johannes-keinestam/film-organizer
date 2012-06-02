using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilmOrganizer {
    public partial class MainWindow : Form {
        private SettingsDialog SettingsForm = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (SettingsForm == null) {
                SettingsForm = new SettingsDialog();
            }
            SettingsForm.ShowDialog();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                groupBox1.Text = listView1.SelectedItems[0].Text;
            } else {
                groupBox1.Text = null;
                // TODO change info at bottom to empty 
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) {
            //listView1.Items.Add("Dummy"+(new Random()).Next());
            ProgramHandler.FilmCollectionHandler.GetFilmList().AddLast(new Film("Dummy" + (new Random()).Next(), 0, 0, 0));
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            listView1.Items.Clear();
            foreach (Film f in ProgramHandler.FilmCollectionHandler.GetFilmList()) {
                ListViewItem filmItem = new ListViewItem(f.Title);
                filmItem.SubItems.Add(Convert.ToString(f.Year));
                filmItem.SubItems.Add(Convert.ToString(f.Megabytes)+" MB");
                filmItem.SubItems.Add(Convert.ToString(f.Minutes)+" min");

                listView1.Items.Add(filmItem);
            }
        }
    }
}
