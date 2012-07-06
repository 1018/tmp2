using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MITSUBISHI_PLC
{
    public class EnableComboBox : ComboBox
    {
        public EnableComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }


        int[] invalidityIndex = new int[] { };


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.Enabled == true)
            {

                Color itemBackColor = e.BackColor;
                Color itemForeColor = e.ForeColor;
                DrawItemState itemState = e.State;

                if (e.Index >= 0)
                {
                    if (this.InvalidityIndex.Contains(e.Index))
                    {
                        itemBackColor = e.BackColor;
                        itemForeColor = SystemColors.ControlDark;
                    }


                    Graphics g = e.Graphics;

                    SolidBrush invalidBackColorBrush = new SolidBrush(itemBackColor);
                    g.FillRectangle(invalidBackColorBrush, e.Bounds);

                    SolidBrush invalidForeColorBrush = new SolidBrush(itemForeColor);
                    g.DrawString(this.Items[e.Index].ToString(), e.Font, invalidForeColorBrush, e.Bounds.Location);


                    if ((e.State & DrawItemState.Focus) != 0)
                    {
                        e.DrawFocusRectangle();
                    }
                }
            }

            base.OnDrawItem(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.InvalidityIndex.Contains(this.SelectedIndex))
            {
                for (int itemIndex = 0; itemIndex < this.Items.Count; itemIndex++)
                {
                    if (!this.InvalidityIndex.Contains(itemIndex))
                    {
                        this.SelectedIndex = itemIndex;
                        break;
                    }
                }
            }

            base.OnSelectedIndexChanged(e);
        }

        //protected override void WndProc(ref Message m)
        //{
        //    // 選択項目が無効項目
        //    if (this.InvalidityIndex.Contains(this.SelectedIndex))
        //    {
        //        // 選択イベント(?)を無視する
        //        if (m.Msg == 0x0111)
        //        {
        //            return;
        //        }
        //    }

        //    base.WndProc(ref m);
        //}


        public object[] InvalidityItems
        {
            get
            {
                List<object> itemList = new List<object>();

                foreach (int index in InvalidityIndex)
                {
                    if (index < this.Items.Count)
                    {
                        itemList.Add(this.Items[index]);
                    }
                }

                return itemList.ToArray();
            }
            set
            {
                object[] setValues = value ?? new object[] { };

                List<int> itemIndexList = new List<int>();

                foreach (object item in setValues)
                {
                    if (this.Items.Contains(item))
                    {
                        itemIndexList.Add(this.Items.IndexOf(item));
                    }
                }

                this.InvalidityIndex = itemIndexList.ToArray();
            }
        }

        public int[] InvalidityIndex
        {
            get
            {
                return this.invalidityIndex;
            }
            set
            {
                int[] setValues = value ?? new int[] { };

                this.invalidityIndex = setValues;
            }
        }
    }
}
