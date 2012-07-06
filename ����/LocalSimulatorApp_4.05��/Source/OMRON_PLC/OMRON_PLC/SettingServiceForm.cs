using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OMRON_PLC
{
    public partial class SettingServiceForm : Form
    {
        public SettingServiceForm()
        {
            InitializeComponent();
        }

        public SettingServiceForm(IEnumerable<string> displayNames) : this()
        {
            DisplayNames(displayNames);
        }

        public void DisplayNames(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                CheckBox checkBox = new CheckBox();
                int controlIndex = this.flowLayoutPanel1.Controls.Count;
                    
                checkBox.Name = controlIndex.ToString();
                checkBox.Text = name;

                checkBox.Appearance = Appearance.Normal;
                checkBox.AutoCheck = true;
                checkBox.AutoSize = true;
                checkBox.CheckAlign = ContentAlignment.MiddleLeft;
                checkBox.CheckState = CheckState.Unchecked;
                checkBox.ThreeState = false;

                this.flowLayoutPanel1.Controls.Add(checkBox);
            }
        }

        public void SetCheckNames(IEnumerable<string> names)
        {
            foreach (Control control in this.flowLayoutPanel1.Controls)
            {
                foreach (string name in names)
                {
                    if (control.Text == name)
                    {
                        CheckBox checkBox = control as CheckBox;
                        if (checkBox != null)
                        {
                            checkBox.CheckState = CheckState.Checked;
                        }
                    }
                }
            }
        }

        public IEnumerable<string> GetCheckNames()
        {
            List<string> checkNameList = new List<string>();

            foreach (Control control in this.flowLayoutPanel1.Controls)
            {
                CheckBox checkBox = control as CheckBox;

                if ((checkBox != null) && (checkBox.CheckState == CheckState.Checked))
                {
                    checkNameList.Add(checkBox.Text);
                }
            }

            return checkNameList;
        }
    }
}
