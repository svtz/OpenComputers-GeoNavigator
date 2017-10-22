﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GeoNavigator.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
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
        public void Post([FromBody]PointValue[] value)
        {
            if (value != null)
                foreach (var point in value)
                {
                    Console.WriteLine($"{point.PosX}; {point.PosY}; {point.PosZ} == {point.Hardness}");
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

    public class PointValue
    {
        public long PosX { get; set; }
        public long PosZ { get; set; }
        public long PosY { get; set; }
            
        public double Hardness { get; set; }
    }

}