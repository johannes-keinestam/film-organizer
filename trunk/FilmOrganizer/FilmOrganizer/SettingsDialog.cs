using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilmOrganizer {
    public partial class SettingsDialog : Form {

        public SettingsDialog() {
            InitializeComponent();
            UpdateFolderList();
        }

        private void SettingsDialog_Load(object sender, EventArgs e) {

        }

        private void UpdateFolderList() {
            FolderListView.Items.Clear();
            foreach (string folder in Properties.Settings.Default.Folders) {
                FolderListView.Items.Add(folder);
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e) {
            DialogResult FolderResult = this.folderBrowseDialog.ShowDialog();
            string ChosenFolder = folderBrowseDialog.SelectedPath;
            if (FolderResult.HasFlag(DialogResult.OK) && IsAllowedFolder(ChosenFolder)) {
                Properties.Settings.Default.Folders.Add(ChosenFolder);
                UpdateFolderList();
            }
        }

        private bool IsAllowedFolder(string NewPath) {
            foreach (ListViewItem FolderItem in FolderListView.Items) {
                string ExistingPath = FolderItem.Text;
                int DeepestPathLength = Math.Min(NewPath.Length, ExistingPath.Length);
                string ExistingSharedPath = ExistingPath.Substring(0, DeepestPathLength);
                string NewSharedPath = NewPath.Substring(0, DeepestPathLength);

                if (ExistingSharedPath.Equals(NewSharedPath)) {
                    return false;
                }
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e) {
            foreach (ListViewItem SelectedFolder in FolderListView.SelectedItems) {
                Properties.Settings.Default.Folders.Remove(SelectedFolder.Text);
            }
            UpdateFolderList();
        }

        private void button2_Click(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            this.Close();
        }

    }
}
