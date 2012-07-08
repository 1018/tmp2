using System;
using System.Collections.Generic;
using System.Threading;

namespace CommonClassLibrary
{
    public static class SymbolCyclicThread
    {
        //1スレッドあたりのシンボル処理数
        private static readonly int SymbolCount = 50;

        private static List<ThreadClass> ThreadClassList = new List<ThreadClass>();        

        //シンボル登録
        public static void Registration(Symbol_Draw obj)
        {
            if (!obj.Cyclic) { return; }

            if (ThreadClassList.Count != 0)
            {
                foreach (ThreadClass MyClass in ThreadClassList)
                {
                    if (MyClass.DrawInstance.Count < SymbolCount)
                    {
                        MyClass.DrawInstance.Add(obj);
                        return;
                    }
                }
            }

            ThreadClass AddClass = new ThreadClass();
            AddClass.DrawInstance.Add(obj);
            ThreadClassList.Add(AddClass);

        }

        public static void Clear()
        {
            ThreadClassList = new List<ThreadClass>();
        }

        //スレッド開始
        public static void Start()
        {
            foreach (ThreadClass MyClass in ThreadClassList)
            {
                MyClass.Start();
            }
        }

        //スレッド終了
        public static void End()
        {
            foreach (ThreadClass MyClass in ThreadClassList)
            {
                MyClass.End();
            }
            Global.LogManager.Write("シンボル定周期処理停止");
        }

        private class ThreadClass
        {
            public List<Symbol_Draw> DrawInstance = new List<Symbol_Draw>();
            private Thread CyclicThread;
            private bool EndFlag = false;

            public void Start()
            {
                EndFlag = false;
                CyclicThread = new Thread(new ThreadStart(ThreadCyclic));
                CyclicThread.IsBackground = true;
                CyclicThread.Start();
            }

            private void ThreadCyclic()
            {
                while (true)
                {
                    try
                    {
                        if (EndFlag == true) { break; }
                        foreach (Symbol_Draw obj in DrawInstance)
                        {
                            //if (DrawInstance.Count == 50)
                            //{
                            //    Console.WriteLine(obj.SymbolName);
                            //}
                            obj.BaseCyclicMethod();
                            
                        }
                    }
                    //終了時、同期していない為に例外がでる場合がある
                    catch
                    {
                    }
                    //Console.WriteLine(DrawInstance.Count.ToString());
                    //Console.WriteLine("ThreadClassList " + ThreadClassList.Count.ToString());
                    System.Threading.Thread.Sleep(50);

                }
                Console.WriteLine("CyclicMethod Stop");

            }

            public void End()
            {
                EndFlag = true;

                if (CyclicThread != null)
                {
                    CyclicThread.Abort();
                }
            }
        }
    }
}
