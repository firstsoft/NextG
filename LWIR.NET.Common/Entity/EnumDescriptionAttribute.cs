using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Common.Entity
{
    /// <summary>
    /// 枚举的描述属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string languageFlag = string.Empty;
        /// <summary>
        /// 语言标识
        /// </summary>
        public string LanguageFlag
        {
            get { return languageFlag; }
            set { languageFlag = value; }
        }


        private string languageName = string.Empty;
        /// <summary>
        /// 语言名
        /// </summary>
        public string LanguageName
        {
            get { return languageName; }
            set { languageName = value; }
        }
    }
}
