using Microsoft.Win32;
using LWIR.NET.Common.Entity;
using LWIR.NET.Common.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;

namespace LWIR.NET.Common.Model
{
    /// <summary>
    /// 语言模型
    /// </summary>
    public class LanguageModel
    {
        private readonly static Lazy<LanguageModel> lazy = new Lazy<LanguageModel>(() => new LanguageModel());
        private LanguageInfo curLanguage = null;
        private List<LanguageInfo> languageInfoList = new List<LanguageInfo>();

        /// <summary>
        /// 语言变化的通知
        /// </summary>
        public Action LanguageChanged;

        private LanguageModel()
        {
            //构造可用的语言列表
            FieldInfo[] fieldArray = typeof(LanguageEnum).GetFields();

            foreach (var f in fieldArray)
            {
                var attrObj = f.GetCustomAttributes(typeof(EnumDescriptionAttribute), false).FirstOrDefault();

                if (attrObj == null)
                    continue;

                EnumDescriptionAttribute attr = attrObj as EnumDescriptionAttribute;
                LanguageInfo info = new LanguageInfo(attr.LanguageName.Equals("English"), attr.LanguageFlag, attr.LanguageName, (LanguageEnum)f.GetValue(typeof(LanguageEnum)));
                languageInfoList.Add(info);
            }

            //先默认一个系统语言
            curLanguage = languageInfoList.FirstOrDefault(x => x.IsDefault == true);

            //读取软件语言，如果没有语言选项则默认当前系统语言，如果系统语言不支持则默认英文
            string curLanguageFlag = Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(curLanguageFlag))
            {
                //如果该语言与语言表有匹配则使用匹配语言作为软件语言，否则默认系统语言
                var selectLan = this.languageInfoList.FirstOrDefault(lan => lan.LanguageFlag.Equals(curLanguageFlag));
                SetCurLanguage(selectLan ?? languageInfoList.FirstOrDefault(x => x.IsDefault == true));
            }
            else
            {
                //没有保存的语言则根据系统语言选择
                var curSysLanguage = languageInfoList.FirstOrDefault(x => x.LanguageFlag.Equals(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName));
                SetCurLanguage(curSysLanguage ?? languageInfoList.FirstOrDefault(x => x.IsDefault == true));
            }
        }

        public static LanguageModel Instance
        {
            get { return lazy.Value; }
        }

        /// <summary>
        /// 设置当前系统语言
        /// </summary>
        /// <param name="info">当前系统语言</param>
        private void SetCurLanguage(LanguageInfo info)
        {
            info = info ?? languageInfoList.FirstOrDefault(x => x.IsDefault == true);

            //加载新资源文件后删除老资源
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(info.LanguageFile) });
            ResourceDictionary oldRes = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Equals(this.curLanguage.LanguageFile));

            if (oldRes != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldRes);
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(info.LanguageFlag);

            //保存语言
            this.curLanguage = info;
            Properties.Settings.Default.Language = info.LanguageFlag;
            Properties.Settings.Default.Save();

            if (this.LanguageChanged != null)
            {
                LanguageChanged();
            }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public LanguageInfo CurLanguage
        {
            get { return curLanguage; }
        }

        /// <summary>
        /// 选择语言进行切换
        /// </summary>
        /// <param name="selectedLanguage">选择的语言</param>
        public void SelectLanguage(LanguageEnum selectedLanguage)
        {
            SetCurLanguage(languageInfoList.FirstOrDefault(x => x.CurLanguage == selectedLanguage));
        }

        /// <summary>
        /// 根据语言简称切换语言
        /// </summary>
        /// <param name="languageFlag"></param>
        public void SelectLanguage(string languageFlag)
        {
            SetCurLanguage(languageInfoList.FirstOrDefault(x => x.LanguageFlag.Equals(languageFlag)));
        }
    }
}
