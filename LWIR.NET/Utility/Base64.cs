using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// Base64加密解密
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// 根据字符编码对字符串进行加密
        /// </summary>
        /// <param name="sourceStr">需加密的字符串</param>
        /// <param name="encoding">指定字符编码</param>
        /// <returns>Base64密码</returns>
        public static string Encode(string sourceStr, Encoding encoding)
        {
            byte[] originalArray = encoding.GetBytes(sourceStr);
            return Convert.ToBase64String(originalArray);
        }

        /// <summary>
        /// 对字节数组进行加密生成base64字符串，主要针对图片加密
        /// </summary>
        /// <param name="originalArray">数据源</param>
        /// <returns>Base64密码</returns>
        public static string Encode(byte[] originalArray)
        {
            return Convert.ToBase64String(originalArray);
        }

        /// <summary>
        /// 根据指定字符编码对base64字符串进行解密
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="encoding"></param>
        /// <returns>解密后文字</returns>
        public static string Decode(string sourceStr, Encoding encoding)
        {
            byte[] originalArray = Convert.FromBase64String(sourceStr);
            return encoding.GetString(originalArray);
        }

        /// <summary>
        /// 根据base64字符串进行解密，主要针对图片
        /// </summary>
        /// <param name="sourceStr">需解密的base64字符串</param>
        /// <returns>解密后字节数组</returns>
        public static byte[] Decode(string sourceStr)
        {
            return Convert.FromBase64String(sourceStr);
        }
    }
}
