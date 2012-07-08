using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonClassLibrary
{
    static public class LoadAssembly
    {
        static List<Assembly> _EnableAssemblyList = null;
        static string _CurrentDirectory = null;

        /// <summary>
        /// 使用可能なPLCのメーカー名を取得します。
        /// </summary>
        /// <param name="directoryPath">PLCのDLLファイルが存在するディレクトリ</param>
        /// <returns>メーカー名</returns>
        static public string[] GetMakerNames(string directoryPath)
        {
            CreateAssemblyList(directoryPath);

            List<string> makerNameList = new List<string>();

            foreach (Assembly asm in _EnableAssemblyList)
            {
                Type[] exportedTypes = asm.GetExportedTypes();

                foreach (Type type in exportedTypes)
                {
                    if (type.GetInterfaces().Contains(typeof(IPlcCommunication)))
                    {
                        IPlcCommunication instance = (IPlcCommunication)Activator.CreateInstance(type);

                        makerNameList.Add(instance.MakerName);
                    }
                }
            }

            return makerNameList.ToArray();
        }

        /// <summary>
        /// 指定のメーカーのIPlcCommunicationの実装を取得します。
        /// </summary>
        /// <param name="directoryPath">PLCのDLLファイルが存在するディレクトリ</param>
        /// <param name="makerName">実装を取得するPLCのメーカー名</param>
        /// <returns>IPlcCommunicationを実装した型のインスタンス</returns>
        static public IPlcCommunication LoadPlcCommunication(string directoryPath, string makerName)
        {
            CreateAssemblyList(directoryPath);

            return GetMakerInterfaceImpl<IPlcCommunication>(makerName);
        }

        /// <summary>
        /// 指定のメーカーのIDeviceManagerの実装を取得します。
        /// </summary>
        /// <param name="directoryPath">PLCのDLLファイルが存在するディレクトリ</param>
        /// <param name="makerName">実装を取得するPLCのメーカー名</param>
        /// <returns>IDeviceMangerを実装した型のインスタンス</returns>
        static public IDeviceManager LoadDeviceManager(string directoryPath, string makerName)
        {
            CreateAssemblyList(directoryPath);

            return GetMakerInterfaceImpl<IDeviceManager>(makerName);
        }

        /// <summary>
        /// 指定のメーカーのIPlcSettingの実装を取得します。
        /// </summary>
        /// <param name="directoryPath">PLCのDLLファイルが存在するディレクトリ</param>
        /// <param name="makerName">実装を取得するPLCのメーカー名</param>
        /// <returns>IPlcSettingを実装した型のインスタンス</returns>
        static public IPlcSetting LoadPlcSetting(string directoryPath, string makerName)
        {
            CreateAssemblyList(directoryPath);

            return GetMakerInterfaceImpl<IPlcSetting>(makerName);
        }


        static private void CreateAssemblyList(string directoryPath)
        {
            if (_CurrentDirectory == directoryPath)
            {
                return;
            }

            IEnumerable<string> dllFiles = System.IO.Directory.GetFiles(directoryPath, "*.dll");
            List<Assembly> assemblyList = new List<Assembly>();

            // IPlcCommunicationを実装している型が存在するDLLのみ
            // assemblyListに登録する。
            foreach (string path in dllFiles)
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(path);

                    Type[] exportedTypes = asm.GetExportedTypes();

                    foreach (Type type in exportedTypes)
                    {
                        if (type.GetInterfaces().Contains(typeof(IPlcCommunication)))
                        {
                            assemblyList.Add(asm);
                            break;
                        }
                    }
                }
                catch(Exception e)
                {
                    // デバッグ用
                    Global.LogManager.Write(e.ToString());
                }
            }

            _EnableAssemblyList = assemblyList;
            _CurrentDirectory = directoryPath;
        }

        static private Assembly GetMakerAssembly(string makerName)
        {
            foreach (Assembly asm in _EnableAssemblyList)
            {
                Type[] exportedTypes = asm.GetExportedTypes();

                foreach (Type type in exportedTypes)
                {
                    if (type.GetInterfaces().Contains(typeof(IPlcCommunication)))
                    {
                        IPlcCommunication plc = (IPlcCommunication)Activator.CreateInstance(type);

                        if (plc.MakerName == makerName)
                        {
                            return asm;
                        }
                    }
                }
            }

            return null;
        }

        static private T GetMakerInterfaceImpl<T>(string makerName)
        {
            Assembly makerAsm = GetMakerAssembly(makerName);

            Type[] exporetedTypes = makerAsm.GetExportedTypes();

            foreach (Type type in exporetedTypes)
            {
                if (type.GetInterfaces().Contains(typeof(T)))
                {
                    T instance = (T)Activator.CreateInstance(type);

                    return instance;
                }
            }

            return default(T);
        }
    }
}
