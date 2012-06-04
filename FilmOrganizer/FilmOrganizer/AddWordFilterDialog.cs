using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilmOrganizer {
    public partial class AddWordFilterDialog : Form {
        public string FilterText;
        public string FilterType;
        

        public AddWordFilterDialog() {
            InitializeComponent();
            comboBox1.Text = "Title";
        }

        private void button1_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (textBox1.Text.Trim().Length > 0) {
                FilterText = textBox1.Text;
                FilterType = comboBox1.Text;

                this.DialogResult = DialogResult.OK;
                this.Close();
            } else {
                System.Media.SystemSounds.Beep.Play();
            }

        }
    }
}
