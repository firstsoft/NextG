using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AMEC.WH.Enum
{
    public enum GroupType1
    {
        [Description("系统")]
        SYSTEM = 0,

        [Description("前端传送机")]
        EFEM = 1,

        [Description("真空进样室")]
        LL = 2,

        [Description("缓存室")]
        BF = 3,

        [Description("传送腔")]
        TM = 4,

        [Description("反应腔")]
        PM = 5,

        [Description("工厂自动化")]
        FA = 6,

        [Description("缺陷检测分类")]
        FDC = 7,

        [Description("气体")]
        GAS = 8,

        [Description("参数绘图")]
        DP = 9,

        [Description("脉冲")]
        PULSE = 10,

        [Description("真空晶圆盒升降机")]
        VCE = 11,
    }
}
