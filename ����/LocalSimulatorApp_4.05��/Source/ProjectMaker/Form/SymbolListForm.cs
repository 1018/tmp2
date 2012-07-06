using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CommonClassLibrary;
using System.Reflection;
using System.Threading;

namespace LocalSimulator.ProjectMaker
{
    public partial class SymbolListForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private int IndexNum = 1;
        private int dllCount;
        private Form runningForm = null;
        private Label runningLabel = null;
        GourListViewItemCollection SymbolViewItemCollection = new GourListViewItemCollection(); 

        public SymbolListForm()
        {
            InitializeComponent();            
        }

        #region シンボルサムネイルリスト作成

        public void CreateThumbnailList()
        {
            //Panel nonPanel = new Panel();

            //MainForm.Instance.Controls.Add(nonPanel);

            Panel nonPanel = MainForm.Instance.nonPanel;

            //進捗フォーム
            Thread thread = new Thread(new ThreadStart(ThumbnailThread));
            thread.IsBackground = true;
            thread.Start();

            this.SymbolListView.BeginUpdate();
            string[] DllFiles = Directory.GetFiles(AppSetting.SymbolPath, "*.dll");
            Array.Sort(DllFiles);
            dllCount = DllFiles.Length;

            //----------------サムネイルリスト作成-----------------//
            int width = 80;
            int height = 80;
            SymbolImageList.ImageSize = new Size(width, height);
            
            foreach (string DllFile in DllFiles)
            {
                try
                {
                    if (runningLabel != null)
                    {
                        runningLabel.Invoke((MethodInvoker)delegate()
                        {
                            runningLabel.Text = "シンボルサムネイル作成中 "
                                                 + IndexNum.ToString().PadLeft(3, '0')
                                                 + " / " + dllCount.ToString().PadLeft(3, '0');
                        });
                    }
                }
                catch { }

                Assembly Asm = Assembly.LoadFrom(DllFile);
                Type DllType = Asm.GetType(AppSetting.SymbolNameSpace + "." + Path.GetFileNameWithoutExtension(DllFile) + "_Draw");

                if (DllType != null)
                {

                    Symbol_Draw Symbol = (Symbol_Draw)Activator.CreateInstance(DllType);
                    Symbol.Zoom = 1;

                    nonPanel.Controls.Add(Symbol);

                    //コントロールの外観を描画するBitmapの作成
                    Bitmap bmp = new Bitmap(Symbol.Size.Width, Symbol.Size.Height);

                    //キャプチャする
                    Symbol.DrawToBitmap(bmp, new Rectangle(0, 0, Symbol.Size.Width, Symbol.Size.Height));

                    Image thumbnail = CreateThumbnail(bmp, width, height);

                    SymbolImageList.Images.Add(thumbnail);

                    //表示名
                    GroupListViewItem item = new GroupListViewItem(Symbol.SymbolType, IndexNum - 1);
                    item.Tag = Symbol.GetType().Name.Replace("_Draw", "");
                    item.GroupName = Symbol.Category;

                    SymbolViewItemCollection.Add(item);

                    thumbnail.Dispose();
                    bmp.Dispose();

                    IndexNum++;
                }
               
            }

            SymbolListView.Items.AddRange(SymbolViewItemCollection.ToArray());

            //TabPage追加（カテゴリ数分）
            foreach (string CategoryName in SymbolViewItemCollection.GetGroupNames())
            {
                TabPage tp = new TabPage(CategoryName);
                tabControl1.TabPages.Add(tp);
            }
            this.SymbolListView.EndUpdate();
            runningForm.Close();

            //nonPanel.Dispose();


        }

        private Image CreateThumbnail(Image image, int w, int h)
        {
            Bitmap canvas = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(canvas);
            g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, w, h);

            float fw = (float)w / (float)image.Width;
            float fh = (float)h / (float)image.Height;

            float scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }

        private void ThumbnailThread()
        {
            runningForm = new Form();
            runningForm.Size = new Size(260, 100);
            runningLabel = new Label();
            runningForm.Controls.Add(runningLabel);
            runningLabel.Location = new Point(35, 45);
            runningLabel.Size = new Size(200, 20);
            runningForm.StartPosition = FormStartPosition.CenterScreen;
            runningForm.FormBorderStyle = FormBorderStyle.None;
            runningForm.ShowInTaskbar = false;
            runningForm.ShowDialog();
            runningForm.TopMost = true;
        }

        #endregion
               

        private void SymbolListForm_Load(object sender, EventArgs e)
        {  
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "全て表示")
            {
                SymbolListView.Items.Clear();
                SymbolListView.Items.AddRange(SymbolViewItemCollection.ToArray());
            }
            else
            {
                SymbolListView.Items.Clear();
                SymbolListView.Items.AddRange(SymbolViewItemCollection.GetGroupItems(e.TabPage.Text).ToArray());
            }

        }
        
        private void SymbolListView_Click(object sender, EventArgs e)
        {
            //ShapeToolStrip選択を解除
            MainForm.Instance.LineToolStripButton.Checked = false;
            MainForm.Instance.LinesToolStripButton.Checked = false;
            MainForm.Instance.CircleToolStripButton.Checked = false;
            MainForm.Instance.SquareToolStripButton.Checked = false;
            MainForm.Instance.PolygonToolStripButton.Checked = false;
            MainForm.Instance.TextToolStripButton.Checked = false;
            
        }

    }
}
