using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Data;
using JMICSAPP.Hubs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MTC.JMICS.BL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;

namespace JMICSAPP.APIControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AAASSOSController : ControllerBase
    {
        public AppUser user;
        private IHubContext<PushHub, IPushHub> _hubContext;

        public AAASSOSController(IHubContext<PushHub, IPushHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST: api/AAASSOS
        [HttpPost]
        public IActionResult Post(AAASSOSRequest sosRequest)
        {
            try
            {
          
                AAASSOS SOSModel = sosRequest.Adapt<AAASSOS>();               
                using (AAASSOSService ASService = new AAASSOSService())
                {                    
                    ASService.Add(SOSModel,1, "AAAS User");                    
                }
               
                using (NotificationService notificationService = new NotificationService())
                {
                    Notifications notificationModel = new Notifications();
                    notificationModel.NotificationContent = "A SOS was reported by " + SOSModel.UserContactNumber;                    
                    notificationModel = notificationService.Add(notificationModel, 1, "AAAS User");
                    _hubContext.Clients.Group((notificationModel.SubscriberId).ToString()).PushNotification(notificationModel);                  
                }                              
            }
             
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
            }
            return Ok();            //status code 200
        }

        //GET: api/AAASSOS
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //GET: api/AAASSOS/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

      
        // PUT: api/AAASSOS/5
        //[HttpPut("{id}")]
        //public void Put(int id, AAASSOSRequest request)
        //{
        //}

        //// DELETE: api/AAASSOS/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
