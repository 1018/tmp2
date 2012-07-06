using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CommonClassLibrary;

namespace LocalSimulator.ProjectMaker
{
    public partial class SystemSettingForm : Form
    {        

        public SystemSettingForm()
        {
            InitializeComponent();

            string[] MakerNames = LoadAssembly.GetMakerNames(Application.StartupPath);

            for (int i = 0; i < MakerNames.Length; i++)
            {
                comboBoxMakerName.Items.Add(MakerNames[i]);
            }
            comboBoxMakerName.Text = MainForm.Instance.ProjectData.MakerName;

            Global.DeviceManager = LoadAssembly.LoadDeviceManager(Application.StartupPath, MainForm.Instance.ProjectData.MakerName);

        }  

        private void SystemSettingForm_Load(object sender, EventArgs e)
        {  
            

        }  


        private void buttonUpdate_Click(object sender, EventArgs e)
        {             
            ////ﾒｰｶｰ名取得
            //if (comboBoxMakerName.SelectedItem == null)
            //{
            //    MessageBox.Show("メーカ名が指定されていません。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //MainForm.Instance.ProjectData.SystemSetting.MakerName = (string)comboBoxMakerName.SelectedItem;

            //this.Close();
            
        }

        private void buttonCansel_Click(object sender, EventArgs e)
        {            
            //this.Close();
        }

        private void comboBoxMakerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ﾒｰｶｰ名取得
            if (comboBoxMakerName.SelectedItem == null)
            {
                MessageBox.Show("メーカ名が指定されていません。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainForm.Instance.ProjectData.MakerName = (string)comboBoxMakerName.SelectedItem;

            Global.DeviceManager = LoadAssembly.LoadDeviceManager(Application.StartupPath, MainForm.Instance.ProjectData.MakerName);


        }

        private void SystemSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void SystemSettingForm_Activated(object sender, EventArgs e)
        {
            MainForm.Instance.PropertyView.SelectedObject = null;
        }

        
    }
}
