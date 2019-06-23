using System;
using System.Collections.Generic;

namespace GeoNavigator.Server.Models
{
    public class VeinInfo
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public int YMin { get; set; }
        public int YMax { get; set; }
     
        public virtual ICollection<VeinDimensionInfo> Dimensions { get; set; }
        public virtual ICollection<VeinOreInfo> Ores { get; set; }
    }
}