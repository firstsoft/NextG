using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR
{
    public enum MOD_ID
    {
        MOD_ID_ALL,
        MOD_ID_LOADLOCK_A,
        MOD_ID_LOADLOCK_B,
        MOD_ID_PROCESS_MODULE_A1,
        MOD_ID_PROCESS_MODULE_A2,
        MOD_ID_PROCESS_MODULE_B1,
        MOD_ID_PROCESS_MODULE_B2,
        MOD_ID_PROCESS_MODULE_C1,
        MOD_ID_PROCESS_MODULE_C2
    }

    public enum RecipeType
    {
        Process,
        Clean
    }
}
