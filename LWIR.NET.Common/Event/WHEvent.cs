using AMEC.Native;
using LWIR.NET.Common.Entity;
using LWIR.NET.Framework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Common.Event
{
    public class HistoryFolderEvent : CompositeEvent<string>
    {
    }

    public class HistorySh2Event : CompositeEvent<SH2Info>
    {
    }

    public class HistoryWh3Event : CompositeEvent<WaferHistoryInfo>
    {
    }

    public class FileInfoExEvent : CompositeEvent<FileInfoEx>
    {

    }

    public class FilesCountEvent : CompositeEvent<FilesCount>
    {

    }

    public class SearchConditionsEvent : CompositeEvent<SearchConditions>
    {

    }

    public class Wd2FilesEvent : CompositeEvent<string[]>
    {
    }
}
