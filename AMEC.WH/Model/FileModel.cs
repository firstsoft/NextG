using AMEC.Native;
using LWIR.NET.Common.Event;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace AMEC.WH.Model
{
    internal class FileModel
    {
        private readonly object lockobj = new object();
        private readonly static Lazy<FileModel> lazy = new Lazy<FileModel>(() => new FileModel());
        private Dictionary<String, SH2Info> sh2Dic = new Dictionary<string, SH2Info>();
        private Dictionary<String, WaferHistoryInfo> wh3Dic = new Dictionary<string, WaferHistoryInfo>();
        private List<SH2Info> sh2Collection = new List<SH2Info>();
        private List<WaferHistoryInfo> wh3Collection = new List<WaferHistoryInfo>();

        public Action PostSH2InfoEvent;
        public Action PostWaferHistoryInfoEvent;
        public Action<long, long> PostFilesCountEvent;

        public List<SH2Info> Sh2Collection
        {
            get { return sh2Collection; }
        }

        public List<WaferHistoryInfo> Wh3Collection
        {
            get { return wh3Collection; }
        }

        public WaferHistoryInfo GetWaferHistoryInfo(string fullpath)
        {
            if (wh3Dic.ContainsKey(fullpath))
            {
                return wh3Dic[fullpath];
            }
            else
            {
                return null;
            }
        }

        private FileModel()
        {
            LWIR.NET.Framework.Utility.ThreadPool.Instance.SpecifyCpuEnabled = true;
            LWIR.NET.Framework.Utility.ThreadPool.Instance.Start((uint)Environment.ProcessorCount);
        }

        public static FileModel Instance
        {
            get { return lazy.Value; }
        }


        public void ReceiveHistoryFolder(string historyFolder)
        {
            sh2Dic.Clear();
            wh3Dic.Clear();
            sh2Collection.Clear();
            wh3Collection.Clear();

            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                IEnumerable<FileInfoEx> curSh2Enumerable = DirectoryEx.GetFiles(historyFolder, "*.sh2", SearchOption.AllDirectories);
                IEnumerable<FileInfoEx> curWh3Enumerable = DirectoryEx.GetFiles(historyFolder, "*.wh3", SearchOption.AllDirectories);
                IEnumerable<FileInfoEx> curCh3Enumerable = DirectoryEx.GetFiles(historyFolder, "*.ch3", SearchOption.AllDirectories);

                if (PostFilesCountEvent != null)
                {
                    PostFilesCountEvent(curSh2Enumerable.Count(), curWh3Enumerable.Count() + curCh3Enumerable.Count());
                }

                foreach (FileInfoEx f in curSh2Enumerable)
                {
                    ReadDetailInfo(f);
                }

                if (PostSH2InfoEvent != null)
                {
                    PostSH2InfoEvent();
                }

                foreach (FileInfoEx f in curWh3Enumerable)
                {
                    ReadDetailInfo(f);
                }

                foreach (FileInfoEx f in curCh3Enumerable)
                {
                    ReadDetailInfo(f);
                }

                if (PostWaferHistoryInfoEvent != null)
                {
                    PostWaferHistoryInfoEvent();
                }
            }));

            th.Start();
        }


        private void ReadDetailInfo(FileInfoEx info)
        {
            if (info.GetFileType() == FileType.SH2)
            {
                SH2Info sh2 = new SH2Info();
                sh2.CurFileInfoEx = info;

                if (!sh2Dic.ContainsKey(info.Fullfile))
                {
                    sh2Dic.Add(info.Fullfile, sh2);
                    sh2Collection.Add(sh2);
                    //sh2.FillInfos();
                    LWIR.NET.Framework.Utility.ThreadPool.Instance.Add(() => sh2.FillInfos());
                }

                return;
            }

            if (info.GetFileType() == FileType.WH3 || info.GetFileType() == FileType.CH3)
            {
                WaferHistoryInfo wh3 = new WaferHistoryInfo();
                wh3.CurFileInfoEx = info;

                if (!wh3Dic.ContainsKey(info.Fullfile))
                {
                    wh3Dic.Add(info.Fullfile, wh3);
                    wh3Collection.Add(wh3);
                    //wh3.FillInfos();
                    LWIR.NET.Framework.Utility.ThreadPool.Instance.Add(() => wh3.FillInfos());
                }

                return;
            }
        }
    }
}
