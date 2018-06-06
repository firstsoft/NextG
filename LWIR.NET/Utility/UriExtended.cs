using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// Uri类的扩展
    /// </summary>
    public static class UriExtended
    {
        /// <summary>
        /// 递归判断目录是否是指定目录的父目录或是祖先目录
        /// </summary>
        /// <param name="parentDirectory">父目录</param>
        /// <param name="subDirectory">指定子目录</param>
        /// <returns></returns>
        public static bool IsParentOf(this Uri parentPath, Uri subPath)
        {
            if (parentPath == null || subPath == null)
            {
                return false;
            }

            // 提取本地完整路径，去除路径尾部多余的分隔符
            string parentPathStr = parentPath.GetValidPath();
            string subPathStr = subPath.GetValidPath();

            // 如果路径相等即为符合判断条件，否则判断指定目录的父级目录，直至父级目录筛选到根目录
            if (parentPathStr.Equals(subPathStr))
            {
                return true;
            }

            var subDir = new DirectoryInfo(subPathStr);
            var parentDir = subDir.Parent;

            if (parentDir == null)
            {
                return false;
            }

            return parentPath.IsParentOf(new Uri(parentDir.FullName));
        }

        /// <summary>
        /// 获取目录的有效完整路径
        /// </summary>
        /// <param name="curUri"></param>
        /// <returns></returns>
        public static string GetValidPath(this Uri curUri)
        {
            return Path.GetFullPath(curUri.LocalPath)
               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
               .ToUpperInvariant();
        }
    }
}
