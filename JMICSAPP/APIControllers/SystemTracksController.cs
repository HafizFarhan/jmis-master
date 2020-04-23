using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Hubs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;

namespace JMICSAPP.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemTracksController : ControllerBase
    {
        private IHubContext<PushHub, IPushHub> _hubContext;

        public SystemTracksController(IHubContext<PushHub, IPushHub> hubContext)
        {
            _hubContext = hubContext;
        }


        // GET: api/SystemTracks
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SystemTracks/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SystemTracks
        [HttpPost]
        public void Post(AISTrackRequest aisTrackRequest)
        {
            using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
            {
                AISTrack model = aisTrackRequest.Adapt<AISTrack>();
                //aisTrackRepo.Insert<AISTrack>(model);
                //_hubContext.Clients.All.PushAISTrackNew(model);
            }
            //Console.WriteLine(aisTrackRequest.IMO);
            //TrackHub trackHub = new TrackHub();
            //_ = trackHub.Send(aisTrackRequest.MMSI, aisTrackRequest.LAT, aisTrackRequest.LON, aisTrackRequest.SPEED, aisTrackRequest.HEADING, aisTrackRequest.COURSE, aisTrackRequest.STATUS, aisTrackRequest.ROT, aisTrackRequest.FLAG);
            _hubContext.Clients.All.PushAISTrack(aisTrackRequest); //aisTrackRequest.TRACK_TYPE, aisTrackRequest.TRACK_SOURCE);
        }

        // PUT: api/SystemTracks/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/SystemTracks/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
    