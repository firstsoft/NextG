using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>16进制32位MD5密码</returns>
        public static string GetMd5_32String(string str)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string t2 = BitConverter.ToString(GetMd5_32(str));
                return t2.Replace("-", "");
            }
        }

        /// <summary>
        /// 字符串加密为16位Md5
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>16进制16位MD5密码</returns>
        public static string GetMd5_16String(string str)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string t2 = BitConverter.ToString(GetMd5_32(str), 4, 8);
                return t2.Replace("-", "");
            }
        }

        /// <summary>
        /// 获取Md5码的字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetMd5_32(string str)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        /// <summary>
        /// Get string from md5 bytes
        /// </summary>
        /// <param name="md5Bytes"></param>
        /// <param name="isBit32">Is 32bit pwd</param>
        /// <returns></returns>
        public static string GetStringFromMd5Bytes(byte[] md5Bytes, bool isBit32 = true)
        {
            if (md5Bytes == null || md5Bytes.Length != 16)
                return string.Empty;

            return isBit32 ? BitConverter.ToString(md5Bytes).Replace("-", "") : BitConverter.ToString(md5Bytes, 4, 8).Replace("-", "");
        }
    }
}
