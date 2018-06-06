using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Common.Entity
{
    public class SearchConditions
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String FoupId { get; set; }
        public String LotId { get; set; }
        public String WaferId { get; set; }
        public String RecipeName { get; set; }
        public String RecipeType { get; set; }
        public String ModuleId { get; set; }
    }
}
