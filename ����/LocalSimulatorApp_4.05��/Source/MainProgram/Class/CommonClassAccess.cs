using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace CommonClassLibrary
{
    public static class CommonClassAccess
    {
        private static CommonClass Common;       

        public static void Start()
        {            
            
            //サーバー設定
            // IPC Channelを作成
            IpcServerChannel ServerChannel = new IpcServerChannel("PLC_LocalSimulator");

            // リモートオブジェクトを登録
            ChannelServices.RegisterChannel(ServerChannel, true);

            // Typeを登録
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(CommonClass), "CommonClass", WellKnownObjectMode.Singleton);

            Common = CommonClass.Instance;

            RemotingServices.Marshal(Common, "CommonClass", typeof(CommonClass));
        }       

    }

}
