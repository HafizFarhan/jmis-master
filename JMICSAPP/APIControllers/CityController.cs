using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MTC.JMICS.BL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;

namespace JMICSAPP.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        // GET: api/City
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/City/5
        // [HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/City
        [HttpPost]
        [DisableRequestSizeLimit]
        public void Post(List<CityRequest> cityReqList)
        {
            using (CityService cityService = new CityService())
            {
                foreach (var item in cityReqList)
                {
                    //City cityModel = new City();
                    cityService.Add(new City() { CityId = item.Id, CityName = item.Name, Country = item.Country, CityLat = item.Coord.Lat, CityLon = item.Coord.Lon });
                }
            }
        }




        // PUT: api/City/5
        // [HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        // [HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
