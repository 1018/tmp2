using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using KssClassLibrary;

namespace CommonClassLibrary
{
    public static class Global
    {
        public static bool DesignMode = false;

        public static bool LockDataListMake = false;

        public static void DeviceConvert(ref string Str)
        {
            DeviceElement addr = DeviceManager.ToElement(Str);
            if (addr != null)
            {
                Str = addr.ToString();
            }
            else
            {
                // 不正なアドレス
                Str = string.Empty;
            }

            return;
        }

        private static readonly object MakeLock = new object();

        public static List<EventSymbolFormat> EventSymbolList = null;

        private static RecvDeviceFormat RecvDeviceList = null;

        #region プロパティ
        
        public static Form MainForm { get; set; }

        public static IPlcCommunication PlcCommunication { get; set; }

        public static IDeviceManager DeviceManager { get; set; }

        public static IPlcSetting PlcSetting { get; set; }

        public static List<IBaseForm> FormList { get; set; }

        public static LogManager LogManager { get; set; }
        
        private static bool isConnecting = false;

        private static ReaderWriterLock ConnectingLock = new ReaderWriterLock();
        
        public static bool IsConnecting
        {
            get
            {
                try
                {
                    ConnectingLock.AcquireReaderLock(Timeout.Infinite);

                    return isConnecting;
                }
                finally
                {
                    ConnectingLock.ReleaseReaderLock();
                }
            }
            set
            {
                try
                {
                    ConnectingLock.AcquireWriterLock(Timeout.Infinite);

                    isConnecting = value;
                }
                finally
                {
                    ConnectingLock.ReleaseWriterLock();
                }
            }
        }

        #endregion

