using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace KssClassLibrary
{
    public class LogManager
    {
        public static Dictionary<string, LogManager> LogManagerList = new Dictionary<string, LogManager>();

        //各クラス用のLogManagerを生成・登録する
        public static LogManager CreateInstance(object sender, string filePath, bool onlyOne)
        {
            string key = sender.ToString();

            if (!LogManagerList.ContainsKey(key))
            {
                LogManagerList[key] = new LogManager(filePath, onlyOne);
            }

            return LogManagerList[key];
        }

        private int writeCount = 0;

        private int makeNumber = 0;

        private string baseFilePath = null;

        private TextWriter logWriter;

        public bool StopRequest { get; set; }

        public bool Create { get; private set; }

        public bool OnlyOne { get; set; }

        private LogManager(string filePath, bool onlyOne)
        {
            this.OnlyOne = onlyOne;
            Start(filePath);
        }

        public void Start(string filePath)
        {
            baseFilePath = filePath;

            if (OnlyOne == false)
            {
                //既に存在するファイルがあるか確認し、ファイル名を決定する。
                for (int i = 0; i <= 99999; i++)
                {
                    filePath = GetFilePath(baseFilePath, i);

                    if (!File.Exists(filePath))
                    {
                        makeNumber = i;
                        break;
                    }
                }
            }

            //出力ファイルを指定して、StreamWriterオブジェクトを作成
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter sw = new StreamWriter(filePath, false, sjisEnc);

                sw.AutoFlush = true;

                //スレッドセーフラッパを作成
                logWriter = TextWriter.Synchronized(sw);

                Create = true;
            }
            catch
            { }
        }

        public void Write(string text)
        {
            if (StopRequest || logWriter == null) { return; }
            Debug.Assert(this.Create);

            writeCount += 1;

            if (OnlyOne == false)
            {

                #region 書き込み行数によりファイルを再生成する。
                if (writeCount >= 10000)
                {
                    logWriter.Dispose();

                    makeNumber += 1;

                    string filePath = GetFilePath(baseFilePath, makeNumber);

                    //出力ファイルを指定して、StreamWriterオブジェクトを作成                    
                    Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                    StreamWriter sw = new StreamWriter(filePath, false, sjisEnc);

                    sw.AutoFlush = true;

                    //スレッドセーフラッパを作成
                    logWriter = TextWriter.Synchronized(sw);

                    //書き込み回数リセット
                    writeCount = 0;
                }

                #endregion
            }

            string dateTime = DateTime.Now.ToString("HH:mm:ss") + "." + DateTime.Now.Millisecond.ToString().PadLeft(3, '0');

            logWriter.WriteLine(dateTime + " " + text);
        }

        private string GetFilePath(string filePath, int makeNumber)
        {
            //拡張子を外す
            string pathName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extensionName = Path.GetExtension(filePath);

            return pathName + "\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + makeNumber.ToString().PadLeft(5, '0') + extensionName;
        }

        public void Dispose()
        {
            string myKey = LogManagerList.First((elem) => elem.Value == this).Key;
            LogManagerList.Remove(myKey);

            Console.WriteLine("Dispose");
            Create = false;
            if (logWriter == null) { return; }
            logWriter.Dispose();
        }
    }
}
