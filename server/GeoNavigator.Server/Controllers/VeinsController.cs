using System.Linq;
using System.Text;
using GeoNavigator.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoNavigator.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VeinsController : Controller
    {
        private readonly GeoContext _db;

        public VeinsController(GeoContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public string Info(Dimension dimension, Ore ore)
        {
            var responseBuilder = new StringBuilder();

            var veins = _db
                .Veins
                .Where(v => v.Dimensions.Any(d => d.Dimension == dimension) &&
                            v.Ores.Any(o => o.Ore == ore))
                .ToArray();
            if (veins.Any())
            {
                foreach (var vein in veins)
                {
                    responseBuilder.AppendLine($"{vein.Name} height: [{vein.YMin}, {vein.YMax}]");
                }
            }
            else
            {
                responseBuilder.AppendLine("Not found");
            }

            return responseBuilder.ToString();
        }
    }
}
