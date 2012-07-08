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
    public partial class SetDeviceForm : Form
    {
        public SetDeviceForm(string deviceAddress)
        {
            InitializeComponent();

            this.Text = deviceAddress;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;

                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result();
        }

        private void txbValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Result();
            }
        }

        private void Result()
        {
            if (InvalidateText(txbValue.Text))
            {
                DialogResult = DialogResult.OK;

                this.Close();
            }
            else
            {
                MessageBox.Show("入力値が不正です。");
            }
        }

        private bool InvalidateText(string numText)
        {
            int DummyNum;
            bool IsTextEnabled = true;

            if (rbDec.Checked)
            {
                if (int.TryParse(numText, out DummyNum))
                {
                }
                else
                {
                    IsTextEnabled = false;
                }
            }
            else
            {
                if (int.TryParse(numText, System.Globalization.NumberStyles.HexNumber, null, out DummyNum))
                {
                }
                else
                {
                    IsTextEnabled = false;
                }
            }

            return IsTextEnabled;
        }

        public int InputValue
        {
            get
            {
                if (rbDec.Checked)
                    return Convert.ToInt32(txbValue.Text);
                else
                    return Convert.ToInt32(txbValue.Text, 16);
            }
        }



    }
}
