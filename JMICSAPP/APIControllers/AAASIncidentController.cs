using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Data;
using JMICSAPP.Hubs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MTC.JMICS.BL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;

namespace JMICSAPP.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AAASIncidentController : ControllerBase
    {
        private IHubContext<PushHub, IPushHub> _hubContext;
        public AAASIncidentController(IHubContext<PushHub, IPushHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST: api/AAASIncident
        [HttpPost]
        public IActionResult Post(AAASIncidentRequest incidentRequest)
        {
            try
            {
                AAASIncident AIModel = incidentRequest.Adapt<AAASIncident>();
                using (AAASIncidentService AIService = new AAASIncidentService())
                {
                    AIService.Add(AIModel, 1, "AAAS User");
                }
                using (NotificationService notificationService = new NotificationService())
                {
                    Notifications notificationModel = new Notifications();
                    notificationModel.NotificationContent = "An incident(" + AIModel.IncidentType + ") was reported by " + AIModel.UserContactNumber;
                    notificationModel = notificationService.Add(notificationModel,1, "AAAS User");
                    _hubContext.Clients.Group((notificationModel.SubscriberId).ToString()).PushNotification(notificationModel);
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
            }
            return Ok();
        }

        // GET: api/AAASIncident
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //// GET: api/AAASIncident/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT: api/AAASIncident/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
