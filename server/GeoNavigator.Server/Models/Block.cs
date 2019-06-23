using System;

namespace GeoNavigator.Server.Models
{
    public class Block
    {
        public Guid Id { get; set; }
        
        public long PosX { get; set; }
        public long PosZ { get; set; }
        public long PosY { get; set; }
            
        public Dimension Dimension { get; set; }
        
        public Ore Ore { get; set; }
    }
}