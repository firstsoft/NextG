using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// DES加密解密
    /// </summary>
    public class DES
    {
        #region 加密
        /// <summary> 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt2String(string str, string EcryptKey, string InitialVector)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            byte[] data = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(Encrypt(data, EcryptKey, InitialVector));
        }

        /// <summary>
        /// 加密后返回字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="EcryptKey"></param>
        /// <param name="InitialVector"></param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] Encrypt2ByteArray(string str, string EcryptKey, string InitialVector)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            byte[] data = Encoding.UTF8.GetBytes(str);
            return Encrypt(data, EcryptKey, InitialVector);
        }

        /// <summary> 加密byte[]   
        /// </summary>  
        /// <param name="str">要加密的byte[]</param>  
        /// <returns>加密后的byte[]</returns>  
        private static byte[] Encrypt(byte[] data, string EcryptKey, string InitialVector)
        {
            if (data == null)
            {
                return null;
            }

            try
            {
                using (MemoryStream MStream = new MemoryStream())//实例化内存流对象  
                {
                    DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   
                    byte[] key = Encoding.UTF8.GetBytes(EcryptKey); //定义字节数组，用来存储密钥 
                    byte[] iV = Encoding.UTF8.GetBytes(InitialVector); //初始化向量

                    if (key.Length != 8 || iV.Length != 8)
                    {
                        throw new Exception("密码和向量必须是8字节64位");
                    }

                    //使用内存流实例化加密流对象   
                    CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, iV), CryptoStreamMode.Write);
                    CStream.Write(data, 0, data.Length);  //向加密流中写入数据      
                    CStream.FlushFinalBlock();//释放加密流    
                    CStream.Close();
                    return MStream.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 解密
        /// <summary>  
        /// 解密字符串  
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt2String(string str, string DecryptKey, string InitialVector)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  
            return Encoding.UTF8.GetString(Decrypt(data, DecryptKey, InitialVector));
        }

        /// <summary>
        /// 解密字符串为字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="DecryptKey"></param>
        /// <param name="InitialVector"></param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] Decrypt2ByteArray(string str, string DecryptKey, string InitialVector)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            byte[] data = Encoding.UTF8.GetBytes(str);//定义字节数组，用来存储要解密的字符串  
            return Decrypt(data, DecryptKey, InitialVector);
        }

        /// <summary>  
        /// 解密byte[]   
        /// </summary>  
        /// <param name="str">要解密的byte[] </param>  
        /// <returns>解密后的byte[] </returns>  
        private static byte[] Decrypt(byte[] data, string DecryptKey, string InitialVector)
        {
            if (data == null)
            {
                return null;
            }

            try
            {
                using (MemoryStream MStream = new MemoryStream())//实例化内存流对象
                {
                    DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    
                    byte[] key = Encoding.UTF8.GetBytes(DecryptKey); //定义字节数组，用来存储密钥  
                    byte[] iV = Encoding.UTF8.GetBytes(InitialVector); //初始化向量

                    if (key.Length != 8 || iV.Length != 8)
                    {
                        throw new Exception("密码和向量必须是8字节64位");
                    }

                    //使用内存流实例化解密流对象       
                    CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, iV), CryptoStreamMode.Write);
                    CStream.Write(data, 0, data.Length);      //向解密流中写入数据     
                    CStream.FlushFinalBlock();               //释放解密流    
                    CStream.Close();
                    return MStream.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
