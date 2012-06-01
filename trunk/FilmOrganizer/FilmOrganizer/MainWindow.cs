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
                // TODO change info at bottom to film
                Console.WriteLine(listView1.SelectedItems[0]);
            } else {
                // TODO change info at bottom to empty 
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) {
            listView1.Items.Add("Dummy"+(new Random()).Next());
        }
    }
}
