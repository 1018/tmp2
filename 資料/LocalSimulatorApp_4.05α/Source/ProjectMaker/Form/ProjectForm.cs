using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using CommonClassLibrary;
using System.Xml.Serialization;
using System.IO;

namespace LocalSimulator.ProjectMaker
{
    public partial class ProjectForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ProjectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ベースフォーム一覧を再作成します。
        /// </summary>
        public void Redraw()
        {
            // プロジェクトが開かれていない
            if (MainForm.Instance.ProjectData == null)
            {
                return;
            }

            ProjectWindow.BeginUpdate();

            ProjectWindow.Nodes.Clear();

            List<TreeNode> treeNodeChildren = new List<TreeNode>();

            TreeNode tnSystemSetting = new TreeNode("システム設定");
            tnSystemSetting.Tag = "SystemSetting";
            treeNodeChildren.Add(tnSystemSetting);

            MainForm.Instance.ProjectData.BaseFormData.Sort(delegate(BaseForm frm1, BaseForm frm2) { return frm1.Number - frm2.Number; });

            foreach (BaseForm ProjectFormData in MainForm.Instance.ProjectData.BaseFormData)
            {
                string number = ProjectFormData.Number.ToString().PadLeft(4, '0');
                string title = ProjectFormData.TitleName;

                TreeNode treeNodeBaseForm = new TreeNode(number + " - " + title);
                treeNodeBaseForm.ContextMenuStrip = TreeContextMenu;
                treeNodeBaseForm.Tag = "BaseForm";
                treeNodeChildren.Add(treeNodeBaseForm);
            }

            string ProjectName = MainForm.Instance.ProjectData.ProjectName;
            TreeNode treeNodeProject = new TreeNode(ProjectName, treeNodeChildren.ToArray());
            treeNodeProject.Tag = "Project";
            treeNodeProject.ContextMenuStrip = TreeContextMenu;

            ProjectWindow.Nodes.Add(treeNodeProject);

            ProjectWindow.TopNode.Expand();

            ProjectWindow.Refresh();

            ProjectWindow.EndUpdate();

        }

