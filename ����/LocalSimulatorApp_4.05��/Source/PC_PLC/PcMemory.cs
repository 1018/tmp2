using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using PC_PLC;
using System.Threading;

namespace PcMemoryLib
{
    public class MemMng
    {
        class SafeOpenFileMappingHandle : SafeHandle
        {
            public SafeOpenFileMappingHandle()
                : base(IntPtr.Zero, true)
            {
            }

            public SafeOpenFileMappingHandle(IntPtr handle)
                : base(handle, true)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid
            {
                get { return IsClosed || this.handle == IntPtr.Zero; }
            }

            protected override bool ReleaseHandle()
            {
                return Win32API.UnmapViewOfFile(this.handle);
            }
        }

        class SafeMappingFileHandle : SafeHandle
        {
            public SafeMappingFileHandle()
                : base(IntPtr.Zero, true)
            {
            }

            public SafeMappingFileHandle(IntPtr handle)
                : base(handle, true)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid
            {
                get { return IsClosed || this.handle == IntPtr.Zero; }
            }

            protected override bool ReleaseHandle()
            {
                return Win32API.CloseHandle(this.handle);
            }
        }

        const int MAX_SIZE = 65535;
        const int MAX_BUF_UNIT = 50;
        const int VM_D = 0;
        const int VM_X = 1;
        const int VM_Y = 2;
        const int VM_M = 3;
        const int VM_ZR = 4;
        const int VM_BUF = 5;

        SafeOpenFileMappingHandle hVM_X;
        SafeMappingFileHandle pVM_X;
        SafeOpenFileMappingHandle hVM_Y;
        SafeMappingFileHandle pVM_Y;
        SafeOpenFileMappingHandle hVM_M;
        SafeMappingFileHandle pVM_M;

        SafeOpenFileMappingHandle hVM_CHK;
        SafeMappingFileHandle pVM_CHK;


        SafeOpenFileMappingHandle[] hVM_BUF = new SafeOpenFileMappingHandle[MAX_BUF_UNIT];
        SafeMappingFileHandle[] pVM_BUF = new SafeMappingFileHandle[MAX_BUF_UNIT];

        SafeOpenFileMappingHandle[] hVM_BUF_CPU = new SafeOpenFileMappingHandle[5];
        SafeMappingFileHandle[] pVM_BUF_CPU = new SafeMappingFileHandle[5];

        private byte lifeCounter = 0;

        public MemMng()
        {            
        }

        private void CheckConnect()
        {
            while (true)
            {
                lifeCounter++;

                unsafe
                {
                    byte* pData = (byte*)pVM_CHK.DangerousGetHandle();
                    if (pData != null)
                    {
                        pData[0] = lifeCounter;
                    }
                }


                Thread.Sleep(1000);
            }
        }



