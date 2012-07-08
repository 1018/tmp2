namespace LocalSimulator.MainProgram
{
    partial class BaseFormListForm
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
            this.BaseFormList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // BaseFormList
            // 
            this.BaseFormList.BackColor = System.Drawing.SystemColors.Control;
            this.BaseFormList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.BaseFormList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseFormList.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BaseFormList.FullRowSelect = true;
            this.BaseFormList.GridLines = true;
            this.BaseFormList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.BaseFormList.Location = new System.Drawing.Point(0, 0);
            this.BaseFormList.Name = "BaseFormList";
            this.BaseFormList.Size = new System.Drawing.Size(292, 266);
            this.BaseFormList.TabIndex = 10;
            this.BaseFormList.UseCompatibleStateImageBehavior = false;
            this.BaseFormList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 134;
            // 
            // BaseFormListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.BaseFormList);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "BaseFormListForm";
            this.Text = "画面選択";
            this.SizeChanged += new System.EventHandler(this.BaseFormList_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView BaseFormList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}