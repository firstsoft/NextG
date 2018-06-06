using AMEC.Native;
using LWIR.NET.Common.Event;
using LWIR.NET.Entity;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AMEC.WH.Controls
{
    [Export(typeof(Wd2ViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Wd2ViewModel : ViewModelBase
    {
        private IRegionManager inRegionManager;
        private IEventAggregator inEventAggregator;
        public RelayCommand CurvesChangedCommand { private set; get; }

        private WD2Info[] m_WD2Infos = null;
        public WD2Info[] WD2Infos
        {
            get { return m_WD2Infos; }
            set { Set(() => WD2Infos, ref m_WD2Infos, value); }
        }

        private DateTime minX = DateTime.Now.Date;
        public DateTime MinX
        {
            get { return minX; }
            set { Set(() => MinX, ref minX, value); }
        }

        private DateTime maxX = DateTime.Now.Date.AddSeconds(15);
        public DateTime MaxX
        {
            get { return maxX; }
            set { Set(() => MaxX, ref maxX, value); }
        }

        private double minY = 0.0;
        public double MinY
        {
            get { return minY; }
            set { Set(() => MinY, ref minY, value); }
        }

        private double maxY = 1.0;
        public double MaxY
        {
            get { return maxY; }
            set { Set(() => MaxY, ref maxY, value); }
        }

        private List<CurvesData> curvesList = new List<CurvesData>();
        public List<CurvesData> CurvesList
        {
            get { return curvesList; }
            set { Set(() => CurvesList, ref curvesList, value); }
        }

        [ImportingConstructor]
        public Wd2ViewModel(IRegionManager _inRegionManager, IEventAggregator _inEventAggregator)
        {
            inRegionManager = _inRegionManager;
            inEventAggregator = _inEventAggregator;

            CurvesChangedCommand = new RelayCommand(CurvesChanged);
            inEventAggregator.GetEvent<Wd2FilesEvent>().Subscribe(Wd2FilesRead_Callback);
        }

        private void CurvesChanged()
        {
            if (WD2Infos == null)
                return;

            CurvesList = WD2Infos.SelectMany(info=>info.RecipeItems.Where(r=>r.IsSelected)).ToList<CurvesData>();
            
            if (CurvesList.Count==0)
            {
                MinX = DateTime.Now.Date;
                MaxX = MinX.AddMilliseconds(15);
                MinY = 0.0;
                MaxY = 1.0;
                return;
            }

            MinX = CurvesList.Min(c => c.DataPoints.Min(x => x.X));
            MaxX = CurvesList.Max(c => c.DataPoints.Max(x => x.X));

            double MinY1 = CurvesList.Min(c => c.DataPoints.Min(x => x.Y));
            double MaxY1 = CurvesList.Max(c => c.DataPoints.Max(x => x.Y));

            double gap = (MaxY - MinY) / 5;
            MinY = MinY1 - gap;
            MaxY = MaxY1 + gap;
        }

        private void Wd2FilesRead_Callback(string[] wd2Files)
        {
            WD2Infos = new WD2Info[wd2Files.Length];

            for (int num = 0; num < wd2Files.Length; num++)
            {
                WD2Info info = new WD2Info();
                info.CurFileInfoEx = new FileInfoEx();
                info.CurFileInfoEx.ParentDir = Directory.GetParent(wd2Files[num]).FullName;
                info.CurFileInfoEx.Name = Path.GetFileName(wd2Files[num]);
                WD2Infos[num] = info;

                LWIR.NET.Framework.Utility.ThreadPool.Instance.Add(() => info.FillInfos());
            }
        }

        public override void Dispose()
        {
            inEventAggregator.GetEvent<Wd2FilesEvent>().Unsubscribe(Wd2FilesRead_Callback);
        }
    }
}
