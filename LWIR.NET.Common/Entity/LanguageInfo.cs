using LWIR.NET.Common.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LWIR.NET.Common.Entity
{
    public class LanguageInfo
    {
        private bool isDefault = false;
        private string languageFlag = "en";
        private string languageName = "English";
        private LanguageEnum curLanguage = LanguageEnum.English;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_isDefault">是否是默认语言</param>
        /// <param name="_languageFlag">语言简称字母</param>
        /// <param name="_languageName">语言名</param>
        /// <param name="_curLanguage">当前语言</param>
        public LanguageInfo(bool _isDefault, string _languageFlag, string _languageName, LanguageEnum _curLanguage)
        {
            isDefault = _isDefault;
            languageFlag = _languageFlag;
            languageName = _languageName;
            curLanguage = _curLanguage;
        }

        /// <summary>
        /// 是否是默认语言
        /// </summary>
        public bool IsDefault { get { return isDefault; } }

        /// <summary>
        /// 语言简写字母
        /// </summary>
        public string LanguageFlag { get { return languageFlag; } }

        /// <summary>
        /// 语言名称
        /// </summary>
        public string LanguageName { get { return languageName; } }

        public LanguageEnum CurLanguage { get { return curLanguage; } }

        /// <summary>
        /// 语言文件
        /// </summary>
        public string LanguageFile
        {
            get
            {
                if (IsDefault)
                {
                    return "pack://application:,,,/LWIR.NET.Common;Component/Languages/en/Strings.xaml";
                }
                else
                {
                    string file = string.Format("{0}/Languages/{1}/Strings.xaml", AppDomain.CurrentDomain.BaseDirectory, this.LanguageFlag);
                    return !File.Exists(file) ? "pack://application:,,,/LWIR.NET.Common;Component/Languages/en/Strings.xaml" : file;
                }
            }
        }
    }
}
