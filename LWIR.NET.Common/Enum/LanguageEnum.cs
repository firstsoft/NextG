using LWIR.NET.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LWIR.NET.Common.Enum
{
    /// <summary>
    /// 支持的语言的枚举
    /// </summary>
    public enum LanguageEnum : uint
    {
        /// <summary>
        /// 德语
        /// </summary>
        [EnumDescriptionAttribute(LanguageFlag = "de", LanguageName = "German")]
        German,

        /// <summary>
        /// 英文
        /// </summary>
        [EnumDescriptionAttribute(LanguageFlag = "en", LanguageName = "English")]
        English,

        /// <summary>
        /// 中文
        /// </summary>
        [EnumDescriptionAttribute(LanguageFlag = "zh", LanguageName = "简体中文")]
        Chinese,
    }
}
