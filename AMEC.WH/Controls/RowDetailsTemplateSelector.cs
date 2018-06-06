using AMEC.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AMEC.WH.Controls
{
    public class RowDetailsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate FlowRatioTemplate { get; set; }
        public DataTemplate PulseTemplate { get; set; }
        public DataTemplate RampTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is RecipeParam))
                return null;

            RecipeParam curParam = item as RecipeParam;

            if (curParam.CurAuxData == null)
                return null;

            if (curParam.CurAuxData is AuxEnum)
            {
                return EnumTemplate;
            }

            if (curParam.CurAuxData is AuxGasSplitter)
            {
                return FlowRatioTemplate;
            }

            if (curParam.CurAuxData is AuxGasPulse)
            {
                return PulseTemplate;
            }

            if (curParam.CurAuxData is AuxGasRamp)
            {
                return RampTemplate;
            }

            return null;
        }
    }
}
