using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonClassLibrary;

namespace LocalSimulator.ProjectMaker
{
    public partial class ImportProjectForm : Form
    {

        ProjectDataSerializeFormat ImportData;

        public ImportProjectForm(ProjectDataSerializeFormat loadData)
        {
            InitializeComponent();

            ImportData = loadData;

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            foreach (string obj in this.listBox1.SelectedItems)
            {
                //インポートデータから一致するフォームをロードする。
                foreach (BaseFormDataSerializeFormat baseFormData in ImportData.BaseFormData)
                {
                    if (baseFormData.Text.ToString() == obj)
                    {                        
                        //ベースフォームデシリアライズ
                        BaseForm myForm = (BaseForm)SerializeSupport.Deserialize(baseFormData, typeof(BaseForm));

                        //空きナンバーを検索する
                        for (int i = 0; i < MainForm.Instance.ProjectData.BaseFormData.Count(); i++)
                        {
                            if (((BaseForm)MainForm.Instance.ProjectData.BaseFormData[i]).Number == myForm.Number)
                            {
                                myForm.Number++;
                                i = 0;
                            }
                        }

                        myForm.ShapeCollection = new OriginalCollection<ShapeObject>();
                        myForm.Deserialize(baseFormData);
                        MainForm.Instance.ProjectData.BaseFormData.Add(myForm);
                        myForm.Hide();
                        myForm.MdiParent = MainForm.Instance;

                        
                    }
                }

            }

            MainForm.Instance.ProjectForm.Redraw();
            this.Close();

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