        private void ProjectWindow_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((string)e.Node.Tag == "SystemSetting")
            {
                MainForm.Instance.SystemSettingForm.Show();
                MainForm.Instance.SystemSettingForm.Activate();
            }
            else if ((string)e.Node.Tag == "BaseForm")
            {
                int number = Convert.ToInt32(e.Node.Text.Substring(0, 4));

                //ダブルクリックされたフォームをアクティブにする。
                foreach (BaseForm myForm in MainForm.Instance.ProjectData.BaseFormData)
                {
                    if (myForm.Number == number)
                    {
                        myForm.Show();
                        myForm.Activate();
                    }


                }
            }
        }

        #region コンテキストメニュー

        private void CutMenu_Click(object sender, EventArgs e)
        {
            BaseForm cutForm = MatchForm();


            MainForm.Instance.ClipBoardBaseForm = cutForm.Serialize();

            MainForm.Instance.ProjectData.BaseFormData.Remove(cutForm);
            cutForm.Close();

            Redraw();

        }

        private void CopyMenu_Click(object sender, EventArgs e)
        {
            BaseForm copyForm = MatchForm();

            MainForm.Instance.ClipBoardBaseForm = copyForm.Serialize();
        }

        private void PasteMenu_Click(object sender, EventArgs e)
        {
            //空きナンバーを検索する
            int number = 0;

            for (int i = 0; i < MainForm.Instance.ProjectData.BaseFormData.Count; i++)
            {
                if (((BaseForm)MainForm.Instance.ProjectData.BaseFormData[i]).Number == number)
                {
                    number++;
                    i = 0;
                }
            }

            //Deserialize
            //ベースフォームデシリアライズ
            BaseForm frm = (BaseForm)SerializeSupport.Deserialize(MainForm.Instance.ClipBoardBaseForm, typeof(BaseForm));
            frm.ShapeCollection.Clear();
            frm.Deserialize(MainForm.Instance.ClipBoardBaseForm);
            frm.Number = number;
            frm.MdiParent = MainForm.Instance;

            MainForm.Instance.ProjectData.BaseFormData.Add(frm);

            frm.Show();

            Redraw();
        }

        private void DeleteMenu_Click(object sender, EventArgs e)
        {
            BaseForm delfrm = MatchForm();

            MainForm.Instance.ProjectData.BaseFormData.Remove(delfrm);
            delfrm.Close();

            Redraw();
        }

        #endregion

        private void ProjectWindow_MouseDown(object sender, MouseEventArgs e)
        {
            Point hPoint;
            TreeNode hNode;

            hPoint = ProjectWindow.PointToClient(Cursor.Position);
            hNode = ProjectWindow.GetNodeAt(hPoint);

            ProjectWindow.SelectedNode = hNode;

        }


        /// <summary>
        /// 現在選択されているTreeNodeが示すBaseFormを取得します。
        /// </summary>
        /// <returns></returns>
        private BaseForm MatchForm()
        {
            TreeNode obj = ProjectWindow.SelectedNode;

            int number = Convert.ToInt32(obj.Text.Substring(0, 4));

            var baseForms = MainForm.Instance.ProjectData.BaseFormData;
            BaseForm myfrm = baseForms.Single((frm) => frm.Number == number);

            return myfrm;

        }

        private void TreeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            TreeNode obj = ProjectWindow.SelectedNode;
            if ((string)obj.Tag == "Project")
            {
                CutMenu.Visible = false;
                CopyMenu.Visible = false;
                DeleteMenu.Visible = false;
                MenuSeparator.Visible = false;
                if (MainForm.Instance.ClipBoardBaseForm != null)
                {
                    PasteMenu.Visible = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else if ((string)obj.Tag == "BaseForm")
            {
                CutMenu.Visible = true;
                CopyMenu.Visible = true;
                PasteMenu.Visible = MainForm.Instance.ClipBoardBaseForm != null;
                DeleteMenu.Visible = true;
            }

        }

        private void ProjectWindow_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Level != 0) { e.CancelEdit = true; }
        }

        private void ProjectWindow_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Level != 0) { e.CancelEdit = true; }

            if (String.IsNullOrEmpty(e.Label) == true) { e.CancelEdit = true; }

            MainForm.Instance.ProjectData.ProjectName = e.Label;

        }

        public void ExportMenu_Click(object sender, EventArgs e)
        {
            string filePath = null;

            TreeNode obj = ProjectWindow.SelectedNode;

            if (obj == null)
            {
                MessageBox.Show("BaseFormを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (obj.Tag.ToString() != "BaseForm")
            {
                MessageBox.Show("BaseFormを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            BaseForm exportData = MatchForm();

            if (MainForm.Instance.GetSaveFileName(ref filePath, exportData.Number.ToString().PadLeft(4, '0'), "xml") == false) { return; }


            //シリアライズ用クラスの作成            

            //ベースフォームシリアライズ


            BaseFormDataSerializeFormat serializeData = exportData.Serialize();

            Type[] et = new Type[] { typeof(string[]) };

            //XmlSerializerオブジェクトを作成。書き込むオブジェクトの型を指定する            
            XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataSerializeFormat), et);

            //ファイルを開く
            FileStream fs = new FileStream(filePath, FileMode.Create);

            //シリアル化し、XMLファイルに保存する
            serializer.Serialize(fs, serializeData);

            //閉じる
            fs.Close();

            MessageBox.Show("エクスポート完了");

        }

        private void ProjectWindow_Enter(object sender, EventArgs e)
        {
            //ShapeToolStrip選択を解除
            MainForm.Instance.LineToolStripButton.Checked = false;
            MainForm.Instance.LinesToolStripButton.Checked = false;
            MainForm.Instance.CircleToolStripButton.Checked = false;
            MainForm.Instance.SquareToolStripButton.Checked = false;
            MainForm.Instance.PolygonToolStripButton.Checked = false;
            MainForm.Instance.TextToolStripButton.Checked = false;

            //ListViewの選択を解除
            if (MainForm.Instance.SymbolListView.FocusedItem != null)
            {
                ListViewItem SelectedItem = MainForm.Instance.SymbolListView.Items[MainForm.Instance.SymbolListView.FocusedItem.Index];
                SelectedItem.Selected = false;

            }
        }
    }
}
