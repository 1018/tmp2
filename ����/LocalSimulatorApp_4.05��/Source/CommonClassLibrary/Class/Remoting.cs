using System;
using System.Collections.Generic;

namespace CommonClassLibrary
{
    public class Remoting// : MarshalByRefObject
    {

        private List<RemotingDataFormat> _RecvDataList = new List<RemotingDataFormat>();

        private List<RemotingMessageFormat> _SendDataList = new List<RemotingMessageFormat>();

        private RecvDeviceFormat _RecvDeviceList = new RecvDeviceFormat();

        private bool _PlcCommunicationEndRequest = false;


        private readonly object RecvLock = new object();

        private readonly object SendLock = new object();

        private readonly object RecvDeviceListLock = new object();

        private readonly object PlcPlcCommunicationEndRequestLock = new object();


        private static volatile Remoting _Instance;

        private static object syncRoot = new Object();

        public static Remoting Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new Remoting();
                    }
                }

                return _Instance;
            }
        }

        public List<RemotingDataFormat> RecvDataList
        {
            get
            {
                return _RecvDataList;
            }
            set
            {
                lock (RecvLock)
                {
                    _RecvDataList = value;
                }
            }
        }

        private List<RemotingMessageFormat> SendDataList
        {
            get { return _SendDataList; }
            set { _SendDataList = value; }
        }

        public RecvDeviceFormat RecvDeviceList
        {
            get
            {
                lock (RecvDeviceListLock)
                {
                    return _RecvDeviceList;
                }
            }
            set
            {
                lock (RecvDeviceListLock)
                {
                    _RecvDeviceList = value;
                }
            }
        }

        public bool PlcCommunicationEndRequest
        {
            get
            {
                lock (PlcPlcCommunicationEndRequestLock)
                {
                    return _PlcCommunicationEndRequest;
                }
            }
            set
            {
                lock (PlcPlcCommunicationEndRequestLock)
                {
                    _PlcCommunicationEndRequest = value;
                }
            }
        }

        ///// <summary>
        ///// 共有オブジェクトの有効期間を無期限に設定するための処理
        ///// </summary>
        ///// <returns></returns>
        //public override object InitializeLifetimeService()
        //{
        //    ILease lease = (ILease)base.InitializeLifetimeService();

        //    if (lease.CurrentState == LeaseState.Initial)
        //    {
        //        lease.InitialLeaseTime = TimeSpan.Zero;
        //    }

        //    return lease;
        //}

        public void AddSendData(RemotingMessageFormat addData)
        {
            lock (SendLock)
            {
                this.SendDataList.Add(addData);
            }
        }

        public RemotingMessageFormat[] GetSendDataList()
        {
            lock (SendLock)
            {
                RemotingMessageFormat[] RetArray = this.SendDataList.ToArray();
                this.SendDataList.Clear();
                return RetArray;
            }
        }

        public List<RemotingDataFormat> GetRecvDataList()
        {
            List<RemotingDataFormat> ReturnList = new List<RemotingDataFormat>();

            ReturnList.AddRange(RecvDataList);
            ReturnList.AddRange(RecvKMList);

            return ReturnList;
        }


        #region KM 送受信
        readonly object SendKMLock = new object();
        readonly object ReadKMLock = new object();

        List<RemotingMessageFormat> _sendKMList = new List<RemotingMessageFormat>();
        private List<RemotingMessageFormat> SendKMList
        {
            get { return _sendKMList; }
            set { _sendKMList = value; }
        }

        List<RemotingDataFormat> _recvKMList = new List<RemotingDataFormat>();
        public List<RemotingDataFormat> RecvKMList
        {
            get { return _recvKMList; }
            set
            {
                lock (ReadKMLock)
                {
                    _recvKMList = value;
                }
            }
        }

        public void AddSendKMData(RemotingMessageFormat addData)
        {
            lock (SendKMLock)
            {
                this.SendKMList.Add(addData);
            }

        }

        public RemotingMessageFormat[] GetSendKMDataList()
        {
            lock (SendKMLock)
            {
                RemotingMessageFormat[] RetArray = this.SendKMList.ToArray();
                this.SendKMList.Clear();
                return RetArray;
            }
        }
        #endregion


        public void SetSendData(Symbol_Draw callSymbol, string address, int dataCount, ICalculator calculator)
        {
            RemotingMessageFormat MyData = new RemotingMessageFormat();

            Global.DeviceConvert(ref address);

            if (!string.IsNullOrEmpty(address))
            {
                MyData.DeviceString = address;
                MyData.Calculator = calculator;
                MyData.DataCount = dataCount;

                // 通常デバイス
                if (address.IndexOf("KM") != 0)
                {
                    AddSendData(MyData);
                }
                // 仮想デバイス
                else
                {
                    AddSendKMData(MyData);
                }


                //// ログ出力デバイス
                //LoggingDeviceCollection LoggingDevice = DeviceLogManager.Collection;

                //DeviceElement SetAddress = DeviceManager.ToElement(address);
                //DeviceFormat SetDevice = new DeviceFormat(dataCount);

                //// セット以外無理。。。
                //if(calculator is CalculatorSet == false) { return;}

                //ushort[] SetScheduleValue = new ushort[dataCount];
                //SetScheduleValue = calculator.Calculate(SetScheduleValue);

                //for (int i = 0; i < dataCount; i++)
                //{
                //    SetDevice.Address = SetAddress.Offset(i).ToString();

                //    if (LoggingDevice.Contains(callSymbol, SetDevice))
                //    {
                //        DeviceLogManager.LogOut(callSymbol.SymbolName, string.Empty, SetDevice.Address, string.Empty, SetScheduleValue[i].ToString());
                //    }
                //}
            }
        }

        //接続時の引数
        public object[] ConnectSetting { get; set; }
        public ConnectionMode Mode { get; set; }


        private readonly object ConnectedLock = new object();
        private bool _PlcCommunictionIsConnected;
        public bool PlcCommunicationIsConnected
        {
            get
            {
                return _PlcCommunictionIsConnected;
            }
            set
            {
                lock (ConnectedLock)
                {
                    _PlcCommunictionIsConnected = value;
                }
            }
        }

        public long CommunicationCyclicTime { get; set; }

        public event PlcErrorEventHandler CommunicationError;

        public void OnCommunicationError(PlcErrorEventArgs e)
        {
            if (this.CommunicationError != null)
            {
                this.CommunicationError(this, e);
            }
        }
    }

    public static class RemotingClient
    {
        public static Remoting Connection()
        {
            //// IPC Channelの作成
            //IpcClientChannel clientChannel = new IpcClientChannel();

            //try
            //{
            //    // リモート処理チャネルの登録
            //    ChannelServices.RegisterChannel(clientChannel, true);

            //    // オブジェクトの取得
            //    string uri = "ipc://LocalSimulator/CommonClass";

            //    return (Remoting)Activator.GetObject(typeof(Remoting), uri);
            //}
            //catch (System.Runtime.Remoting.RemotingException)
            //{
            //    return null;
            //}

            return Remoting.Instance;



        }

        public static void Disconnection()
        {
            //IChannel[] channels = ChannelServices.RegisteredChannels;
            //foreach (IChannel channel in channels)
            //{
            //    if (channel.GetType() == typeof(IpcClientChannel))
            //    {
            //        ChannelServices.UnregisterChannel(channel);
            //    }
            //}
        }
    }

    public static class RemotingServer
    {
        private static Remoting Common;

        public static void Start()
        {
            ////サーバー設定
            //// IPC Channelを作成
            //IpcServerChannel serverChannel = new IpcServerChannel("LocalSimulator");

            //// リモートオブジェクトを登録
            //ChannelServices.RegisterChannel(serverChannel, true);

            //// Typeを登録
            //RemotingConfiguration.RegisterWellKnownServiceType(typeof(Remoting), "CommonClass", WellKnownObjectMode.Singleton);
            //Common = Remoting.Instance;
            //RemotingServices.Marshal(Common, "CommonClass", typeof(Remoting));

            //Global.LogManager.Write("リモーティング接続（IPCサーバー）完了");

            Common = Remoting.Instance;
        }

    }






}
