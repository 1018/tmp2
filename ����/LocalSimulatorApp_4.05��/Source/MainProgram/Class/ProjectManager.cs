using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonClassLibrary;
using System.Windows.Forms;
using System.Drawing;

namespace LocalSimulator.MainProgram
{
    public static class ProjectManager
    {
        public static bool Load(string filePath)
        {
            ProjectDataSerializeFormat projectData = ProjectDataSerializeFormat.Load(filePath);
            if (projectData == null)
            {
                return false;
            }

            SetProjectData(filePath, projectData);

            return true;
        }

        public static void Save()
        {
            SerializeData.IOMonitorData = MonitorSymbolList;  // モニタシンボル
            SerializeData.ConnectSetting = ConnectSetting;    // PLC設定

            ProjectDataSerializeFormat.SaveMainProgram(ProjectFilePath, SerializeData);
        }

        private static void SetProjectData(string filePath, ProjectDataSerializeFormat projectData)
        {
            // プロジェクトデータに変換する
            SerializeData = projectData;
            ProjectFilePath = filePath;
            ProjectName = projectData.ProjectName;
            ConnectSetting = projectData.ConnectSetting;
            MakerName = projectData.MakerName;
            BaseFormList = new List<IBaseForm>();
            MonitorSymbolList = projectData.IOMonitorData;
        }


        public static ProjectDataSerializeFormat SerializeData { get; set; }

        public static string ProjectFilePath { get; set; }

        public static string ProjectName { get; set; }

        public static List<IBaseForm> BaseFormList { get; set; }

        public static SystemSettingFormat SettingData { get; set; }

        public static string MakerName { get; set; }

        public static List<object> ConnectSetting { get; set; }

        public static List<IOMonitorSymbolFormat> MonitorSymbolList { get; set; }

        public static bool Loaded { get; set; }
    }
}
