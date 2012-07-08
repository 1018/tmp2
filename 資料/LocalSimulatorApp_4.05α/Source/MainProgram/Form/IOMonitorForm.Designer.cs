namespace LocalSimulator.MainProgram
{
    partial class IOMonitorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer treeListViewItemCollectionComparer1 = new System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer();
            this.DummyPanel1 = new System.Windows.Forms.Panel();
            this.rbDouble = new System.Windows.Forms.RadioButton();
            this.rbFloat = new System.Windows.Forms.RadioButton();
            this.rbInt32 = new System.Windows.Forms.RadioButton();
            this.rbInt16 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.DummyPanel2 = new System.Windows.Forms.Panel();
            this.rbBinNumber = new System.Windows.Forms.RadioButton();
            this.rbHexNumber = new System.Windows.Forms.RadioButton();
            this.rbDecNumber = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.treeListIOMonitor = new System.Windows.Forms.TreeListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnValue = new System.Windows.Forms.ColumnHeader();
            this.columnFormNumber = new System.Windows.Forms.ColumnHeader();
            this.DummyPanel1.SuspendLayout();
            this.DummyPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // DummyPanel1
            // 
            this.DummyPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DummyPanel1.Controls.Add(this.rbDouble);
            this.DummyPanel1.Controls.Add(this.rbFloat);
            this.DummyPanel1.Controls.Add(this.rbInt32);
            this.DummyPanel1.Controls.Add(this.rbInt16);
            this.DummyPanel1.Controls.Add(this.label1);
            this.DummyPanel1.Location = new System.Drawing.Point(12, 12);
            this.DummyPanel1.Name = "DummyPanel1";
            this.DummyPanel1.Size = new System.Drawing.Size(189, 91);
            this.DummyPanel1.TabIndex = 1;
            // 
            // rbDouble
            // 
            this.rbDouble.AutoSize = true;
            this.rbDouble.Location = new System.Drawing.Point(44, 69);
            this.rbDouble.Name = "rbDouble";
            this.rbDouble.Size = new System.Drawing.Size(99, 19);
            this.rbDouble.TabIndex = 4;
            this.rbDouble.Text = "実数(倍精度)";
            this.rbDouble.UseVisualStyleBackColor = true;
            this.rbDouble.CheckedChanged += new System.EventHandler(this.ValuePattern_CheckedChanged);
            // 
            // rbFloat
            // 
            this.rbFloat.AutoSize = true;
            this.rbFloat.Location = new System.Drawing.Point(44, 47);
            this.rbFloat.Name = "rbFloat";
            this.rbFloat.Size = new System.Drawing.Size(99, 19);
            this.rbFloat.TabIndex = 3;
            this.rbFloat.Text = "実数(単精度)";
            this.rbFloat.UseVisualStyleBackColor = true;
            this.rbFloat.CheckedChanged += new System.EventHandler(this.ValuePattern_CheckedChanged);
            // 
            // rbInt32
            // 
            this.rbInt32.AutoSize = true;
            this.rbInt32.Location = new System.Drawing.Point(44, 25);
            this.rbInt32.Name = "rbInt32";
            this.rbInt32.Size = new System.Drawing.Size(84, 19);
            this.rbInt32.TabIndex = 2;
            this.rbInt32.Text = "32ﾋﾞｯﾄ整数";
            this.rbInt32.UseVisualStyleBackColor = true;
            this.rbInt32.CheckedChanged += new System.EventHandler(this.ValuePattern_CheckedChanged);
            // 
            // rbInt16
            // 
            this.rbInt16.AutoSize = true;
            this.rbInt16.Checked = true;
            this.rbInt16.Location = new System.Drawing.Point(44, 3);
            this.rbInt16.Name = "rbInt16";
            this.rbInt16.Size = new System.Drawing.Size(84, 19);
            this.rbInt16.TabIndex = 1;
            this.rbInt16.TabStop = true;
            this.rbInt16.Text = "16ﾋﾞｯﾄ整数";
            this.rbInt16.UseVisualStyleBackColor = true;
            this.rbInt16.CheckedChanged += new System.EventHandler(this.ValuePattern_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "表示：";
            // 
            // DummyPanel2
            // 
            this.DummyPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DummyPanel2.Controls.Add(this.rbBinNumber);
            this.DummyPanel2.Controls.Add(this.rbHexNumber);
            this.DummyPanel2.Controls.Add(this.rbDecNumber);
            this.DummyPanel2.Controls.Add(this.label2);
            this.DummyPanel2.Location = new System.Drawing.Point(207, 12);
            this.DummyPanel2.Name = "DummyPanel2";
            this.DummyPanel2.Size = new System.Drawing.Size(123, 91);
            this.DummyPanel2.TabIndex = 2;
            // 
            // rbBinNumber
            // 
            this.rbBinNumber.AutoSize = true;
            this.rbBinNumber.Location = new System.Drawing.Point(46, 47);
            this.rbBinNumber.Name = "rbBinNumber";
            this.rbBinNumber.Size = new System.Drawing.Size(44, 19);
            this.rbBinNumber.TabIndex = 3;
            this.rbBinNumber.Text = "2進";
            this.rbBinNumber.UseVisualStyleBackColor = true;
            this.rbBinNumber.CheckedChanged += new System.EventHandler(this.NumberPattern_CheckedChanged);
            // 
            // rbHexNumber
            // 
            this.rbHexNumber.AutoSize = true;
            this.rbHexNumber.Location = new System.Drawing.Point(46, 25);
            this.rbHexNumber.Name = "rbHexNumber";
            this.rbHexNumber.Size = new System.Drawing.Size(51, 19);
            this.rbHexNumber.TabIndex = 2;
            this.rbHexNumber.Text = "16進";
            this.rbHexNumber.UseVisualStyleBackColor = true;
            this.rbHexNumber.CheckedChanged += new System.EventHandler(this.NumberPattern_CheckedChanged);
            // 
            // rbDecNumber
            // 
            this.rbDecNumber.AutoSize = true;
            this.rbDecNumber.Checked = true;
            this.rbDecNumber.Location = new System.Drawing.Point(46, 3);
            this.rbDecNumber.Name = "rbDecNumber";
            this.rbDecNumber.Size = new System.Drawing.Size(51, 19);
            this.rbDecNumber.TabIndex = 1;
            this.rbDecNumber.TabStop = true;
            this.rbDecNumber.Text = "10進";
            this.rbDecNumber.UseVisualStyleBackColor = true;
            this.rbDecNumber.CheckedChanged += new System.EventHandler(this.NumberPattern_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "数値：";
            // 
            // treeListIOMonitor
            // 
            this.treeListIOMonitor.AutoArrange = false;
            this.treeListIOMonitor.CheckBoxes = System.Windows.Forms.CheckBoxesTypes.Simple;
            this.treeListIOMonitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnAddress,
            this.columnValue,
            this.columnFormNumber});
            treeListViewItemCollectionComparer1.Column = 0;
            treeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeListIOMonitor.Comparer = treeListViewItemCollectionComparer1;
            this.treeListIOMonitor.FullRowSelect = false;
            this.treeListIOMonitor.GridLines = true;
            this.treeListIOMonitor.LabelWrap = false;
            this.treeListIOMonitor.Location = new System.Drawing.Point(0, 109);
            this.treeListIOMonitor.MultiSelect = false;
            this.treeListIOMonitor.Name = "treeListIOMonitor";
            this.treeListIOMonitor.Size = new System.Drawing.Size(342, 157);
            this.treeListIOMonitor.Sorting = System.Windows.Forms.SortOrder.None;
            this.treeListIOMonitor.TabIndex = 0;
            this.treeListIOMonitor.UseCompatibleStateImageBehavior = false;
            this.treeListIOMonitor.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.TreeListIOMonitor_ItemChecked);
            this.treeListIOMonitor.DoubleClick += new System.EventHandler(this.TreeListIOMonitor_DoubleClick);
            this.treeListIOMonitor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeListIOMonitor_KeyDown);
            this.treeListIOMonitor.AfterExpand += new System.Windows.Forms.TreeListViewEventHandler(this.TreeListIOMonitor_AfterExpand);
            // 
            // columnName
            // 
            this.columnName.Text = "名称";
            this.columnName.Width = 140;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "アドレス";
            // 
            // columnValue
            // 
            this.columnValue.Text = "値";
            // 
            // columnFormNumber
            // 
            this.columnFormNumber.Text = "Form№";
            this.columnFormNumber.Width = 70;
            // 
            // IOMonitorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 266);
            this.Controls.Add(this.DummyPanel2);
            this.Controls.Add(this.DummyPanel1);
            this.Controls.Add(this.treeListIOMonitor);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 9F);
            this.HideOnClose = true;
            this.Name = "IOMonitorForm";
            this.Text = "I/Oモニタ";
            this.Load += new System.EventHandler(this.IOMonitorForm_Load);
            this.SizeChanged += new System.EventHandler(this.IOMonitorForm_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.IOMonitorForm_VisibleChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.IOMonitorForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.IOMonitorForm_DragEnter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOMonitorForm_FormClosing);
            this.DummyPanel1.ResumeLayout(false);
            this.DummyPanel1.PerformLayout();
            this.DummyPanel2.ResumeLayout(false);
            this.DummyPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeListView treeListIOMonitor;
        private System.Windows.Forms.Panel DummyPanel1;
        private System.Windows.Forms.RadioButton rbInt16;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbDouble;
        private System.Windows.Forms.RadioButton rbFloat;
        private System.Windows.Forms.RadioButton rbInt32;
        private System.Windows.Forms.Panel DummyPanel2;
        private System.Windows.Forms.RadioButton rbDecNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbHexNumber;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.ColumnHeader columnFormNumber;
        private System.Windows.Forms.RadioButton rbBinNumber;
    }
}