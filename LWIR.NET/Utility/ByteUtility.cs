using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Utility
{
    public class ByteUtility
    {
        /// <summary>
        /// Judge a msgId is an extended one.
        /// </summary>
        /// <param name="checkByte">a byte to check</param>
        /// <returns>true if hightest bit is 1, otherwise false</returns>
        public static bool IsMsgIdEx(byte checkByte)
        {
            return (checkByte >> 7) == 1;
        }

        /// <summary>
        /// Get extended msgId from short value
        /// </summary>
        /// <param name="byte0"></param>
        /// <param name="byte1"></param>
        /// <returns>msgId</returns>
        public static ushort GetMsgIdEx(byte byte0, byte byte1)
        {
            ushort preId = GetUshortValueFromBytes(byte0, byte1);
            return (ushort)(preId & (~(1 << 15)));
        }

        /// <summary>
        /// Get byte array from message id
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public static byte[] GetMsgIdBytes(ushort msgId)
        {
            if (msgId <= 0x7F)
            {
                return new byte[] { (byte)msgId };
            }
            else
            {
                msgId = (ushort)(msgId | (1 << 15));
                return BitConverter.GetBytes(msgId);
            }
        }

        /// <summary>
        /// Get Unshort value from bytes
        /// </summary>
        /// <param name="byte0"></param>
        /// <param name="byte1"></param>
        /// <returns></returns>
        public static ushort GetUshortValueFromBytes(byte byte0, byte byte1)
        {
            return BitConverter.ToUInt16(BitConverter.IsLittleEndian ? new byte[2] { byte0, byte1 } : new byte[2] { byte1, byte0 }, 0);
        }

        /// <summary>
        /// 字节转16进制字符串
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static string Bytes2HexStr(byte[] bs)
        {
            return BitConverter.ToString(bs).Replace("-", " ");
        }

        /// <summary>
        /// 转换成小端字节的float
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetSingle(float input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToSingle(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的double
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double GetDouble(double input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToDouble(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的short
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short GetInt16(short input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToInt16(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的ushort
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ushort GetUInt16(ushort input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToUInt16(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetInt32(int input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToInt32(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的uint
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static uint GetUInt32(uint input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToUInt32(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的long
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long GetInt64(long input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToInt64(tempArray, 0);
            }
        }

        /// <summary>
        /// 转换成小端字节的ulong
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ulong GetUInt64(ulong input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input;
            }
            else
            {
                byte[] tempArray = BitConverter.GetBytes(input);
                Array.Reverse(tempArray, 0, tempArray.Length);
                return BitConverter.ToUInt64(tempArray, 0);
            }
        }
    }
}
