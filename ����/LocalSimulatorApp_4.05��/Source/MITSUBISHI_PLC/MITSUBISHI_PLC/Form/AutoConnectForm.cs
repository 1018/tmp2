using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MITSUBISHI_PLC
{
    public partial class AutoConnectForm : Form
    {
        public AutoConnectForm()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Break = true;
            this.Close();
        }

        private void AutoConnectForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Break = true;
            }
        }

        public bool Break { get; private set; }

    }
}
