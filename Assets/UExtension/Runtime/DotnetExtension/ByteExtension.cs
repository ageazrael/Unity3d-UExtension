using System.Text;
using System;

namespace UExtension
{
    public enum LimitSize
    {
        Byte1 = 1,
        Byte2 = 2,
        Byte3 = 3,
        Byte4 = 4,
        Byte5 = 5,
        Byte6 = 6,
        Byte7 = 7,
        Byte8 = 8
    }

    public static class ByteExtension
    {
        public static string ToHEXString(this byte[] rSelf)
        {
            var rText = new StringBuilder();
            for (int nIndex = 0; nIndex < rSelf.Length; ++nIndex)
                rText.AppendFormat("{0:X}", rSelf[nIndex]);
            return rText.ToString();
        }

        #if _USE_UNSAFE
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, byte value)
        {
            return Write(rBytes, nStartIndex, &value, sizeof(byte));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, short value, LimitSize ls = LimitSize.Byte2)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(short), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, ushort value, LimitSize ls = LimitSize.Byte2)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(ushort), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, int value, LimitSize ls = LimitSize.Byte4)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(int), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, uint value, LimitSize ls = LimitSize.Byte4)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(uint), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, long value, LimitSize ls = LimitSize.Byte8)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(long), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, ulong value, LimitSize ls = LimitSize.Byte8)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(ulong), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, float value, LimitSize ls = LimitSize.Byte8)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(float), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, double value, LimitSize ls = LimitSize.Byte8)
        {
            return Write(rBytes, nStartIndex, (byte *)&value, Math.Min(sizeof(double), (int)ls));
        }
        public static unsafe bool Write(this byte[] rBytes, int nStartIndex, byte * value, int nWriteSize)
        {
            if (null == rBytes)
                return false;
            if (rBytes.Length <= nStartIndex + nWriteSize)
                return false;

            fixed(byte * ptr = rBytes)
            {
                Copy(ptr, nStartIndex, value, nWriteSize);
            }
            return true;
        }

        public static unsafe void Copy(byte * pBytes, int nStartIndex, byte * value, int nWriteSize)
        {
            for (int nIndex = 0; nIndex < nWriteSize; ++ nIndex)
                *(pBytes + nStartIndex + nIndex) = *(value + nIndex);
        }
        #endif
    }
}