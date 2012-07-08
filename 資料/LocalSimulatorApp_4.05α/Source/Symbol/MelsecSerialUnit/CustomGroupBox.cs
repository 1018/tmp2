using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolLibrary
{
    public partial class CustomGroupBox : GroupBox
    {
        public CustomGroupBox()
        {
            InitializeComponent();
        }

        public Color BorderColor { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            Size textSize = TextRenderer.MeasureText(this.Text, this.Font);
            Rectangle borderRect = this.ClientRectangle;
            borderRect.Y = (borderRect.Y + (textSize.Height / 2));
            borderRect.Height = (borderRect.Height - (textSize.Height / 2));
            ControlPaint.DrawBorder(e.Graphics, borderRect, this.BorderColor,ButtonBorderStyle.Solid);

            Rectangle textRect = this.ClientRectangle;
            textRect.X = (textRect.X + 6);
            textRect.Width = textSize.Width;
            textRect.Height = textSize.Height;
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), textRect);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(Color.Blue),textRect);

        }
    }
}
