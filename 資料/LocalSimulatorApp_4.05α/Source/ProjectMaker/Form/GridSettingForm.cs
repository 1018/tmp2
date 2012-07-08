using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalSimulator.ProjectMaker
{
    public partial class GridSettingForm : Form
    {
        public GridSettingForm()
        {
            InitializeComponent();

            if (MainCtrl.GridSize.Width != 0) { this.TextWidth.Text = MainCtrl.GridSize.Width.ToString(); }
            if (MainCtrl.GridSize.Height != 0) { this.TextHeight.Text = MainCtrl.GridSize.Height.ToString(); }

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt16(this.TextWidth.Text) <= 0 || Convert.ToInt16(this.TextWidth.Text) > 50) { return; }
            if (Convert.ToInt16(this.TextHeight.Text) <= 0 || Convert.ToInt16(this.TextHeight.Text) > 50) { return; }

            MainCtrl.GridSize = new Size(Convert.ToInt16(this.TextWidth.Text), 
                                         Convert.ToInt16(this.TextHeight.Text));
            this.Dispose();
        }       

    }
}
