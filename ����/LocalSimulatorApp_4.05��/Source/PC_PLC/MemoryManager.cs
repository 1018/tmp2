using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using PcMemoryLib;

namespace PC_PLC
{
    public static class MemoryManager
    {
        static MemMng PcMemory;

        static MemoryManager()
        {
            PcMemory = new MemMng();
        }

        public static int OpenMemory()
        {
            return PcMemory.OpenMemory();
        }

        public static int ReadBit(int type, int address, int size, ref bool[] readData)
        {
            return PcMemory.ReadBit(type, address, size, ref readData);
        }

        public static int ReadBuf(int unitNum, int address, int size, ref ushort[] readData)
        {
            return PcMemory.ReadBuf(unitNum, address, size, ref readData);
        }
        public static void ReadBufBit(int unitNum, int wordNum, int bitNum, ref bool readData)
        {
            PcMemory.ReadBufBit(unitNum, wordNum, bitNum, ref readData);
        }

        public static void WriteBit(int type, int address, int size, bool[] writeData)
        {
            PcMemory.WriteBit(type, address, size, writeData);
        }

        public static void WriteBuf(int unitNum, int address, int size, ushort[] writeData)
        {
            PcMemory.WriteBuf(unitNum, address, size, writeData);
        }

        public static void WriteBufBit(int unitNum, int wordNum, int bitNum, bool writeData)
        {
            PcMemory.WriteBufBit(unitNum, wordNum, bitNum, writeData);
        }
        

    }
}
