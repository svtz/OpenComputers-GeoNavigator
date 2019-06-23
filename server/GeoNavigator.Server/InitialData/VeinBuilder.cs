using System;
using System.Collections.Generic;
using System.Linq;
using GeoNavigator.Server.Models;

namespace GeoNavigator.Server.InitialData
{
    internal class VeinBuilder
    {
        private readonly int _yMin;
        private readonly int _yMax;
        private readonly List<Dimension> _dimensions = new List<Dimension>();
        private readonly List<Ore> _ores = new List<Ore>();
        
        public VeinBuilder(int yMin, int yMax)
        {
            if (yMin <= 0 || yMax <= 0 || yMin > yMax)
                throw new ArgumentException();
                
            _yMin = yMin;
            _yMax = yMax;
        }

        public VeinBuilder Dimensions(params Dimension[] dimensions)
        {
            if (dimensions.Distinct().Count() != dimensions.Length || dimensions.Length == 0)
                throw new ArgumentException();
            
            _dimensions.AddRange(dimensions);
            return this;
        }
        
        public VeinBuilder Ores(params Ore[] ores)
        {
            if (ores.Distinct().Count() != ores.Length || ores.Length == 0)
                throw new ArgumentException();
            
            _ores.AddRange(ores);
            return this;
        }

        public void SaveTo(GeoContext context)
        {
            if (!_dimensions.Any() || !_ores.Any())
                throw new InvalidOperationException();

            var oreNames = _ores
                .Select(o => o.ToString())
                .OrderBy(o => o);
            
            var info = new VeinInfo
            {
                Name = string.Join("/", oreNames),
                YMax = _yMax,
                YMin = _yMin
            };

            context.Veins.Add(info);

            foreach (var dimension in _dimensions)
            {
                var dimensionInfo = new VeinDimensionInfo
                {
                    Dimension = dimension,
                    VeinInfo = info
                };
                context.VeinDimensions.Add(dimensionInfo);
            }
            foreach (var ore in _ores)
            {
                var oreInfo = new VeinOreInfo
                {
                    Ore = ore,
                    VeinInfo = info
                };
                context.VeinOres.Add(oreInfo);
            }
        }

    }
}