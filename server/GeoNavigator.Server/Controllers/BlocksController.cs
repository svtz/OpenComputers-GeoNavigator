using System;
using System.Linq;
using System.Text;
using GeoNavigator.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoNavigator.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BlocksController : Controller
    {
        private readonly GeoContext _db;

        public BlocksController(GeoContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public string Find(Dimension dimension, Ore ore, int x, int y, int z, int limit)
        {
            var responseBuilder = new StringBuilder();
            
            var ores = _db
                .Blocks
                .Where(b => b.Dimension == dimension && b.Ore == ore)
                .OrderBy(b => (b.PosX - x) * (b.PosX - x) + (b.PosY - y) * (b.PosY - y) + (b.PosZ - z) * (b.PosZ - z))
                .Take(limit)
                .ToArray();

            responseBuilder.AppendLine($"Blocks of {ore} at {dimension}:");
            if (ores.Any())
            {
                foreach (var block in ores)
                {
                    responseBuilder.AppendLine($"x={block.PosX} z={block.PosZ} y={block.PosY}");
                }
            }
            else
            {
                responseBuilder.AppendLine("Not found");
            }

            var companions = _db
                .Blocks
                .Where(b => b.Dimension == dimension && _db
                                .Veins
                                .Where(v => v.Dimensions.Any(d => d.Dimension == dimension) &&
                                            v.Ores.Any(o => o.Ore == ore))
                                .SelectMany(v => v.Ores.Select(o => o.Ore))
                                .Where(o => o != ore)
                                .Contains(b.Ore))
                .OrderBy(b => (b.PosX - x) * (b.PosX - x) + (b.PosY - y) * (b.PosY - y) + (b.PosZ - z) * (b.PosZ - z))
                .Take(limit)
                .ToArray();
            responseBuilder.AppendLine($"Companions of {ore} at {dimension}:");
            if (companions.Any())
            {
                foreach (var block in companions)
                {
                    responseBuilder.AppendLine($"x={block.PosX} z={block.PosZ} y={block.PosY}");
                }
            }
            else
            {
                responseBuilder.AppendLine("Not found");
            }

            return responseBuilder.ToString();
        }

        [HttpPost]
        public void Add([FromBody]BlockDto data)
        {
            var existing = _db
                .Blocks
                .SingleOrDefault(b => b.Dimension == data.Dimension
                                      && data.PosX == b.PosX
                                      && data.PosY == b.PosY
                                      && data.PosZ == b.PosZ);

            var block = existing ?? new Block();
            block.Dimension = data.Dimension;
            block.Ore = data.Ore;
            block.PosX = data.PosX;
            block.PosY = data.PosY;
            block.PosZ = data.PosZ;
            if (block.Id == Guid.Empty)
                _db.Blocks.Add(block);
            _db.SaveChanges();
        }
    }

    public class BlockDto
    {
        public long PosX { get; set; }
        public long PosY { get; set; }
        public long PosZ { get; set; }
        
        public Ore Ore { get; set; }
        public Dimension Dimension { get; set; }
    }
}
