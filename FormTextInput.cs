using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MMC
{
    public partial class FormTextInput : Form
    {
        public string Answer;

        public FormTextInput(string Query)
        {
            InitializeComponent();
            labelQuery.Text = Query;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Answer = textBoxAnswer.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