        public static void DataListMake()
        {
            if (LockDataListMake) { return; }

            Remoting Common = Remoting.Instance;

            lock (MakeLock)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                #region EventSymbolList作成

                EventSymbolList = new List<EventSymbolFormat>();

                List<IBaseForm> EntryFormList = new List<IBaseForm>();

                //// 所有している全てのMDIフォームと
                //EntryFormList.AddRange(MainForm.MdiChildren);
                //// MainProguramが表示している全てのフォーム
                //EntryFormList.AddRange(MainForm.OwnedForms);

                EntryFormList.AddRange(FormList);

                // フォームの所持しているSymbol_DrawをEventSymbolListに追加
                foreach (Form BaseForm in EntryFormList)
                {
                    AddEventSymbol(BaseForm);
                }

                #endregion

                #region RecvDeviceList作成

                RecvDeviceList = new RecvDeviceFormat();

                //EventSymbolListから作成する。
                for (int i = 0; i < EventSymbolList.Count; i++)
                {
                    int DataCount = EventSymbolList[i].DataCount;
                    //int Index_Dot = DeviceString.IndexOf('.');
                    DeviceElement Device = EventSymbolList[i].Device;

                    // 無効アドレス
                    if (Device == null)
                    {
                        continue;   // 受信リストに含めない
                    }

                    #region Comment Out
                    //#region デバイス分類：D
                    //if (DeviceString.Substring(0, 1) == "D")
                    //{
                    //    if (Index_Dot == -1)
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.D_Word_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.D_Word_Block);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.D_Bit_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.D_Bit_Block);
                    //        }
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：ZR
                    //if (DeviceString.Substring(0, 2) == "ZR")
                    //{
                    //    if (Index_Dot == -1)
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.ZR_Word_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.ZR_Word_Block);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.ZR_Bit_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.ZR_Bit_Block);
                    //        }
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：BUF
                    //if (DeviceString.Substring(0, 1) == "U")
                    //{
                    //    if (Index_Dot == -1)
                    //    {

                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.BUF_Word_Block);
                    //    }
                    //    else
                    //    {

                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.BUF_Bit_Block);

                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：X
                    //if (DeviceString.Substring(0, 1) == "X")
                    //{
                    //    if (DataCount == 1)
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.X_Single);
                    //    }
                    //    else
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.X_Block);
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：Y
                    //if (DeviceString.Substring(0, 1) == "Y")
                    //{
                    //    if (DataCount == 1)
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.Y_Single);
                    //    }
                    //    else
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.Y_Block);
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：M
                    //if (DeviceString.Substring(0, 1) == "M")
                    //{
                    //    if (DataCount == 1)
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.M_Single);
                    //    }
                    //    else
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.M_Block);
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：L
                    //if (DeviceString.Substring(0, 1) == "L")
                    //{
                    //    if (DataCount == 1)
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.L_Single);
                    //    }
                    //    else
                    //    {
                    //        RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.L_Block);
                    //    }
                    //}
                    //#endregion
                    //#region デバイス分類：KM
                    //if (DeviceString.Substring(0, 2) == "KM")
                    //{
                    //    if (Index_Dot == -1)
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.KM_Word_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.KM_Word_Block);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (DataCount == 1)
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.KM_Bit_Single);
                    //        }
                    //        else
                    //        {
                    //            RecvDeviceList_Add(DeviceString, DataCount, ref RecvDeviceList.KM_Bit_Block);
                    //        }
                    //    }
                    //}
                    //#endregion
                    #endregion
                    #region デバイス分類：ビットデバイス
                    if (Device.DeviceType == DeviceType.BitDevice)
                    {
                        if (DataCount == 1)
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.BitDevice_Single);
                        }
                        else
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.BitDevice_Block);
                        }
                    }
                    #endregion
                    #region デバイス分類：ワードデバイス
                    if (Device.DeviceType == DeviceType.WordDevice)
                    {
                        if (DataCount == 1)
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.WordDevice_Single);
                        }
                        else
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.WordDevice_Block);
                        }
                    }
                    #endregion
                    #region デバイス分類：バッファデバイス
                    if (Device.DeviceType == DeviceType.BufDevice)
                    {
                        RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.Buf_Block);
                    }
                    #endregion
                    #region デバイス分類：仮想デバイス
                    if (Device.DeviceType == DeviceType.VirtualDevice)
                    {
                        if (DataCount == 1)
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.Virtual_Single);
                        }
                        else
                        {
                            RecvDeviceList_Add(Device, DataCount, ref RecvDeviceList.Virtual_Block);
                        }
                    }
                    #endregion
                }

                Common.RecvDeviceList = RecvDeviceList;



                #endregion

                //Console.WriteLine("List作成完了 " + sw.ElapsedMilliseconds);
            }


        }

        private static void AddEventSymbol(Control control)
        {
            if (control is Symbol_Draw)
            {
                Symbol_Draw Symbol = (Symbol_Draw)control;

                foreach (PropertyInfo Property in Symbol.GetType().GetProperties())
                {
                    if (Property.PropertyType == typeof(DeviceFormat))
                    {
                        DeviceFormat DeviceData = (DeviceFormat)Property.GetValue(Symbol, null);

                        // 受信を行うDeviceFormat
                        if ((DeviceData.Io & IoType.In) != 0)
                        {
                            EventSymbolList_Add(DeviceData.Address, -1, DeviceData.DataCount, Symbol, Property);
                        }
                    }
                    else if (Property.PropertyType == typeof(DeviceFormat[]))
                    {
                        DeviceFormat[] DeviceDatas = (DeviceFormat[])Property.GetValue(Symbol, null);
                        for (int i = 0; i < DeviceDatas.Length; i++)
                        {
                            if ((DeviceDatas[i].Io & IoType.In) != 0)
                            {
                                EventSymbolList_Add(DeviceDatas[i].Address, i, DeviceDatas[i].DataCount, Symbol, Property);
                            }
                        }
                    }
                }
            }

            foreach (Control ChildControl in control.Controls)
            {
                AddEventSymbol(ChildControl);
            }
        }

        private static void EventSymbolList_Add(string DeviceString, int ArrayNumber, int DataCount, Symbol_Draw Symbol, PropertyInfo Property)
        {
            if (String.IsNullOrEmpty(DeviceString) == false)
            {
                Global.DeviceConvert(ref DeviceString);
                EventSymbolList.Add(new EventSymbolFormat(DeviceManager.ToElement(DeviceString), ArrayNumber, DataCount, Symbol, Property));
            }
        }

        private static void RecvDeviceList_Add(DeviceElement Device, int DataCount, ref List<DataFormat> DeviceList)
        {
            bool IsMatch = false;
            for (int i = 0; i < DeviceList.Count; i++)
            {
                //if (DeviceString == DeviceList[i].DeviceString)
                if(Device == DeviceList[i].Device
                    && DataCount == DeviceList[i].DataCount)
                {
                    IsMatch = true;
                    break;
                }
            }
            if (!IsMatch)
            {
                DeviceList.Add(new DataFormat(Device, DataCount));
            }


        }

        public static string GetDisplayName(Symbol_Draw Symbol, PropertyInfo Pi)
        {
            PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(Symbol);

            DisplayNameAttribute DisplayNameAtt = (DisplayNameAttribute)Properties[Pi.Name].Attributes[typeof(DisplayNameAttribute)];
            if (DisplayNameAtt == null) { return Pi.Name; }

            return DisplayNameAtt.DisplayName;
        }

        public static string GetDisplayName(Symbol_Draw Symbol, PropertyDescriptor Pd)
        {
            DisplayNameAttribute DisplayNameAtt = (DisplayNameAttribute)Pd.Attributes[typeof(DisplayNameAttribute)];
            if (DisplayNameAtt == null) { return Pd.Name; }

            return DisplayNameAtt.DisplayName;
        }

        public static bool GetVisibleAttribute(object obj, PropertyDescriptor Pd)
        {
            PropertyInfo Pi = obj.GetType().GetProperty(Pd.Name);

            VisibleAttribute attr = (VisibleAttribute)Attribute.GetCustomAttribute(Pi, typeof(VisibleAttribute));

            if (attr != null && attr.IsVisible) { return true; }

            return false;
        }

        public static bool GetVisibleAttribute(object obj, PropertyInfo Pi)
        {
            VisibleAttribute attr = (VisibleAttribute)Attribute.GetCustomAttribute(Pi, typeof(VisibleAttribute));

            if (attr != null && attr.IsVisible) { return true; }

            return false;
        }

        public static string VerInfo()
        {
            return "Ver.4.05α";
        }      
    }
}
