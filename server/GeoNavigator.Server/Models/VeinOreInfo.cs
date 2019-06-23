using System;

namespace GeoNavigator.Server.Models
{
    public class VeinOreInfo
    {
        public Guid Id { get; set; }
        public VeinInfo VeinInfo { get; set; }
        public Ore Ore { get; set; }
    }
}