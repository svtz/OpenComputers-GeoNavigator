using System;
using System.Collections.Generic;
using GeoNavigator.Hardness;
using GeoNavigator.Server;
using Microsoft.AspNetCore.Mvc;

namespace GeoNavigator.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static readonly HardnessRepository _hardnessRepository = new HardnessRepository();

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]HardnessData data)
        {
            var values = new List<PointValue>(data.HardnessValues.Length);

            using (var valuesEnumerator = ((IEnumerable<double>)data.HardnessValues).GetEnumerator())
            {
                const long chunkSize = 16;
                for (var x = data.StartX; x < data.StartX + 1; x += data.Step)
                for (var z = data.StartZ; z < data.StartZ + chunkSize; z += data.Step)
                for (var y = data.StartY; y > Math.Max(1, data.StartY - 64); y--)
                {
                    if (!valuesEnumerator.MoveNext())
                    {
                        Console.WriteLine("Error not enough hardness values");
                        return;
                    }

                    values.Add(new PointValue()
                    {
                        Hardness = valuesEnumerator.Current,
                        PosX = x,
                        PosZ = z,
                        PosY = y
                    });
                }

                if (valuesEnumerator.MoveNext())
                {
                    Console.WriteLine("Too many hardness values");
                    return;
                }

                _hardnessRepository.StoreHardness(values);
                Console.WriteLine("OK");
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class HardnessData
    {
        public long StartX { get; set; }
        public long StartY { get; set; }
        public long StartZ { get; set; }
        public long Step { get; set; }
        public double[] HardnessValues { get; set; }
    }
}
