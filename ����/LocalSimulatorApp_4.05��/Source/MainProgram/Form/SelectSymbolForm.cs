using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonClassLibrary;

namespace LocalSimulator.MainProgram
{
    public partial class SelectSymbolForm : Form
    {
        public static readonly Symbol_Draw[] EmptyCollection = new Symbol_Draw[] { };


        public SelectSymbolForm()
        {
            InitializeComponent();

            SelectedSymbolCollection = EmptyCollection;
        }

        List<Symbol_Draw> sourceList = new List<Symbol_Draw>();
        public List<Symbol_Draw> SourceList
        {
            get { return sourceList; }
            set
            {
                sourceList = value;

                // 全アイテム削除
                listBox1.Items.Clear();

                // 選択リスト作成
                List<string> BindingSource = new List<string>(
                    from element in SourceList
                    where !string.IsNullOrEmpty(element.SymbolName)     // ダミーシンボル対策
                    select element.SymbolName
                );
                BindingList<string> BindingList = new BindingList<string>(BindingSource);

                // ListBoxにバインド
                listBox1.DataSource = BindingList;
            }
        }

        ICollection<Symbol_Draw> selectedSymbolCollection = null;
        public ICollection<Symbol_Draw> SelectedSymbolCollection
        {
            get { return selectedSymbolCollection; }
            set
            {
                if (value != null)
                {
                    foreach (Symbol_Draw SetSymbol in value)
                    {
                        if (sourceList.Find((symbol) => SetSymbol.Equals(symbol)) == null)                        
                        {
                            throw (new InvalidOperationException(string.Format("\"{0}\"は無効なシンボル名です。", SetSymbol.SymbolName)));
                        }
                    }

                    selectedSymbolCollection = value;
                }
                else
                {
                    selectedSymbolCollection = EmptyCollection;
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            // 選択シンボル登録
            SetSelectedSymbols();

            // シンボルが登録された
            if (SelectedSymbolCollection.Equals(EmptyCollection) == false)
            {
                this.Close();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SetSelectedSymbols();

                if (SelectedSymbolCollection.Equals(EmptyCollection) == false)
                {
                    this.Close();
                }
            }

            base.OnKeyDown(e);
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listBox1.SelectedItem != null)
                {
                    //this.contextMenuStrip1.Show(listBox1, this.Cursor.HotSpot);
                    //this.contextMenuStrip1.Show(this.Cursor.HotSpot);
                }
            }
        }

        private void toolStripMenuRegist_Click(object sender, EventArgs e)
        {
            // 選択シンボル登録
            SetSelectedSymbols();

            // シンボルが登録された
            if (SelectedSymbolCollection.Equals(EmptyCollection) == false)
            {
                this.Close();
            }
        }

        private void SetSelectedSymbols()
        {
            if (listBox1.SelectedItems != null)
            {
                IEnumerable<string> SelectedStrings =
                    from object SelectedItem in listBox1.SelectedItems
                    where SelectedItem is string
                    select (string)SelectedItem;

                List<Symbol_Draw> SelectedSymbolList = new List<Symbol_Draw>();

                foreach (string SymbolName in SelectedStrings)
                {
                    SelectedSymbolList.Add(SourceList.Find(symbol => symbol.SymbolName == SymbolName));
                }

                SelectedSymbolCollection = SelectedSymbolList;
            }
        }
    }
}
