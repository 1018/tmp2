using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace LocalSimulator.ProgramSelecter
{
    public partial class SelectForm : Form
    {        

        public SelectForm()
        {
            InitializeComponent();

            Thread exeThread = new Thread(new ThreadStart(ExeFindThread));
            exeThread.IsBackground = true;
            exeThread.Start();            

        }

        private void ExeFindThread()
        {
            try
            {
                while (true)
                {                    
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate()
                        {
                            ExeFind();
                        });
                    }
                    else
                    {
                        ExeFind();
                    }
                    
                    Thread.Sleep(100);
                }
            }
            catch { }
        }

        private void ExeFind()
        {
            bool isFindMainProgram = false;
            bool isFindProjectMaker = false;

            // 実行中のすべてのプロセスを取得する
            System.Diagnostics.Process[] allProcess = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process oneProcess in allProcess)
            {

                if (oneProcess.ProcessName == "MainProgram")
                {
                    isFindMainProgram = true;                    
                }

                if (oneProcess.ProcessName == "ProjectMaker")
                {
                    isFindProjectMaker = true;
                }

            }

            if (this.btnMainProgram.Enabled == true && isFindMainProgram == true)
            {
                this.btnMainProgram.Enabled = false;
            }

            if (this.btnMainProgram.Enabled == false && isFindMainProgram == false)
            {
                this.btnMainProgram.Enabled = true;
            }

            if (this.btnProjectMaker.Enabled == true && isFindProjectMaker == true)
            {
                this.btnProjectMaker.Enabled = false;
            }

            if (this.btnProjectMaker.Enabled == false && isFindProjectMaker == false)
            {
                this.btnProjectMaker.Enabled = true;
            }

        }

        private void btnMainProgram_Click(object sender, EventArgs e)
        { 
           
            string folderPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            Process.Start(folderPath + "\\" + "MainProgram.exe", "\"" + Global.CommandLineDatas[1] + "\"");

            this.Close();
        }

        private void btnProjectMaker_Click(object sender, EventArgs e)
        {
            string folderPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            Process.Start(folderPath + "\\" + "Projectmaker.exe", "\"" + Global.CommandLineDatas[1] + "\"");

            this.Close();
        }





    }
}