        public int OpenMemory()
        {
            hVM_X = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder("VM_X")));
            pVM_X = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_X.DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));

            hVM_Y = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder("VM_Y")));
            pVM_Y = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_Y.DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));

            hVM_M = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder("VM_M")));
            pVM_M = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_M.DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));

            hVM_CHK = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder("VM_CHK")));
            pVM_CHK = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_CHK.DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));

            for (int i = 0; i < MAX_BUF_UNIT; i++)
            {
                string vmName = string.Format("VM_BUF_{0:X2}", i);

                hVM_BUF[i] = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder(vmName)));
                pVM_BUF[i] = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_BUF[i].DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));
            }

            for (int i = 0; i < 5; i++)
            {
                string vmName = string.Format("VM_BUF_{0:X3}", i + 0x3E0);

                hVM_BUF_CPU[i] = new SafeOpenFileMappingHandle(Win32API.OpenFileMapping((UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, new StringBuilder(vmName)));
                pVM_BUF_CPU[i] = new SafeMappingFileHandle(Win32API.MapViewOfFile(hVM_BUF_CPU[i].DangerousGetHandle(), (UInt32)Win32API.FILE_MAP_ALL_ACCESS, 0, 0, 0));
            }

            Thread chk = new Thread(new ThreadStart(CheckConnect));
            chk.Start();
            chk.IsBackground = true;


            if (pVM_X.DangerousGetHandle() == IntPtr.Zero ||
                pVM_Y.DangerousGetHandle() == IntPtr.Zero ||
                pVM_M.DangerousGetHandle() == IntPtr.Zero)
            {
                return -1;
            }

            return 0;
        }

        //size = ビット数, data = ビット配列
        public void WriteBit(int type, int address, int size, bool[] data)
        {
            unsafe
            {
                byte* pVmData = null;

                if (type == VM_X)
                {
                    pVmData = (byte*)pVM_X.DangerousGetHandle();
                }
                else if (type == VM_Y)
                {
                    pVmData = (byte*)pVM_Y.DangerousGetHandle();
                }
                else if (type == VM_M)
                {
                    pVmData = (byte*)pVM_M.DangerousGetHandle();
                }
                else
                {
                    return;
                }

                if (pVmData == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\ntype = {0}", type));
                }

                for (int i = 0; i < size; i++)
                {
                    int indexNum = (address + i) / 8;
                    int bitNum = (address + i) - (indexNum * 8);

                    if (data[i])
                    {
                        pVmData[indexNum] |= (byte)(0x01 << bitNum);
                    }
                    else
                    {
                        pVmData[indexNum] &= (byte)~(0x01 << bitNum);
                    }
                }
            }
        }

        //size = ワード数, data = ワード配列
        public void WriteWord(int type, int address, int size, UInt16[] data)
        { }
        //size = ワード数, data = ワード配列
        public void WriteBuf(int unitNum, int address, int size, UInt16[] data)
        {
            unsafe
            {
                UInt16* pArea = null;

                //CPUバッファ判断
                if (unitNum < 0x3e0)
                {
                    pArea = (UInt16*)pVM_BUF[unitNum].DangerousGetHandle();
                }
                else
                {
                    pArea = (UInt16*)pVM_BUF_CPU[unitNum - 0x3e0].DangerousGetHandle();
                }

                if (pArea == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\nunitNum = {0}", unitNum));
                }

                for (int i = 0; i < size; i++)
                {
                    pArea[address + i] = data[i];
                }
            }

        }

        public void WriteBufBit(int unitNum, int wordNum, int bitNum, bool data)
        {
            unsafe
            {
                UInt16* pArea = null;

                //CPUバッファ判断
                if (unitNum < 0x3e0)
                {
                    pArea = (UInt16*)pVM_BUF[unitNum].DangerousGetHandle();
                }
                else
                {
                    pArea = (UInt16*)pVM_BUF_CPU[unitNum - 0x3e0].DangerousGetHandle();
                }

                if (pArea == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\nunitNum = {0}", unitNum));
                }

                if (data)
                {
                    pArea[wordNum] |= (UInt16)(0x01 << bitNum);
                }
                else
                {
                    pArea[wordNum] &= (UInt16)~(0x01 << bitNum);
                }
            }
        }

        //size = ビット数, data = ビット配列
        public int ReadBit(int type, int address, int size, ref bool[] data)
        {
            unsafe
            {
                byte* pVmData = null;

                if (type == VM_X)
                {
                    pVmData = (byte*)pVM_X.DangerousGetHandle();
                }
                else if (type == VM_Y)
                {
                    pVmData = (byte*)pVM_Y.DangerousGetHandle();
                }
                else if (type == VM_M)
                {
                    pVmData = (byte*)pVM_M.DangerousGetHandle();
                }
                else
                {
                    return 0;
                }

                if (pVmData == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\ntype = {0}", type));
                }

                for (int i = 0; i < size; i++)
                {
                    int indexNum = (address + i) / 8;
                    int bitNum = (address + i) - (indexNum * 8);

                    if ((pVmData[indexNum] & (0x01 << bitNum)) != 0)
                    {
                        data[i] = true;
                    }
                    else
                    {
                        data[i] = false;
                    }
                }
            }

            return size;
        }

        //size = ワード数, data = ワード配列
        public int ReadWord(int type, int address, int size, ref UInt16[] data)
        {
            return 0;
        }

        //size = ワード数, data = ワード配列
        public int ReadBuf(int unitNum, int address, int size, ref UInt16[] data)
        {
            unsafe
            {
                UInt16* pArea = null;

                if (unitNum < 0x3e0)
                {
                    pArea = (UInt16*)pVM_BUF[unitNum].DangerousGetHandle();
                }
                else
                {
                    pArea = (UInt16*)pVM_BUF_CPU[unitNum - 0x3e0].DangerousGetHandle();
                }

                if (pArea == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\nunitNum = {0}", unitNum));
                }

                for (int i = 0; i < size; i++)
                {
                    data[i] = pArea[address + i];
                }
            }

            return size;
        }

        public void ReadBufBit(int unitNum, int wordNum, int bitNum, ref bool data)
        {
            unsafe
            {
                UInt16* pArea = null;

                if (unitNum < 0x3e0)
                {
                    pArea = (UInt16*)pVM_BUF[unitNum].DangerousGetHandle();
                }
                else
                {
                    pArea = (UInt16*)pVM_BUF_CPU[unitNum - 0x3e0].DangerousGetHandle();
                }

                if (pArea == null)
                {
                    throw new InvalidOperationException(
                        string.Format("共有メモリにアクセス出来ません。\nunitNum = {0}", unitNum));
                }

                data = ((pArea[wordNum] & (0x01 << bitNum)) != 0);
            }

            return;
        }
    }
}
