using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalSimulator.MainProgram
{
    public partial class BaseFormListForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public BaseFormListForm()
        {
            InitializeComponent();

            SetBaseFormListHeigth(30);
        }

        public void SetDisplayBaseForms(IEnumerable<string> baseFormNames)
        {
            // BaseForm選択リストに表示
            foreach (string name in baseFormNames)
            {
                this.BaseFormList.Items.Add(name);
            }
        }

        public void ClearDisplayBaseForms()
        {
            this.BaseFormList.Items.Clear();
        }

        private void BaseFormList_SizeChanged(object sender, EventArgs e)
        {
            // カラム幅を自動で調整

            int listWidth = BaseFormList.Width;

            BaseFormList.Columns[0].Width = listWidth;
        }

        private void SetBaseFormListHeigth(int heigth)
        {
            ImageList imageListSmall = new ImageList();
            imageListSmall.ImageSize = new Size(1, heigth);
            this.BaseFormList.SmallImageList = imageListSmall;
        }
    }
}
