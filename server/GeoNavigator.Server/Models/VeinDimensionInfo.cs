using System;

namespace GeoNavigator.Server.Models
{
    public class VeinDimensionInfo
    {
        public Guid Id { get; set; }
        public VeinInfo VeinInfo { get; set; }
        public Dimension Dimension { get; set; }
    }
}