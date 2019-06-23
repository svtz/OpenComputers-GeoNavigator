using System;
using System.Linq;
using GeoNavigator.Server.Models;

namespace GeoNavigator.Server.InitialData
{
    public class VeinsConfiguration
    {
        private readonly GeoContext _db;

        public VeinsConfiguration(GeoContext db)
        {
            _db = db ?? throw new ArgumentNullException();
        }

        public void Initialize()
        {
            ClearInfos();
            
            new VeinBuilder(40, 60)
                .Dimensions(Dimension.Overworld)
                .Ores(Ore.Apatite, Ore.Phosphorus, Ore.Phosphate)
                .SaveTo(_db);
            
            new VeinBuilder(50, 90)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Bauxite, Ore.Aluminium, Ore.Ilmenite)
                .SaveTo(_db);
            
            new VeinBuilder(5, 30)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Beryllium, Ore.Emerald, Ore.Thorium)
                .SaveTo(_db);
            
            new VeinBuilder(40, 120)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Tin, Ore.Cassiterite)
                .SaveTo(_db);
            
            new VeinBuilder(50, 80)
                .Dimensions(Dimension.Overworld)
                .Ores(Ore.Coal, Ore.Lignite)
                .SaveTo(_db);
            
            new VeinBuilder(10, 30)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.Moon, Dimension.Mars)
                .Ores(Ore.Chalcopyrite, Ore.Iron, Ore.Pyrite, Ore.Copper)
                .SaveTo(_db);

            new VeinBuilder(5, 20)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Graphite, Ore.Diamond, Ore.Coal)
                .SaveTo(_db);

            new VeinBuilder(30, 60)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Galena, Ore.Silver, Ore.Lead)
                .SaveTo(_db);

            new VeinBuilder(60, 80)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Magnetite, Ore.VanadiumMagnetite, Ore.Gold)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.Moon, Dimension.Mars)
                .Ores(Ore.BrownLimonite, Ore.YellowLimonite, Ore.BandedIron, Ore.Malachite)
                .SaveTo(_db);

            new VeinBuilder(20, 50)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Lazurite, Ore.Sodalite, Ore.Lapis, Ore.Calcite)
                .SaveTo(_db);

            new VeinBuilder(50, 130)
                .Dimensions(Dimension.Overworld)
                .Ores(Ore.Lignite, Ore.Coal)
                .SaveTo(_db);

            new VeinBuilder(50, 120)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.Moon, Dimension.Mars)
                .Ores(Ore.Magnetite, Ore.Iron, Ore.VanadiumMagnetite)
                .SaveTo(_db);

            new VeinBuilder(20, 30)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Asteroids)
                .Ores(Ore.Grossular, Ore.Spessartine, Ore.Pyrolusite, Ore.Tantalite, Ore.Manganese)
                .SaveTo(_db);

            new VeinBuilder(20, 50)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Wulfenite, Ore.Molybdenite, Ore.Molybdenum, Ore.Powellite)
                .SaveTo(_db);

            new VeinBuilder(20, 40)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Bastnasite, Ore.Monazite, Ore.Neodymium)
                .SaveTo(_db);

            new VeinBuilder(60, 120)
                .Dimensions(Dimension.Asteroids)
                .Ores(Ore.Naquadah, Ore.EnrichedNaquadah)
                .SaveTo(_db);

            new VeinBuilder(40, 80)
                .Dimensions(Dimension.Nether)
                .Ores(Ore.NetherQuartz)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Garnierite, Ore.Nickel, Ore.Cobaltite, Ore.Pentlandite)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Bentonite, Ore.Magnesite, Ore.Olivine, Ore.Glauconite)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Pitchblende, Ore.Uraninite, Ore.Uranium238)
                .SaveTo(_db);

            new VeinBuilder(40, 50)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Sheldonite, Ore.Palladium, Ore.Platinum, Ore.Iridium)
                .SaveTo(_db);

            new VeinBuilder(20, 30)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Uraninite, Ore.Uranium238)
                .SaveTo(_db);

            new VeinBuilder(40, 80)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Quartzite, Ore.Barite, Ore.CertusQuartz, Ore.Quartz)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Redstone, Ore.Ruby, Ore.Cinnabar)
                .SaveTo(_db);

            new VeinBuilder(50, 60)
                .Dimensions(Dimension.Overworld, Dimension.Moon)
                .Ores(Ore.RockSalt, Ore.Salt, Ore.Lepidolite, Ore.Spodumene)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Almandine, Ore.Pyrope, Ore.Sapphire, Ore.GreenSapphire)
                .SaveTo(_db);

            new VeinBuilder(10, 40)
                .Dimensions(Dimension.Overworld, Dimension.Moon, Dimension.Mars)
                .Ores(Ore.Soapstone, Ore.Talc, Ore.Glauconite, Ore.Pentlandite)
                .SaveTo(_db);

            new VeinBuilder(5, 20)
                .Dimensions(Dimension.Nether, Dimension.Mars)
                .Ores(Ore.Sulflur, Ore.Pyrite, Ore.Sphalerite)
                .SaveTo(_db);

            new VeinBuilder(60, 120)
                .Dimensions(Dimension.Overworld, Dimension.Nether, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Tetrahedrite, Ore.Copper, Ore.Stibnite)
                .SaveTo(_db);

            new VeinBuilder(20, 50)
                .Dimensions(Dimension.Overworld, Dimension.End, Dimension.Moon, Dimension.Mars, Dimension.Asteroids)
                .Ores(Ore.Scheelite, Ore.Tungstate, Ore.Lithium)
                .SaveTo(_db);

            new VeinBuilder(50, 80)
                .Dimensions(Dimension.Overworld)
                .Ores(Ore.OilSands)
                .SaveTo(_db);

            _db.SaveChanges();
        }

        private void ClearInfos()
        {
            var veinInfos = _db.Veins.ToArray();
            _db.Veins.RemoveRange(veinInfos);
        }
    }
}