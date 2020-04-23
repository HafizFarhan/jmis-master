using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Hubs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MTC.JMICS.BL;
using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;

namespace JMICSAPP.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AISTracksController : ControllerBase
    {
        private IHubContext<PushHub, IPushHub> _hubContext;

        public AISTracksController(IHubContext<PushHub, IPushHub> hubContext)
        {
            _hubContext = hubContext;
        }


        // GET: api/AISTracks
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AISTracks/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AISTracks
        [HttpPost]
        public void Post(AISTrackRequest aisTrackRequest)
        {
            using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
            {
                AISTrack model = aisTrackRequest.Adapt<AISTrack>();
                //aisTrackRepo.Insert<AISTrack>(model);
                //_hubContext.Clients.All.PushAISTrackNew(model);

                Ship shipModel = GetShipDetails(Convert.ToString(aisTrackRequest.TRACK_NUMBER), Convert.ToString(aisTrackRequest.IMO));
                if (shipModel!=null && shipModel.ShipId != 0)
                {
                    aisTrackRequest.IsLloydInfoPresent = true;
                    aisTrackRequest.LloydInfo = shipModel;
                }
                else
                    aisTrackRequest.IsLloydInfoPresent = false;
            }

            //Console.WriteLine(aisTrackRequest.IMO);
            //TrackHub trackHub = new TrackHub();
            //_ = trackHub.Send(aisTrackRequest.MMSI, aisTrackRequest.LAT, aisTrackRequest.LON, aisTrackRequest.SPEED, aisTrackRequest.HEADING, aisTrackRequest.COURSE, aisTrackRequest.STATUS, aisTrackRequest.ROT, aisTrackRequest.FLAG);
            _hubContext.Clients.All.PushAISTrack(aisTrackRequest); //aisTrackRequest.TRACK_TYPE, aisTrackRequest.TRACK_SOURCE);

        }

        public Ship GetShipDetails(string MMSI, string IMO)
        {
            Ship shipModel = new Ship();
            try
            {
                ShipPicture shipPicModel;
                using (ShipService shipService = new ShipService())
                {
                    shipModel = shipService.GetShipDetails(MMSI, IMO);
                    if (shipModel != null && shipModel.PhotoPresent == true)
                    {
                        if (shipModel.IMO != null)
                        {
                            using (ShipPictureService shipPicService = new ShipPictureService())
                            {
                                shipPicModel = shipPicService.GetShipPicByIMO(shipModel.IMO);
                                shipModel.PhotoPresent = false;
                                //if (shipPicModel != null)
                                //{
                                //    if (((shipPicModel.PictureName).ToString()).Length > 2)
                                //    {
                                //        string folderName = ((shipPicModel.PictureName).ToString()).Substring(0, ((shipPicModel.PictureName).ToString()).Length - 2);
                                //        string shipPicUrl = AppSettings.Configuration.GetSection("ProjectResources").GetSection("PhotoStoreUrl").Value;
                                //        string shipPicName1URL = shipPicUrl + "\\" + folderName + "\\" + shipPicModel.PictureName + "_1.jpg";
                                //        string shipPicName2URL = shipPicUrl + "\\" + folderName + "\\" + shipPicModel.PictureName + "_2.jpg";

                                //        if (System.IO.File.Exists(shipPicName2URL))
                                //            shipModel.PhotoContent = Common.GetBase64Content(shipPicName2URL);
                                //        else if (System.IO.File.Exists(shipPicName1URL))
                                //            shipModel.PhotoContent = Common.GetBase64Content(shipPicName1URL);
                                //        else
                                //            shipModel.PhotoPresent = false;

                                //        if (string.IsNullOrWhiteSpace(shipModel.PhotoContent))
                                //            shipModel.PhotoPresent = false;

                                //        //log message picture path not valid 
                                //    }
                                //    else
                                //        shipModel.PhotoPresent = false;
                                //    //log massage picture name not valid 
                                //}
                                //else
                                //    shipModel.PhotoPresent = false;

                                //log massage picture model is null
                            }
                        }
                        else
                        {
                            shipModel.PhotoPresent = false;
                        }
                        return shipModel;
                    }
                    else
                        return shipModel;
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return shipModel;
            }
        }

        // PUT: api/AISTracks/5
        [HttpPut("{mmsi}")]
        public void Put(int mmsi, AISTrackRequest aisTrackRequest)
        {
            try
            {
                using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                {
                    AISTrack model = aisTrackRequest.Adapt<AISTrack>();
                    //aisTrackRepo.Insert<AISTrack>(model);
                    _hubContext.Clients.All.PushAISTrackUpdate(aisTrackRequest);
                }
            }
            catch (Exception ex)
            {
            
            }
        }

        // DELETE: api/AISTracks/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
