using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace CommonClassLibrary
{
    public static class DeviceEvent
    {
        private static Remoting Common;
        private static Thread PostThread;
        private static bool RunRequest;
        private static List<EventSymbolFormat> EventSymbolList;
        private static List<RemotingDataFormat> RecvDataList;
        private static List<RemotingDataFormat> EventDataList;

        private static List<RemotingDataFormat> BeforeRecvDataList = null;

        //静的コンストラクタ
        static DeviceEvent()
        {
            PostThread = new Thread(PostingThread);
            PostThread.IsBackground = true;
            PostThread.Start();

            RunRequest = false;

            Common = Remoting.Instance;


        }

        public static bool IsRunning { get; private set; }

        public static void Start()
        {
            RecvDataList = new List<RemotingDataFormat>();
            EventDataList = new List<RemotingDataFormat>();
            BeforeRecvDataList = null;
            RunRequest = true;
        }

        public static void End()
        {
            RunRequest = false;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //停止するまで待機する。(1秒タイムアウト）
            while (IsRunning && sw.ElapsedMilliseconds < 1000) { }

            Global.LogManager.Write("デバイスイベント処理停止");
        }

        private static void PostingThread()
        {

            while (true)
            {
                if (RunRequest == true)
                {
                    IsRunning = true;
                    //EventSymbolList変更の反映
                    EventSymbolList = Global.EventSymbolList;

                    //初期設定
                    RecvDataList = Common.GetRecvDataList();
                    EventDataList.Clear();

                    #region Event発生処理

                    //初回もしくはRecvList更新時処理
                    if (BeforeRecvDataList == null || ListCompare(BeforeRecvDataList, RecvDataList) == false)
                    {
                        EventDataList = RecvDataList;
                    }
                    else
                    {
                        for (int i = 0; i < RecvDataList.Count; i++)
                        {
                            if (Enumerable.SequenceEqual(RecvDataList[i].Value, BeforeRecvDataList[i].Value) == false)
                            {
                                EventDataList.Add(RecvDataList[i]);
                            }
                        }
                    }

                    #endregion

                    #region 各シンボルにデータを書き込む
                    for (int i = 0; i < EventDataList.Count; i++)
                    {
                        for (int j = 0; j < EventSymbolList.Count; j++)
                        {
                            if (EventDataList[i].Device != EventSymbolList[j].Device ||
                                EventDataList[i].DataCount != EventSymbolList[j].DataCount) { continue; }

                            Symbol_Draw Symbol = EventSymbolList[j].Symbol;
                            PropertyInfo Property = EventSymbolList[j].Property;

                            #region コントロールに値を書き込む
                            if (typeof(DeviceFormat[]).IsAssignableFrom(Property.PropertyType))
                            {
                                Symbol.Invoke(new MethodInvoker(delegate
                                {
                                    //現在データを読み込む。
                                    DeviceFormat[] MyArray = (DeviceFormat[])Property.GetValue(Symbol, null);

                                    //Indexに相当するDeviceFormatDataを取り出し、値を書き換える
                                    UInt16[] Values = new UInt16[EventDataList[i].Value.Length];

                                    for (int k = 0; k < EventDataList[i].Value.Length; k++)
                                        Values[k] = (UInt16)(EventDataList[i].Value[k] & 0x0FFFF);

                                    MyArray[EventSymbolList[j].ArrayNumber].Value = Values;

                                    //値をプロパティに書き込む
                                    Property.SetValue(Symbol, MyArray, null);
                                }
                                ));
                            }
                            else
                            {
                                Symbol.Invoke(new MethodInvoker(delegate
                                {
                                    ////現在データを読み込む。
                                    //DeviceFormat MyData = (DeviceFormat)Property.GetValue(Symbol, null);

                                    ////値を書き換える
                                    //UInt16[] Values = new UInt16[EventDataList[i].Value.Length];

                                    //for (int k = 0; k < EventDataList[i].Value.Length; k++)
                                    //    Values[k] = (UInt16)(EventDataList[i].Value[k] & 0x0FFFF);

                                    //MyData.Value = Values;

                                    ////値をプロパティに書き込む
                                    //Property.SetValue(Symbol, MyData, null);

                                    DeviceFormat MyData;

                                    // 現在データを読み込む
                                    try
                                    {
                                        MyData = (DeviceFormat)Property.GetValue(Symbol, null);
                                    }
                                    catch (Exception e)
                                    {
                                        Global.LogManager.Write(
                                            string.Format("プロパティ読み込みエラー：シンボル={0}, プロパティ={1}, 内容={2}",
                                                Symbol.SymbolName, Property.Name, e.InnerException.Message));

                                        return;
                                    }

                                    // 値を書き換える(ToArrayの呼び出しで配列のコピーを生成)
                                    MyData.Value = EventDataList[i].Value.ToArray();

                                    // 値をプロパティに書き込む
                                    try
                                    {
                                        Property.SetValue(Symbol, MyData, null);
                                    }
                                    catch (Exception e)
                                    {
                                        Global.LogManager.Write(
                                            string.Format("プロパティ書き込みエラー：シンボル={0}, プロパティ={1}, 内容={2}",
                                                Symbol.SymbolName, Property.Name, e.InnerException.Message));

                                        return;
                                    }
                                }
                                ));
                            }
                            #endregion


                        }
                    }
                    #endregion

                    BeforeRecvDataList = RecvDataList.ToList();
                }
                else
                {
                    IsRunning = false;
                }
                Thread.Sleep(50);
            }


        }

        private static bool ListCompare(List<RemotingDataFormat> obj1, List<RemotingDataFormat> obj2)
        {
            //個数が違う場合
            if (obj1.Count != obj2.Count) { return false; }

            for (int i = 0; i < obj1.Count; i++)
            {
                if (obj1[i].Device != obj2[i].Device || obj1[i].DataCount != obj2[i].DataCount)
                {
                    return false;
                }
            }

            return true;

        }

    }
}
