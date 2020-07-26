using System;
using System.Windows.Forms;

namespace EasyPattern
{
    public partial class SaveDialogForm : Form
    {
        public SaveDialogForm()
        {
            InitializeComponent();
        }

        private void save_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
