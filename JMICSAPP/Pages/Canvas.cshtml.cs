using JMICSAPP.Data;
using JMICSAPP.Hubs;
using JMICSAPP.Models;
using JMICSAPP.Properties;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MTC.JMICS.BL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Responses;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Xml;
using System.Text;

namespace JMICSAPP.Pages
{
    [Authorize]
    public class CanvasModel : PageModel
    {
        #region "Getters and Setters"

        //public Subscriber subModel = new Subscriber();
        //public COIStatus coiStatusModel = new COIStatus();
        //public UserType userTypeModel = new UserType();
        //public NewsFeedType newsFeedTypeModel = new NewsFeedType();
        //public AppUserViewModel userModel = new AppUserViewModel();
        //public NatureOfThreat threatModel = new NatureOfThreat();
        //public COIType coiTypeModel = new COIType();
        //public InfoConfidenceLevel infoConLevelModel = new InfoConfidenceLevel();
        //public int subsCityId = 0;
        //public List<COIType> coiTypeList = new List<COIType>();
        //public List<RadUnit> RadUnitList = new List<RadUnit>();
        //public List<Drawing> DrawingList = new List<Drawing>();
        //public List<COIView> COIModelList = new List<COIView>();
        //public WeatherDetail weatherModel = new WeatherDetail();
        //public List<double> drCoordinateList2 = new List<double>();
        //public List<NewsView> NewsModelList = new List<NewsView>();
        //public List<COIView> ApprovedCOIsList = new List<COIView>();
        //public List<NewsView> ApprovedNewsList = new List<NewsView>();
        //public List<Subscriber> stakeholderList = new List<Subscriber>();
        //public List<DrawingShape> drShapeList = new List<DrawingShape>();
        //public List<AspNetUserView> userList = new List<AspNetUserView>();
        //public List<EventView> eventViewModelList = new List<EventView>();
        //public List<Subscriber> SubscriberModelList = new List<Subscriber>();
        //public List<NewsFeedType> newsFeedTypeList = new List<NewsFeedType>();
        //public List<NewsFeedView> newsFeedViewList = new List<NewsFeedView>();
        //public List<PRSRViewModel> lstPRSRViewModel = new List<PRSRViewModel>();
        //public List<NatureOfThreat> natureOfThreatList = new List<NatureOfThreat>();
        //public List<DrawingCoordinate> drCoordinateList = new List<DrawingCoordinate>();
        //public List<LostContactReportView> lostContactReportList = new List<LostContactReportView>();
        //public List<NotificationView> notificationList = new List<NotificationView>();
        //public List<COIView> COIViewList = new List<COIView>();
        //public int selectedSubsId { get; set; }
        //public DropReportView dropView { get; set; }
        //public NewsFeedView newsFeedView { get; set; }
        //public SubsequentReportView SRView { get; set; }
        //public PreliminaryReportView PRView { get; set; }
        //public List<DropReportView> dropReportList { get; set; }
        //public List<LostContactReportView> LRViewList { get; set; }
        //public List<SubsequentReportView> SRViewList { get; set; }
        //public List<PreliminaryReportView> PRViewList { get; set; }
        //public List<AmplifyingReportView> ARViewList { get; set; }
        //public List<AfterActionReportView> AARViewList { get; set; }
        //public COIView COIViewModel = new COIView();
        //
        //public COIStatus coiStatusModel = new COIStatus();
        //public UserType userTypeModel = new UserType();
        //public NewsFeedType newsFeedTypeModel = new NewsFeedType();
        //public AppUserViewModel userModel = new AppUserViewModel();
        //public NewsView NewsViewModel = new NewsView();
        //public DrawingView DrawingViewModel = new DrawingView();
        //public SubscriberCOI subsCOIModel = new SubscriberCOI();

        public Subscriber CurrentSubscriber
        {
            get
            {
                if (MemCache.IsIncache(User.Identity.Name + "_Subscriber"))
                {
                    return MemCache.GetFromCache<Subscriber>(User.Identity.Name + "_Subscriber");
                }
                else
                {
                    return GetCurrentSubscriber();
                }
            }
        }

        public AppUser user;
        private readonly MemCache _memCache;
        private readonly ILogger<CanvasModel> _logger;
        private readonly UserManager<AppUser> _userManager;
        private IHubContext<PushHub, IPushHub> _hubContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CanvasModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<CanvasModel> logger, IHubContext<PushHub, IPushHub> hubContext, IMemoryCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _hubContext = hubContext;
            _memCache = new MemCache(cache);

        }
        #endregion

        #region "Functions"
        public Subscriber GetCurrentSubscriber()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            using (SubscriberService subService = new SubscriberService())
            {
                Subscriber currentSubscriber = subService.GetById(user.Result.Subscriber_Id.Value);
                MemCache.AddToCache(User.Identity.Name + "_Subscriber", currentSubscriber);
                return currentSubscriber;
            }
        }

        public bool ValidateFeed(string SourceURL)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(SourceURL);
            XmlNode node = xml.SelectSingleNode("//item");
            XmlNode title = node.SelectSingleNode("title");
            XmlNode description = node.SelectSingleNode("link");
            if (node != null && title != null && description != null)
                return true;
            else
                return false;

        }

        private WeatherDetail UpdateWeather(int CityId)
        {
            try
            {
                RestClient restClient = new RestClient(Resources.OpenWeatherBaseURL);
                RestRequest trackRequest = new RestRequest("/data/2.5/weather?id=" + CityId + "&appid=" + Common.WeatherAPIKey, Method.GET);
                trackRequest.RequestFormat = DataFormat.Json;
                IRestResponse<WeatherResponse> response = restClient.Execute<WeatherResponse>(trackRequest);
                WeatherDetail weatherModel = new WeatherDetail();
                using (WeatherDetailService wdService = new WeatherDetailService())
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        weatherModel = wdService.Add(CurrentSubscriber.SubscriberId, User.Identity.Name, weatherModel, response);

                    else
                        weatherModel = wdService.GetLastUpdatedDetails(CityId);
                }
                return weatherModel;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }

        private WeatherDetail WeatherModelConversions(WeatherDetail WeatherModel)
        {
            //string dtString, sunrise, sunset;
            //dtString = sunrise = sunset = "";
            //if (DateTime.TryParse(Convert.ToString(WeatherModel.Dt), out DateTime dtRecordOn))
            //{
            //    dtString = dtRecordOn.ToString("hh:mm tt");
            //}
            //if (DateTime.TryParse(Convert.ToString(WeatherModel.Sunrise), out DateTime dtSunrise))
            //{
            //    sunrise = dtSunrise.ToString("hh:mm tt");
            //}
            //if (DateTime.TryParse(Convert.ToString(WeatherModel.Sunset), out DateTime dtSunset))
            //{
            //    WeatherModel.Sunset = dtSunset.ToString("hh:mm tt");
            //    //sunset = dtSunset.ToString("hh:mm tt");
            //}
            //wind speed m/s to km/h
            //decimal? windSpeed = 0;
            if (decimal.TryParse(WeatherModel.Speed.ToString(), out decimal speedInDecimal))
            {
                WeatherModel.Speed = (WeatherModel.Speed * 3600) / 1000;
            }
            WeatherModel.Deg = Math.Truncate(WeatherModel.Deg ?? 0);
            WeatherModel.Temp = Math.Truncate(WeatherModel.Temp ?? 0);
            WeatherModel.TempMin = Math.Truncate(WeatherModel.TempMin ?? 0);
            WeatherModel.TempMax = Math.Truncate(WeatherModel.TempMax ?? 0);
            WeatherModel.Pressure = Math.Truncate(WeatherModel.Pressure ?? 0);
            WeatherModel.Humidity = Math.Truncate(WeatherModel.Humidity ?? 0);
            return WeatherModel;
        }

        #region "Event Logging Code"
        //public void UserEventLogging(string userName, string eventDesc, int eventTypeId)
        //{
        //    try
        //    {
        //        using (EventService eventService = new EventService())
        //        {
        //            Event eventModel = new Event();
        //            eventModel.EventTypeId = eventTypeId;
        //            eventModel.SubscriberId = CurrentSubscriber.SubscriberId;
        //            eventModel.EventDescription = eventDesc;
        //            eventModel.CreatedOn = DateTime.Now;
        //            eventModel.CreatedBy = userName;
        //            eventService.Add(eventModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Logging User Events " + Environment.NewLine + ex.Message);
        //    }
        //}

        //private void ShowUserEvents()
        //{
        //    try
        //    {
        //        using (EventService eventService = new EventService())
        //        {
        //            Dictionary<string, string> dicFilters = null;

        //            dicFilters = new Dictionary<string, string>();
        //            dicFilters.Add("orderby", "Event_Id");
        //            dicFilters.Add("offset", "1");
        //            dicFilters.Add("limit", "200");

        //            eventViewModelList = eventService.GetBySubsId(user.Subscriber_Id, dicFilters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Events Data " + Environment.NewLine + ex.Message);
        //    }
        //} 
        #endregion
        #endregion

        #region "Event Handlers"
        public IActionResult OnGetAllStakeholders()
        {
            try
            {
                using (SubscriberService subscriberService = new SubscriberService())
                {
                    return new JsonResult(subscriberService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetPR(int Id)
        {
            try
            {
                PreliminaryReport prModel = new PreliminaryReport();
                if (Id > 0)
                {
                    using (PreliminaryReportService preliminaryReportService = new PreliminaryReportService())
                    {
                        prModel = preliminaryReportService.GetById(Id);
                    }
                }
                else
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    prModel.SubscriberId = subsId;
                    prModel.ActionAddressee = "1";
                    prModel.LastObservationDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
                }
                return new PartialViewResult
                {
                    ViewName = "PreliminaryReportModal",
                    ViewData = new ViewDataDictionary<PreliminaryReport>(ViewData, prModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedPRs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (PreliminaryReportService PRService = new PreliminaryReportService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(PRService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPRs()
        {
            try
            {
                using (PreliminaryReportService PRService = new PreliminaryReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(PRService.List());
                    else
                        return new JsonResult(PRService.GetAllPRsBySubsId(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostPR(PreliminaryReport PR)
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (PreliminaryReportService PRService = new PreliminaryReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subsModel.SubscriberId);
                        lstSubscribers.AddRange(PR.ActionAddresseeArray);
                        PreliminaryReportView PRView = PRService.AddPR(subsModel, User.Identity.Name, PR);

                        foreach (int subscriberId in lstSubscribers.Distinct())
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                Notifications notificationModel = new Notifications();
                                notificationModel.NotificationContent = PRView.PRNumber + " of Type " + PRView.COITypeName + " with Threat Level: " + PRView.ThreatName + " is generated by " + PRView.SubscriberCode;
                                notificationModel.NotificationType = (NotificationTypes.PR).ToString();
                                notificationModel.ReportId = PRView.Id;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);

                                _hubContext.Clients.Group((subscriberId).ToString()).PushPR(PRView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetSR(int PRId = 0, int Id = 0)
        {
            try
            {
                SubsequentReport srModel = new SubsequentReport();
                if (Id > 0)
                {
                    using (SubsequentReportService SRService = new SubsequentReportService())
                    {
                        srModel = SRService.GetById(Id);
                    }
                }
                else
                {
                    if (PRId > 0)
                    {
                        using (PreliminaryReportService preliminaryReportService = new PreliminaryReportService())
                        {
                            PreliminaryReport prReport = preliminaryReportService.GetById(PRId);
                            srModel = prReport.Adapt<SubsequentReport>();
                        }
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "SubsequentReportModal",
                    ViewData = new ViewDataDictionary<SubsequentReport>(ViewData, srModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedSRs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (SubsequentReportService SRService = new SubsequentReportService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(SRService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllSRs()
        {
            try
            {
                using (SubsequentReportService SRService = new SubsequentReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(SRService.List());
                    else
                        return new JsonResult(SRService.GetAllSRsBySubsId(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostSR(SubsequentReport SR)
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (SubsequentReportService SRService = new SubsequentReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subsModel.SubscriberId);
                        lstSubscribers.AddRange(SR.ActionAddresseeArray);
                        SubsequentReportView SRView = SRService.Add(subsModel, User.Identity.Name, SR);

                        foreach (int subscriberId in lstSubscribers.Distinct())
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                Notifications notificationModel = new Notifications();

                                notificationModel.NotificationContent = SRView.SRNumber + " against " + SRView.PRNumber + " of Type " + SRView.COITypeName + " with Threat Level: " + SRView.ThreatName + " is generated by " + SRView.SubscriberCode;
                                notificationModel.NotificationType = (NotificationTypes.SR).ToString();
                                notificationModel.ReportId = SRView.Id;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushSR(SRView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetCOI(int PRId = 0, int SRId = 0, int Id = 0)
        {
            try
            {
                COI coiModel = new COI();
                if (Id > 0)
                {
                    using (COIService COIService = new COIService())
                    {
                        coiModel = COIService.GetById(Id);
                    }
                }
                else if (PRId > 0)
                {
                    using (PreliminaryReportService preliminaryReportService = new PreliminaryReportService())
                    {
                        PreliminaryReport prReport = preliminaryReportService.GetById(PRId);
                        coiModel = prReport.Adapt<COI>();
                    }
                }
                else if (SRId > 0)
                {
                    using (SubsequentReportService SRService = new SubsequentReportService())
                    {
                        SubsequentReport srReport = SRService.GetById(SRId);
                        coiModel = srReport.Adapt<COI>();
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "COIActivationReportModal",
                    ViewData = new ViewDataDictionary<COI>(ViewData, coiModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedCOIs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (COIService coiService = new COIService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(coiService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllCOIs()
        {
            try
            {
                using (COIService coiService = new COIService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(coiService.List());
                    else
                        return new JsonResult(coiService.GetSubsCOIs(CurrentSubscriber));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostCOI(COI COI)
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    Notifications notificationModel = new Notifications();
                    //string generatedCOINo = subsModel.SubscriberCode + "-COI-" + 1;
                    using (COIService COIService = new COIService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subsModel.SubscriberId);
                        lstSubscribers.AddRange(COI.ActionAddresseeArray);
                        lstSubscribers.AddRange(COI.InformationAddresseeArray);
                        COIView coiView = COIService.Add(subsModel, User.Identity.Name, COI);


                        using (PreliminaryReportService PRService = new PreliminaryReportService())
                        {
                            PreliminaryReport PRModel = PRService.GetById(COI.PRId);
                            if (PRModel != null)
                            {
                                PRModel.COIId = coiView.COIId;
                                PRModel.Active = false;
                                PRService.Update(subsModel.SubscriberId, User.Identity.Name, PRModel);
                                _hubContext.Clients.All.DeletePR(PRModel.PRNumber);
                            }
                        }

                        using (SubsequentReportService SRService = new SubsequentReportService())
                        {
                            //get SR Model by PRId FK
                            List<SubsequentReportView> lstSRModel = SRService.GetByPRId(COI.PRId);

                            foreach (var item in lstSRModel)
                            {
                                if (item != null)
                                {
                                    item.COIId = coiView.COIId;
                                    item.Active = false;
                                    SRService.Update(subsModel.SubscriberId, User.Identity.Name, item);
                                    _hubContext.Clients.All.DeleteSR(item.SRNumber);

                                }
                            }
                        }
                        foreach (int subscriberId in lstSubscribers.Distinct())
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                notificationModel.NotificationContent = coiView.COINumber + " of Type " + coiView.COITypeName + " with Threat Level: " + coiView.ThreatName + " is generated by " + coiView.SubscriberCode;
                                notificationModel.NotificationType = (NotificationTypes.COI).ToString();
                                notificationModel.ReportId = coiView.COIId;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushCOIs(coiView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetAR(int COIId = 0, int Id = 0)
        {
            try
            {
                AmplifyingReport arModel = new AmplifyingReport();
                if (Id > 0)
                {
                    using (AmplifyingReportService ARService = new AmplifyingReportService())
                    {
                        arModel = ARService.GetById(Id);
                    }
                }
                else
                {
                    if (COIId > 0)
                    {
                        using (COIService COIService = new COIService())
                        {
                            COI coiModel = COIService.GetById(COIId);
                            arModel = coiModel.Adapt<AmplifyingReport>();
                        }
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "AmplifyingReportModal",
                    ViewData = new ViewDataDictionary<AmplifyingReport>(ViewData, arModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedARs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (AmplifyingReportService ARService = new AmplifyingReportService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(ARService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllARs()
        {
            try
            {
                using (AmplifyingReportService ARService = new AmplifyingReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(ARService.List());
                    else
                        return new JsonResult(ARService.GetSubsARs(CurrentSubscriber));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostAR(AmplifyingReport AR)
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (AmplifyingReportService ARService = new AmplifyingReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subsModel.SubscriberId);
                        lstSubscribers.AddRange(AR.ActionAddresseeArray);
                        lstSubscribers.AddRange(AR.InformationAddresseeArray);
                        AmplifyingReportView arView = ARService.Add(subsModel, User.Identity.Name, AR);

                        foreach (int subscriberId in lstSubscribers.Distinct())
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                Notifications notificationModel = new Notifications();

                                notificationModel.NotificationContent = arView.ARNumber + " against " + arView.COINumber + " is generated by " + arView.SubscriberCode;
                                notificationModel.NotificationType = (NotificationTypes.AR).ToString();
                                notificationModel.ReportId = arView.ARId;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushAR(arView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetLR(int PRId = 0, int COIId = 0, int Id = 0)
        {
            try
            {
                LostContactReport lrModel = new LostContactReport();
                if (Id > 0)
                {
                    using (LostContactReportService lrService = new LostContactReportService())
                    {
                        lrModel = lrService.GetById(Id);
                    }
                }
                else if (PRId > 0)
                {
                    using (PreliminaryReportService preliminaryReportService = new PreliminaryReportService())
                    {
                        PreliminaryReport prReport = preliminaryReportService.GetById(PRId);
                        lrModel = prReport.Adapt<LostContactReport>();
                    }
                }
                else if (COIId > 0)
                {
                    using (COIService COIService = new COIService())
                    {
                        COI COIReport = COIService.GetById(COIId);
                        lrModel = COIReport.Adapt<LostContactReport>();
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "LostReportModal",
                    ViewData = new ViewDataDictionary<LostContactReport>(ViewData, lrModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedLRs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (LostContactReportService LRService = new LostContactReportService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(LRService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllLRs()
        {
            try
            {
                using (LostContactReportService LRService = new LostContactReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(LRService.List());
                    else
                        return new JsonResult(LRService.GetAllLRsBySubsId(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostLR(LostContactReport LR)
        {
            try
            {
                Subscriber subscriberModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (LostContactReportService LRService = new LostContactReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subscriberModel.SubscriberId);
                        if (LR.COIId > 0)
                            lstSubscribers.AddRange(LR.ActionAddresseeArray);

                        LostContactReportView lrView = LRService.Add(subscriberModel, User.Identity.Name, LR);

                        using (NotificationService notificationService = new NotificationService())
                        {
                            Notifications notificationModel = new Notifications();
                            if (LR.COIId > 0)
                            {
                                using (COIService coiService = new COIService())
                                {
                                    COI coiModel = coiService.GetById(LR.COIId);
                                    if (coiModel != null)
                                    {
                                        coiModel.IsLost = true;
                                        coiModel.Active = false;
                                        coiService.Update(subscriberModel.SubscriberId, User.Identity.Name, coiModel);
                                        _hubContext.Clients.All.DeleteCOI(coiModel.COINumber);
                                    }
                                }
                            }

                            using (AmplifyingReportService arService = new AmplifyingReportService())
                            {
                                //get AR Model by COIId FK
                                List<AmplifyingReportView> lstARModel = arService.GetByCOIId(LR.COIId);

                                foreach (var item in lstARModel)
                                {
                                    if (item != null)
                                    {
                                        item.IsLost = true;
                                        item.Active = false;
                                        arService.Update(subscriberModel.SubscriberId, User.Identity.Name, item);
                                        _hubContext.Clients.All.DeleteAR(item.ARNumber);
                                    }
                                }
                            }

                            using (PreliminaryReportService PRService = new PreliminaryReportService())
                            {
                                PreliminaryReport PRModel = PRService.GetById(LR.PRId);
                                if (PRModel != null)
                                {
                                    PRModel.IsLost = true;
                                    PRModel.Active = false;
                                    PRService.Update(subscriberModel.SubscriberId, User.Identity.Name, PRModel);
                                    _hubContext.Clients.All.DeletePR(PRModel.PRNumber);
                                }
                            }

                            using (SubsequentReportService SRService = new SubsequentReportService())
                            {
                                //get SR Model by PRId FK
                                List<SubsequentReportView> lstSRModel = SRService.GetByPRId(LR.PRId);

                                foreach (var item in lstSRModel)
                                {
                                    if (item != null)
                                    {
                                        item.IsLost = true;
                                        item.Active = false;
                                        SRService.Update(subscriberModel.SubscriberId, User.Identity.Name, item);
                                        _hubContext.Clients.All.DeleteSR(item.SRNumber);
                                    }
                                }
                            }

                            foreach (int subscriberId in lstSubscribers.Distinct())
                            {
                                //using (NotificationService notificationService = new NotificationService())
                                //{
                                //    Notifications notificationModel = new Notifications();

                                if (LR.COIId > 0)
                                    notificationModel.NotificationContent = lrView.COINumber + " against " + lrView.PRNumber + " is lost by " + lrView.SubscriberCode;

                                else
                                    notificationModel.NotificationContent = lrView.PRNumber + " is lost by " + lrView.SubscriberCode;

                                notificationModel.NotificationType = (NotificationTypes.LR).ToString();
                                notificationModel.ReportId = lrView.Id;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushLR(lrView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetDR(int PRId = 0, int COIId = 0, int Id = 0)
        {
            try
            {
                DropInfoSharingReport drModel = new DropInfoSharingReport();
                if (Id > 0)
                {
                    using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                    {
                        drModel = drService.GetById(Id);
                    }
                }
                else if (PRId > 0)
                {
                    using (PreliminaryReportService preliminaryReportService = new PreliminaryReportService())
                    {
                        PreliminaryReport prReport = preliminaryReportService.GetById(PRId);
                        drModel = prReport.Adapt<DropInfoSharingReport>();
                    }
                }
                else if (COIId > 0)
                {
                    using (COIService COIService = new COIService())
                    {
                        COI COIReport = COIService.GetById(COIId);
                        drModel = COIReport.Adapt<DropInfoSharingReport>();
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "DropReportModal",
                    ViewData = new ViewDataDictionary<DropInfoSharingReport>(ViewData, drModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedDRs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                {
                    if (!User.IsInRole("ADMIN"))
                        queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(drService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllDRs()
        {
            try
            {
                using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return new JsonResult(drService.List());
                    else
                        return new JsonResult(drService.GetAllDRsBySubsId(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostDR(DropInfoSharingReport DR)
        {
            try
            {
                Subscriber subscriberModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subscriberModel.SubscriberId);
                        if (DR.COIId > 0)
                            lstSubscribers.AddRange(DR.ActionAddresseeArray);
                        DropReportView drView = drService.Add(subscriberModel, User.Identity.Name, DR);

                        using (NotificationService notificationService = new NotificationService())
                        {
                            Notifications notificationModel = new Notifications();

                            using (PreliminaryReportService PRService = new PreliminaryReportService())
                            {
                                PreliminaryReport PRModel = PRService.GetById(DR.PRId);
                                if (PRModel != null)
                                {
                                    PRModel.IsDropped = true;
                                    PRModel.Active = false;
                                    PRService.Update(subscriberModel.SubscriberId, User.Identity.Name, PRModel);
                                    _hubContext.Clients.All.DeletePR(PRModel.PRNumber);
                                }
                            }

                            using (SubsequentReportService SRService = new SubsequentReportService())
                            {
                                //get SR Model by PRId FK
                                List<SubsequentReportView> lstSRModel = SRService.GetByPRId(DR.PRId);

                                foreach (var item in lstSRModel)
                                {
                                    if (item != null)
                                    {
                                        item.IsDropped = true;
                                        item.Active = false;
                                        SRService.Update(subscriberModel.SubscriberId, User.Identity.Name, item);
                                        _hubContext.Clients.All.DeleteSR(item.SRNumber);
                                    }
                                }
                            }

                            if (DR.COIId > 0)
                            {
                                using (COIService coiService = new COIService())
                                {
                                    COI coiModel = coiService.GetById(DR.COIId);
                                    if (coiModel != null)
                                    {
                                        coiModel.IsDropped = true;
                                        coiModel.Active = false;
                                        coiService.Update(subscriberModel.SubscriberId, User.Identity.Name, coiModel);
                                        _hubContext.Clients.All.DeleteCOI(coiModel.COINumber);
                                    }
                                }
                            }

                            using (AmplifyingReportService arService = new AmplifyingReportService())
                            {
                                //get AR Model by COIId FK
                                List<AmplifyingReportView> lstARModel = arService.GetByCOIId(DR.COIId);

                                foreach (var item in lstARModel)
                                {
                                    if (item != null)
                                    {
                                        item.IsDropped = true;
                                        item.Active = false;
                                        arService.Update(subscriberModel.SubscriberId, User.Identity.Name, item);
                                        _hubContext.Clients.All.DeleteAR(item.ARNumber);
                                    }
                                }
                            }

                            foreach (int subscriberId in lstSubscribers.Distinct())
                            {
                                //using (NotificationService notificationService = new NotificationService())
                                //{
                                //    Notifications notificationModel = new Notifications();

                                if (DR.COIId > 0)
                                    notificationModel.NotificationContent = drView.COINumber + " against " + drView.PRNumber + " is dropped by " + drView.SubscriberCode;
                                else
                                    notificationModel.NotificationContent = drView.PRNumber + " is dropped by " + drView.SubscriberCode;

                                notificationModel.NotificationType = (NotificationTypes.DR).ToString();
                                notificationModel.ReportId = drView.Id;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushDR(drView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteDR(int DRId)
        {
            try
            {
                using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                {
                    if (DRId > 0)
                    {
                        drService.Delete(DRId);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetAAR(int DRId = 0, int Id = 0)
        {
            try
            {
                AfterActionReport aarModel = new AfterActionReport();
                if (Id > 0)
                {
                    using (AfterActionReportService AARService = new AfterActionReportService())
                    {
                        aarModel = AARService.GetById(Id);
                    }
                }
                else
                {
                    if (DRId > 0)
                    {
                        using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                        {
                            DropInfoSharingReport DRModel = drService.GetById(DRId);
                            aarModel = DRModel.Adapt<AfterActionReport>();
                            //AfterActionReport AARModel = DRModel.Adapt<AfterActionReport>();
                            aarModel.AddressedTo = "1";

                            using (PreliminaryReportService PRService = new PreliminaryReportService())
                            {
                                //PreliminaryReport PRModel = GetAllPRs().FindAll(x => x.Id == AARModel.PRId).OrderByDescending(x => x.Id).FirstOrDefault();
                                PreliminaryReport PRModel = PRService.GetById(aarModel.PRId);
                                if (PRModel != null)
                                {
                                    aarModel.InitialReportedLatitude = PRModel.Latitude;
                                    aarModel.InitialReportedLongitude = PRModel.Longitude;
                                    aarModel.InitialReportedMMSI = PRModel.MMSI;
                                    aarModel.InitialReportedCourse = PRModel.Course;
                                    aarModel.InitialReportedSpeed = PRModel.Speed;
                                    aarModel.InitialReportedHeading = PRModel.Heading;

                                    if (PRModel.COIId != null)
                                    {
                                        using (COIService coiService = new COIService())
                                        {
                                            COI COIModel = coiService.GetById(aarModel.COIId);

                                            if (COIModel != null)
                                            {
                                                using (AmplifyingReportService arService = new AmplifyingReportService())
                                                {
                                                    AmplifyingReport ARModel = arService.GetById(aarModel.COIId);

                                                    if (ARModel != null)
                                                    {
                                                        aarModel.LastReportedLatitude = ARModel.Latitude;
                                                        aarModel.LastReportedLongitude = ARModel.Longitude;
                                                        aarModel.LastReportedMMSI = ARModel.MMSI;
                                                        aarModel.LastReportedCourse = ARModel.Course;
                                                        aarModel.LastReportedSpeed = ARModel.Speed;
                                                        aarModel.LastReportedHeading = ARModel.Heading;
                                                    }
                                                    else
                                                    {
                                                        aarModel.LastReportedLatitude = COIModel.Latitude;
                                                        aarModel.LastReportedLongitude = COIModel.Longitude;
                                                        aarModel.LastReportedMMSI = COIModel.MMSI;
                                                        aarModel.LastReportedCourse = COIModel.Course;
                                                        aarModel.LastReportedSpeed = COIModel.Speed;
                                                        aarModel.LastReportedHeading = COIModel.Heading;
                                                    }
                                                    aarModel.COITypeId = COIModel.COITypeId;
                                                    aarModel.NatureOfThreatId = COIModel.NatureOfThreatId;
                                                    aarModel.COIActivationDateTime = COIModel.COIActivationDate;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        using (SubsequentReportService SRService = new SubsequentReportService())
                                        {
                                            SubsequentReport SRModel = SRService.GetByPRId(aarModel.PRId).OrderByDescending(x => x.Id).FirstOrDefault();

                                            if (SRModel != null)
                                            {
                                                aarModel.LastReportedLatitude = SRModel.Latitude;
                                                aarModel.LastReportedLongitude = SRModel.Longitude;
                                                aarModel.LastReportedMMSI = SRModel.MMSI;
                                                aarModel.LastReportedCourse = SRModel.Course;
                                                aarModel.LastReportedSpeed = SRModel.Speed;
                                                aarModel.LastReportedHeading = SRModel.Heading;
                                            }
                                            else
                                            {
                                                aarModel.LastReportedLatitude = PRModel.Latitude;
                                                aarModel.LastReportedLongitude = PRModel.Longitude;
                                                aarModel.LastReportedMMSI = PRModel.MMSI;
                                                aarModel.LastReportedCourse = PRModel.Course;
                                                aarModel.LastReportedSpeed = PRModel.Speed;
                                                aarModel.LastReportedHeading = PRModel.Heading;
                                            }
                                        }
                                    }
                                    aarModel.COITypeId = PRModel.COITypeId;
                                    aarModel.NatureOfThreatId = PRModel.NatureOfThreatId;
                                }
                            }
                        }
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "AfterActionReportModal",
                    ViewData = new ViewDataDictionary<AfterActionReport>(ViewData, aarModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedAARs()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (AfterActionReportService aarService = new AfterActionReportService())
                {
                    //if (!User.IsInRole("ADMIN"))
                    //    queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());

                    return new JsonResult(aarService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllAARs()
        {
            try
            {
                using (AfterActionReportService aarService = new AfterActionReportService())
                {
                    //if (User.IsInRole("ADMIN"))
                    return new JsonResult(aarService.List());
                    //else
                    //    return new JsonResult(aarService.GetAllAARsBySubsId(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostAAR(AfterActionReport AAR)
        {
            try
            {
                Subscriber subscriberModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (AfterActionReportService aarService = new AfterActionReportService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subscriberModel.SubscriberId);
                        lstSubscribers.AddRange(AAR.AddressedToArray);
                        AfterActionReportView aarView = aarService.Add(subscriberModel, User.Identity.Name, AAR);

                        using (DropInfoSharingReportService drService = new DropInfoSharingReportService())
                        {
                            DropInfoSharingReport drModel = drService.GetById(AAR.DRId);
                            drModel.AARCreated = true;
                            drService.Update(subscriberModel.SubscriberId, User.Identity.Name, drModel);
                        }

                        foreach (int subscriberId in lstSubscribers)
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                Notifications notificationModel = new Notifications();

                                if (aarView.COIId > 0)
                                    notificationModel.NotificationContent = "After Action Report on " + aarView.COINumber + " against " + aarView.PRNumber + " is generated by " + aarView.SubscriberCode;
                                else
                                    notificationModel.NotificationContent = "After Action Report on " + aarView.PRNumber + " is generated by " + aarView.SubscriberCode;

                                notificationModel.NotificationType = (NotificationTypes.AAR).ToString();
                                notificationModel.ReportId = aarView.AARId;
                                notificationModel = notificationService.Add(notificationModel, subscriberModel.SubscriberId, User.Identity.Name);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushAAR(aarView);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetTemplate(int TemplateId)
        {
            try
            {
                Template templateModel = new Template();
                if (TemplateId > 0)
                {
                    using (TemplateService templateService = new TemplateService())
                    {
                        templateModel = templateService.GetById(TemplateId);
                    }
                }
                else
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    templateModel.SubscriberId = subsId;
                    templateModel.SubscriberId = 1;
                    templateModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
                }
                return new PartialViewResult
                {
                    ViewName = "TemplateModal",
                    ViewData = new ViewDataDictionary<Template>(ViewData, templateModel)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedTemplates()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (TemplateService templateService = new TemplateService())
                {
                    return new JsonResult(templateService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllTemplates()
        {
            try
            {
                using (TemplateService templateService = new TemplateService())
                {
                    return new JsonResult(templateService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostTemplate(Template template)
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                if (ModelState.IsValid)
                {
                    using (TemplateService templateService = new TemplateService())
                    {
                        List<int> lstSubscribers = new List<int>();
                        lstSubscribers.Add(subsModel.SubscriberId);
                        lstSubscribers.AddRange(template.AddressedToArray);
                        TemplateView templateView = templateService.Add(subsModel, User.Identity.Name, template);

                        foreach (int subscriberId in lstSubscribers.Distinct())
                        {
                            using (NotificationService notificationService = new NotificationService())
                            {
                                Notifications notificationModel = new Notifications();
                                notificationModel.NotificationContent = templateView.TemplateTypeName + "  is generated by " + templateView.SubscriberCode;
                                notificationModel = notificationService.Add(notificationModel, subscriberId, User.Identity.Name);
                                notificationModel.NotificationType = (NotificationTypes.Template).ToString();
                                notificationModel.ReportId = templateView.Id;
                                _hubContext.Clients.Group((subscriberId).ToString()).PushTemplate(template);
                                _hubContext.Clients.Group((subscriberId).ToString()).PushNotification(notificationModel);
                            }
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }


        public IActionResult OnGetNotes()
        {
            try
            {
                return new PartialViewResult
                {
                    ViewName = "CRUDNotes"
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllNotes()
        {
            try
            {
                using (NotesService notesService = new NotesService())
                {
                    return new JsonResult(notesService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostNotes(Notes NotesModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (NotesService notesService = new NotesService())
                    {
                        NotesModel = notesService.Add(subsId, User.Identity.Name, NotesModel);
                        _hubContext.Clients.All.PushNotes(NotesModel);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteNotes(int NotesID)
        {
            try
            {
                using (NotesService notesService = new NotesService())
                {
                    if (NotesID > 0)
                    {
                        notesService.Delete(NotesID);
                        _hubContext.Clients.All.PushNotes(null);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }


        public IActionResult OnGetTemplateType(int TemplateTypeId = 0)
        {
            try
            {
                using (TemplateTypeService templateTypeService = new TemplateTypeService())
                {
                    if (TemplateTypeId > 0)
                    {
                        TemplateType templateTypeModel = templateTypeService.GetById(TemplateTypeId);
                        return new PartialViewResult
                        {
                            ViewName = "EditTemplateType",
                            ViewData = new ViewDataDictionary<TemplateType>(ViewData, templateTypeModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "CRUDTemplateType"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllTemplateTypes()
        {
            try
            {
                using (TemplateTypeService templateTypeService = new TemplateTypeService())
                {
                    return new JsonResult(templateTypeService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostTemplateType(TemplateType TemplateTypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (TemplateTypeService templateTypeService = new TemplateTypeService())
                    {
                        if (TemplateTypeModel.TemplateTypeId > 0)
                        {
                            templateTypeService.Update(subsId, User.Identity.Name, TemplateTypeModel);
                            _hubContext.Clients.All.PushTemplateType(TemplateTypeModel);
                        }
                        else
                        {
                            TemplateTypeModel = templateTypeService.Add(subsId, User.Identity.Name, TemplateTypeModel);
                            _hubContext.Clients.All.PushTemplateType(TemplateTypeModel);
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteTemplateType(int TemplateTypeId)
        {
            try
            {
                using (TemplateTypeService templateTypeService = new TemplateTypeService())
                {
                    if (TemplateTypeId > 0)
                    {
                        templateTypeService.Delete(TemplateTypeId);
                        _hubContext.Clients.All.PushTemplateType(null);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }


        public IActionResult OnGetAllPagedIncidents()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (AAASIncidentService IncidentService = new AAASIncidentService())
                {
                    //if (!User.IsInRole("ADMIN"))
                    //    queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());
                    return new JsonResult(IncidentService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllPagedSOS()
        {
            try
            {
                Dictionary<string, string> queryParams = Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                using (AAASSOSService SosService = new AAASSOSService())
                {
                    //if (!User.IsInRole("ADMIN"))
                    //    queryParams.Add("subscriberid", CurrentSubscriber.SubscriberId.ToString());
                    return new JsonResult(SosService.ListPaged(queryParams));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetUser()
        {
            try
            {
                return new PartialViewResult
                {
                    ViewName = "CRUDUser"
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllUsers()
        {
            try
            {
                using (AspNetUserService userService = new AspNetUserService())
                {
                    return new JsonResult(userService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> OnPostUser(AppUserViewModel AppUserView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _user = await _userManager.FindByEmailAsync(AppUserView.Email);
                    if (_user != null)
                        return BadRequest("Email Already Exist");

                    else
                    {
                        var appUser = new AppUser()
                        {
                            UserName = AppUserView.Username,
                            Email = AppUserView.Email,
                            Subscriber_Id = AppUserView.SubscriberId
                        };

                        var createdUser = await _userManager.CreateAsync(appUser, AppUserView.Password);
                        if (createdUser.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(appUser, AppUserView.Role);
                            await _hubContext.Clients.All.PushUser(appUser);
                            return StatusCode(200);
                        }
                    }
                }
                return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> OnGetDeleteUser(string Id)
        {
            try
            {
                var appuser = await _userManager.FindByIdAsync(Id);
                await _userManager.DeleteAsync(appuser);
                await _hubContext.Clients.All.PushUser(appuser);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetUserType(int UserTypeId = 0)
        {
            try
            {
                using (UserTypeService userTypeService = new UserTypeService())
                {
                    if (UserTypeId > 0)
                    {
                        UserType userTypeModel = userTypeService.GetById(UserTypeId);
                        return new PartialViewResult
                        {
                            ViewName = "EditUserType",
                            ViewData = new ViewDataDictionary<UserType>(ViewData, userTypeModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "UserTypeModal"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllUserTypes()
        {
            try
            {
                using (UserTypeService userTypeService = new UserTypeService())
                {
                    return new JsonResult(userTypeService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostUserType(UserType UserTypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (UserTypeService userTypeService = new UserTypeService())
                    {
                        if (UserTypeModel.UserTypeId > 0)
                        {
                            userTypeService.Update(subsId, User.Identity.Name, UserTypeModel);
                            _hubContext.Clients.All.PushUserType(UserTypeModel);
                        }
                        else
                        {
                            UserTypeModel = userTypeService.Add(subsId, User.Identity.Name, UserTypeModel);
                            _hubContext.Clients.All.PushUserType(UserTypeModel);
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteUserType(int UserTypeId)
        {
            try
            {
                using (UserTypeService userTypeService = new UserTypeService())
                {
                    if (UserTypeId > 0)
                    {
                        userTypeService.Delete(UserTypeId);
                        _hubContext.Clients.All.PushUserType(null);
                        //UserEventLogging(User.Identity.Name, "User Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetNewsFeedType(int NewsFeedTypeId = 0)
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                {
                    if (NewsFeedTypeId > 0)
                    {
                        NewsFeedType newsFeedTypeModel = newsFeedTypeService.GetById(NewsFeedTypeId);
                        return new PartialViewResult
                        {
                            ViewName = "EditNewsFeedType",
                            ViewData = new ViewDataDictionary<NewsFeedType>(ViewData, newsFeedTypeModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "NewsFeedTypeModal"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllNewsFeedTypes()
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                {
                    return new JsonResult(newsFeedTypeService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostNewsFeedType(NewsFeedType NewsFeedTypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                    {
                        if (NewsFeedTypeModel.NewsFeedTypeId > 0)
                        {
                            newsFeedTypeService.Update(subsId, User.Identity.Name, NewsFeedTypeModel);
                            _hubContext.Clients.All.PushNewsFeedType(NewsFeedTypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            NewsFeedTypeModel = newsFeedTypeService.Add(subsId, User.Identity.Name, NewsFeedTypeModel);
                            _hubContext.Clients.All.PushNewsFeedType(NewsFeedTypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Created", Convert.ToInt32(EventTypes.COI_Type_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteNewsFeedType(int NewsFeedTypeId)
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                {
                    if (NewsFeedTypeId > 0)
                    {
                        newsFeedTypeService.Delete(NewsFeedTypeId);
                        _hubContext.Clients.All.PushNewsFeedType(null);
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetCOIStatus(int COIStatusId = 0)
        {
            try
            {
                using (COIStatusService coiStatusService = new COIStatusService())
                {
                    if (COIStatusId > 0)
                    {
                        COIStatus coiStatusModel = coiStatusService.GetById(COIStatusId);
                        return new PartialViewResult
                        {
                            ViewName = "EditCOIStatus",
                            ViewData = new ViewDataDictionary<COIStatus>(ViewData, coiStatusModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "COIStatusModal"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllCOIStatuses()
        {
            try
            {
                using (COIStatusService COIStatusService = new COIStatusService())
                {
                    return new JsonResult(COIStatusService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostCOIStatus(COIStatus COIStatusModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (COIStatusService coiStatusService = new COIStatusService())
                    {
                        if (COIStatusModel.COIStatusId > 0)
                        {
                            coiStatusService.Update(subsId, User.Identity.Name, COIStatusModel);
                            _hubContext.Clients.All.PushCOIStatus(COIStatusModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            COIStatusModel = coiStatusService.Add(subsId, User.Identity.Name, COIStatusModel);
                            _hubContext.Clients.All.PushCOIStatus(COIStatusModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Created", Convert.ToInt32(EventTypes.COI_Type_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteCOIStatus(int COIStatusId)
        {
            try
            {
                using (COIStatusService coiStatusService = new COIStatusService())
                {
                    if (COIStatusId > 0)
                    {
                        coiStatusService.Delete(COIStatusId);
                        _hubContext.Clients.All.PushCOIStatus(null);
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetCOIType(int COITypeId = 0)
        {
            try
            {
                using (COITypeService coiTypeService = new COITypeService())
                {
                    if (COITypeId > 0)
                    {
                        COIType coiTypeModel = coiTypeService.GetById(COITypeId);
                        return new PartialViewResult
                        {
                            ViewName = "EditCOIType",
                            ViewData = new ViewDataDictionary<COIType>(ViewData, coiTypeModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "CRUDCOIType"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetCOITypes()
        {
            try
            {
                using (COITypeService coiTypeService = new COITypeService())
                {
                    return new JsonResult(coiTypeService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostCOIType(COIType COITypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (COITypeService COITypeService = new COITypeService())
                    {
                        if (COITypeModel.COITypeId > 0)
                        {
                            COITypeService.Update(subsId, User.Identity.Name, COITypeModel);
                            _hubContext.Clients.All.PushCOIType(COITypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            COITypeModel = COITypeService.Add(subsId, User.Identity.Name, COITypeModel);
                            _hubContext.Clients.All.PushCOIType(COITypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Created", Convert.ToInt32(EventTypes.COI_Type_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteCOIType(int COITypeId)
        {
            try
            {
                using (COITypeService COITypeService = new COITypeService())
                {
                    if (COITypeId > 0)
                    {
                        COITypeService.Delete(COITypeId);
                        _hubContext.Clients.All.PushCOIType(null);
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetNatureOfThreat(int ThreatId = 0)
        {
            try
            {
                using (NatureOfThreatService threatService = new NatureOfThreatService())
                {
                    if (ThreatId > 0)
                    {
                        NatureOfThreat threatModel = threatService.GetById(ThreatId);
                        return new PartialViewResult
                        {
                            ViewName = "EditThreatLevel",
                            ViewData = new ViewDataDictionary<NatureOfThreat>(ViewData, threatModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "CRUDThreatLevel"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllNatureOfThreats()
        {
            try
            {
                using (NatureOfThreatService natureOfThreatService = new NatureOfThreatService())
                {
                    return new JsonResult(natureOfThreatService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostNatureOfThreat(NatureOfThreat ThreatModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (NatureOfThreatService threatService = new NatureOfThreatService())
                    {
                        if (ThreatModel.ThreatId > 0)
                        {
                            threatService.Update(subsId, User.Identity.Name, ThreatModel);
                            _hubContext.Clients.All.PushNatureOfThreat(ThreatModel);
                            //UserEventLogging(User.Identity.Name, "Threat Level Updated", Convert.ToInt32(EventTypes.Threat_Level_Updated));
                        }
                        else
                        {
                            ThreatModel = threatService.Add(subsId, User.Identity.Name, ThreatModel);
                            _hubContext.Clients.All.PushNatureOfThreat(ThreatModel);
                            //UserEventLogging(User.Identity.Name, "Threat Level Created", Convert.ToInt32(EventTypes.Threat_Level_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteNatureOfThreat(int ThreatId)
        {
            try
            {
                using (NatureOfThreatService threatService = new NatureOfThreatService())
                {
                    if (ThreatId > 0)
                    {
                        threatService.Delete(ThreatId);
                        _hubContext.Clients.All.PushNatureOfThreat(null);
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetInfoConLevel(int InfoConLevelId = 0)
        {
            try
            {
                using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                {
                    if (InfoConLevelId > 0)
                    {
                        InfoConfidenceLevel infoConLevelModel = infoConLevelService.GetById(InfoConLevelId).Adapt<InfoConfidenceLevel>();
                        return new PartialViewResult
                        {
                            ViewName = "EditInfoConLevel",
                            ViewData = new ViewDataDictionary<InfoConfidenceLevel>(ViewData, infoConLevelModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "InfoConLevelModal"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllInfoConLevels()
        {
            try
            {
                using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                {
                    return new JsonResult(infoConLevelService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostInfoConLevel(InfoConfidenceLevel InfoConLevelModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                    {
                        if (InfoConLevelModel.InfoConfidenceLevelId > 0)
                        {
                            infoConLevelService.Update(subsId, User.Identity.Name, InfoConLevelModel);
                            _hubContext.Clients.All.PushInfoConLevel(InfoConLevelModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            InfoConLevelModel = infoConLevelService.Add(subsId, User.Identity.Name, InfoConLevelModel);
                            _hubContext.Clients.All.PushInfoConLevel(InfoConLevelModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Created", Convert.ToInt32(EventTypes.COI_Type_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteInfoConLevel(int InfoConLevelId)
        {
            try
            {
                using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                {
                    if (InfoConLevelId > 0)
                    {
                        infoConLevelService.Delete(InfoConLevelId);
                        _hubContext.Clients.All.PushInfoConLevel(null);
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetDrawing()
        {
            try
            {
                return new PartialViewResult
                {
                    ViewName = "DrawingModal"
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllSubsDrawings()
        {
            try
            {
                using (DrawingService drService = new DrawingService())
                {
                    return new JsonResult(drService.List(CurrentSubscriber.SubscriberId));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostDrawing(Drawing Drawing)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int subsId = CurrentSubscriber.SubscriberId;
                    if (!string.IsNullOrWhiteSpace(Drawing.DrawingSource))
                    {
                        using (DrawingService drService = new DrawingService())
                        {
                            Drawing.SubscriberId = subsId;
                            Drawing.DrawingType = "Feature";
                            Drawing.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
                            Drawing.CreatedBy = User.Identity.Name;
                            Drawing = drService.Add(Drawing);
                        }
                    }
                    return StatusCode(200);
                }
                else
                {
                    return BadRequest("Invalid Form");
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteDrawing(int DrId)
        {
            try
            {
                using (DrawingService drService = new DrawingService())
                {
                    using (DrawingCoordinateService drCoordService = new DrawingCoordinateService())
                    {
                        List<DrawingCoordinate> drCoordinateList = drCoordService.GetByDrawingId(DrId);

                        foreach (var item in drCoordinateList)
                        {
                            drCoordService.Delete(item.DrawingCoordinateId);
                        }
                    }
                    drService.Delete(DrId);
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetNewsFeed()
        {
            try
            {
                NewsFeed newsFeed = new NewsFeed();
                newsFeed.SubscriberId = CurrentSubscriber.SubscriberId;
                return new PartialViewResult
                {
                    ViewName = "NewsFeedModal",
                    ViewData = new ViewDataDictionary<NewsFeed>(ViewData, newsFeed)
                };
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllNewsFeed()
        {
            try
            {

                using (NewsFeedService newsFeedService = new NewsFeedService())
                {
                    List<NewsFeedModel> newsFeedModelLst = new List<NewsFeedModel>();
                    XmlDocument xml = new XmlDocument();
                    foreach (var item in newsFeedService.List())
                    {
                        //try
                        //{
                        //URL currently hardcoded - but you could use a macro param to pass in URL
                        xml.Load(item.NewsSourceUrl);
                        //Select the nodes we want to loop through
                        XmlNodeList nodes = xml.SelectNodes("//item");

                        foreach (XmlNode node in nodes)
                        {
                            NewsFeedModel newsFeedModel = new NewsFeedModel();
                            newsFeedModel.NewsFeedTypeName = item.NewsFeedTypeName;
                            newsFeedModel.NewsFeedTitle = node.SelectSingleNode("title").InnerText;
                            newsFeedModel.NewsFeedDescription = node.SelectSingleNode("link").InnerText;

                            newsFeedModelLst.Add(newsFeedModel);
                        }
                        //Traverse the entire XML nodes.
                        //foreach (XmlNode node in nodes)
                        //{
                        //    return new JsonResult(node);

                        //    ////Get the value from the <title> node
                        //    //var title = node.SelectSingleNode("title").InnerText;
                        //    ////Get the value from the <description> node
                        //    //var description = node.SelectSingleNode("link").InnerText;
                        //    //                //<li style = "display:inline;margin-left:10px;" >[@item.NewsFeedTypeName] @title - < a href = @description rel = "nofollow" onclick = "var w = 800; var h = 500; var left = (screen.width / 2) - (w / 2); var top = (screen.height / 2) - (h / 2); var myWindow = window.open(this.href, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left); myWindow.focus(); return false;" style = "margin-right:10px;"> Read more </a> | </li>;

                        //}
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                    }
                    return new JsonResult(newsFeedModelLst);

                    //return new JsonResult(newsFeedService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostNewsFeed(NewsFeed NewsFeed)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validRSS = ValidateFeed(NewsFeed.NewsSourceUrl);
                    if (validRSS)
                    {
                        using (NewsFeedService newsFeedService = new NewsFeedService())
                        {
                            NewsFeed = newsFeedService.Add(NewsFeed, CurrentSubscriber.SubscriberId, User.Identity.Name);

                            _hubContext.Clients.All.PushNewsFeed(NewsFeed);
                        }
                        return StatusCode(200);
                    }
                    else
                        return BadRequest();
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetStakeholder(int SubsId)
        {
            try
            {
                using (SubscriberService subsService = new SubscriberService())
                {
                    if (SubsId > 0)
                    {
                        Subscriber subModel = subsService.GetById(SubsId);
                        return new PartialViewResult
                        {
                            ViewName = "EditStakeholder",
                            ViewData = new ViewDataDictionary<Subscriber>(ViewData, subModel)
                        };
                    }
                    else
                    {
                        return new PartialViewResult
                        {
                            ViewName = "CRUDStakeholder"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostStakeholder(Subscriber SubModel)
        {
            try
            {
                int subsId = CurrentSubscriber.SubscriberId;
                if (ModelState.IsValid)
                {
                    using (SubscriberService subscriberService = new SubscriberService())
                    {
                        if (SubModel.SubscriberId > 0)
                        {
                            subscriberService.Update(SubModel, subsId, User.Identity.Name);
                            _hubContext.Clients.All.PushStakeholder(SubModel);
                            //UserEventLogging(User.Identity.Name, "Stakeholder Updated", Convert.ToInt32(EventTypes.Stakeholder_Updated));
                        }
                        else
                        {
                            SubModel = subscriberService.Add(SubModel, subsId, User.Identity.Name);
                            _hubContext.Clients.All.PushStakeholder(SubModel);
                            //UserEventLogging(User.Identity.Name, "Stakeholder Created", Convert.ToInt32(EventTypes.Stakeholder_Created));
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetDeleteStakeholder(int SubsId)
        {
            try
            {
                using (SubscriberService subscriberService = new SubscriberService())
                {
                    if (SubsId > 0)
                    {
                        subscriberService.Delete(SubsId);
                        _hubContext.Clients.All.PushStakeholder(null);
                        //UserEventLogging(User.Identity.Name, "Stakeholder Deleted", Convert.ToInt32(EventTypes.Stakeholder_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetAllDrawingShapes()
        {
            try
            {
                using (DrawingShapeService drShapeService = new DrawingShapeService())
                {
                    return new JsonResult(drShapeService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllRadiusUnits()
        {
            try
            {
                using (RadUnitService radUnitService = new RadUnitService())
                {
                    return new JsonResult(radUnitService.List());
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllRoles()
        {
            try
            {
                return new JsonResult(_roleManager.Roles.ToList());
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnPostHideNotification(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (NotificationService notiService = new NotificationService())
                    {
                        Notifications notification = notiService.GetById(Id);
                        notiService.Update(notification, User.Identity.Name, CurrentSubscriber.SubscriberId);

                        //notification.IsRead = true;
                        //notification.LastModifiedBy = User.Identity.Name;
                        //notification.ReadBy = User.Identity.Name;
                        //notification.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
                        //notification.ReadOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
                        //notiService.Update(notification);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnGetAllNotifications()
        {
            try
            {

                using (NotificationService notificationService = new NotificationService())
                {
                    return new JsonResult(notificationService.GetSubsUnreadNitifications(CurrentSubscriber.SubscriberId));
                    //return new JsonResult(notificationService.List().FindAll(x => x.SubscriberId == subsId && x.IsRead == false).OrderByDescending(x => x.CreatedOn));
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetShip(string MMSI, string IMO)
        {
            try
            {
                Ship shipModel;
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
                                if (shipPicModel != null)
                                {
                                    if (((shipPicModel.PictureName).ToString()).Length > 2)
                                    {
                                        string folderName = ((shipPicModel.PictureName).ToString()).Substring(0, ((shipPicModel.PictureName).ToString()).Length - 2);
                                        string shipPicUrl = AppSettings.Configuration.GetSection("ProjectResources").GetSection("PhotoStoreUrl").Value;
                                        string shipPicName1URL = shipPicUrl + "\\" + folderName + "\\" + shipPicModel.PictureName + "_1.jpg";
                                        string shipPicName2URL = shipPicUrl + "\\" + folderName + "\\" + shipPicModel.PictureName + "_2.jpg";

                                        if (System.IO.File.Exists(shipPicName2URL))
                                            shipModel.PhotoContent = Common.GetBase64Content(shipPicName2URL);
                                        else if (System.IO.File.Exists(shipPicName1URL))
                                            shipModel.PhotoContent = Common.GetBase64Content(shipPicName1URL);
                                        else
                                            shipModel.PhotoPresent = false;

                                        if (string.IsNullOrWhiteSpace(shipModel.PhotoContent))
                                            shipModel.PhotoPresent = false;

                                        //log message picture path not valid 
                                    }
                                    else
                                        shipModel.PhotoPresent = false;
                                    //log massage picture name not valid 
                                }
                                else
                                    shipModel.PhotoPresent = false;

                                //log massage picture model is null
                            }
                        }
                        else
                        {
                            shipModel.PhotoPresent = false;
                        }
                        return new JsonResult(shipModel);
                    }
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetWeatherInfo()
        {
            try
            {
                Subscriber subsModel = CurrentSubscriber;
                using (CityService cityService = new CityService())
                {
                    City city = cityService.GetSubscriberCity(subsModel.City);
                    WeatherDetail weatherModel = new WeatherDetail();
                    if (city != null)
                    {
                        string tenMinPreviousDateTime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsModel.SubscriberId)).AddMinutes(-10).ToString();
                        using (WeatherDetailService wdService = new WeatherDetailService())
                        {
                            weatherModel = wdService.Get10MinPrevDetails(subsModel.SubscriberId, city.CityId, tenMinPreviousDateTime);

                            if (weatherModel == null)
                                weatherModel = UpdateWeather(city.CityId);

                            //weatherModel = WeatherModelConversions(weatherModel);

                            //if (weatherModel != null && weatherModel.WeatherId > 0)
                            //{
                            //    weatherModel = wdService.GetLastUpdatedDetails(city.CityId);
                            //}
                            //else
                            //UpdateWeather(city.CityId, tenMinPreviousDateTime);
                        }
                        return new JsonResult(weatherModel);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region "Functions old"
        //public List<COIView> GetAllCOIs()
        //{
        //    try
        //    {
        //        Subscriber subsModel = CurrentSubscriber;
        //        using (COIService COIService = new COIService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                COIViewList = COIService.List();

        //            else
        //                COIViewList = COIService.List().FindAll(x => x.SubscriberId == subsModel.SubscriberId || x.InformationAddressee.Contains(subsModel.SubscriberCode));
        //        }
        //        return COIViewList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //public List<AmplifyingReportView> GetAllARs()
        //{
        //    try
        //    {
        //        Subscriber subsModel = CurrentSubscriber;
        //        using (AmplifyingReportService ARService = new AmplifyingReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                ARViewList = ARService.List();

        //            else
        //                ARViewList = ARService.List().FindAll(x => x.SubscriberId == subsModel.SubscriberId || x.InformationAddressee.Contains(subsModel.SubscriberCode));
        //        }
        //        return ARViewList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //public List<DropReportView> GetAllDRs()
        //{
        //    try
        //    {
        //        int subsId = CurrentSubscriber.SubscriberId;
        //        using (DropInfoSharingReportService DRService = new DropInfoSharingReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                return DRService.List();
        //            else
        //                return DRService.List().FindAll(x => x.SubscriberId == subsId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}

        //public List<LostContactReportView> GetAllLRs()
        //{
        //    try
        //    {
        //        using (LostContactReportService LostReportService = new LostContactReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                LRViewList = LostReportService.List().ToList();

        //            else
        //                LRViewList = LostReportService.GetAllLRsBySubsId(CurrentSubscriber.SubscriberId).ToList();
        //        }
        //        return LRViewList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Saving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //public List<Subscriber> GetAllStakeholders()
        //{
        //    try
        //    {
        //        using (SubscriberService subscriberService = new SubscriberService())
        //        {
        //            stakeholderList = subscriberService.List();
        //            return stakeholderList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<DrawingShape> GetAllDrawingShapes()
        //{
        //    try
        //    {
        //        using (DrawingShapeService drShapeService = new DrawingShapeService())
        //        {
        //            drShapeList = drShapeService.List();
        //            return drShapeList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<RadUnit> GetAllRadiusUnits()
        //{
        //    try
        //    {
        //        using (RadUnitService radUnitService = new RadUnitService())
        //        {
        //            RadUnitList = radUnitService.List();
        //            return RadUnitList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<AspNetUserView> GetAllUsers()
        //{
        //    try
        //    {
        //        using (AspNetUserService userService = new AspNetUserService())
        //        {
        //            userList = userService.List();
        //            return userList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<IdentityRole> GetAllRoles()
        //{
        //    try
        //    {
        //        return _roleManager.Roles.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}




        //public void GetAllCOITypes()
        //{
        //    try
        //    {
        //        using (COITypeService coiTypeService = new COITypeService())
        //        {
        //            coiTypeList = coiTypeService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<UserType> GetAllUserTypes()
        //{
        //    try
        //    {
        //        using (UserTypeService userTypeService = new UserTypeService())
        //        {
        //            return userTypeService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<NewsFeedType> GetAllNewsFeedTypes()
        //{
        //    try
        //    {
        //        using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
        //        {
        //            return newsFeedTypeService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<COIStatus> GetAllCOIStatuses()
        //{
        //    try
        //    {
        //        using (COIStatusService COIStatusService = new COIStatusService())
        //        {
        //            return COIStatusService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public List<InfoConfidenceLevel> GetAllInfoConLevels()
        //{
        //    try
        //    {
        //        using (InfoConLevelService infoConLevelService = new InfoConLevelService())
        //        {
        //            return infoConLevelService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public void GetAllDrawings()
        //{
        //    try
        //    {
        //        using (DrawingService drService = new DrawingService())
        //        {
        //            DrawingList = drService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Drawing Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //public void GetAllNewsFeed()
        //{
        //    try
        //    {
        //        using (NewsFeedService newsFeedService = new NewsFeedService())
        //        {
        //            newsFeedViewList = newsFeedService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving News Feeds." + Environment.NewLine + ex.Message);
        //    }
        //}
        //public List<NewsFeedType> GetAllNewsTypes()
        //{
        //    try
        //    {
        //        using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
        //        {
        //            newsFeedTypeList = newsFeedTypeService.List();
        //        }
        //        return newsFeedTypeList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private void GetStakeholderDrawings()
        //{
        //    try
        //    {
        //        Dictionary<string, string> dicFilters = null;
        //        using (DrawingService drService = new DrawingService())
        //        {
        //            dicFilters = new Dictionary<string, string>();
        //            dicFilters.Add("orderby", "Drawing_Type");
        //            dicFilters.Add("offset", "1");
        //            dicFilters.Add("limit", "200");

        //            DrawingList = drService.GetBySubsId(user.Subscriber_Id, dicFilters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private void GetAllDrawingCoordinates()
        //{
        //    try
        //    {
        //        using (DrawingCoordinateService drCoordService = new DrawingCoordinateService())
        //        {
        //            drCoordinateList = drCoordService.List();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private List<PreliminaryReportView> GetAllPRs()
        //{
        //    try
        //    {
        //        using (PreliminaryReportService PRService = new PreliminaryReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                PRViewList = PRService.List().ToList();

        //            else
        //                PRViewList = PRService.GetAllPRsBySubsId(CurrentSubscriber.SubscriberId).ToList(); 

        //            return PRViewList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //private List<SubsequentReportView> GetAllSRs()
        //{
        //    try
        //    {
        //        using (SubsequentReportService SRService = new SubsequentReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                SRViewList = SRService.List().ToList();

        //            else
        //                SRViewList = SRService.GetAllSRsBySubsId(CurrentSubscriber.SubscriberId).ToList();

        //            return SRViewList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Saving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        //public void ShowNotification()
        //{
        //    try
        //    {
        //        using (COIService coiService = new COIService())
        //        {

        //            COIViewList = coiService.List();
        //            COIModelList = COIViewList.FindAll(COI => COI.COIStatusId == 3);


        //            //COIModelList = coiService.GetByActivationStatus();

        //        }
        //        using (NewsService newsService = new NewsService())
        //        {
        //            NewsModelList = newsService.GetByActivationStatus();
        //            ApprovedNewsList = newsService.GetApprovedNews();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}

        //public void AddSubsCOI(COI coiModel, int[] selectedSubs)
        //{
        //    try
        //    {
        //        using (SubscriberCOIService subsCOIService = new SubscriberCOIService())
        //        {
        //            if (subsCOIModel != null && subsCOIModel.Id > 0)
        //            {
        //                //to update Drawing
        //            }
        //            else
        //            {
        //                subsCOIModel.COIId = coiModel.COIId;
        //                //List<AspNetUser> aspNetUsersList = UserService.GetIdByName(User.Identity.Name);
        //                foreach (var item in selectedSubs)
        //                {
        //                    subsCOIModel.SubscriberId = item;
        //                    subsCOIService.Add(subsCOIModel);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public string GetNextPRNumber(string PRNumber)
        //{
        //    string[] prNumberParts = PRNumber.Split('-');
        //    if (prNumberParts.Length > 2)
        //    {
        //        prNumberParts[2] = Convert.ToString(Convert.ToInt64(PRNumber.Split('-')[2]) + 1);
        //        return string.Join('-', prNumberParts);
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid Form").ToString();
        //        //throw new Exception("Invalid PR Number");
        //    }
        //}
        #endregion

        #region "Event Handlers old"
        public void OnGet()
        {
            user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            GetCurrentSubscriber();


            //GetSubscriberName();
            //GetSubscriberCityId();
            //var tasks = new[]
            //{
            //    Task.Factory.StartNew(() => ShowWeatherInfo()),
            //    Task.Factory.StartNew(() => GetStakeholderDrawings()),
            //    Task.Factory.StartNew(() => GetAllNewsFeed()),
            //    Task.Factory.StartNew(() => GetAllPRs()),
            //    Task.Factory.StartNew(() => GetAllSRs()),
            //    Task.Factory.StartNew(() => GetAllLRs()),
            //    Task.Factory.StartNew(() => GetAllCOIs()),
            //    Task.Factory.StartNew(() => GetAllARs()),
            //    Task.Factory.StartNew(() => GetAllAARs()),
            //    //Task.Factory.StartNew(() => GetAllNotification())
            //};

            //GetAllNotification();
            //Task.WaitAll(tasks);

            //Parallel.Invoke(() => Task.Factory.StartNew(() => ShowWeatherInfo()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetStakeholderDrawings()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllNewsFeed()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllPRs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllSRs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllLRs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllCOIs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllARs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllAARs()));
            //Parallel.Invoke(() => Task.Factory.StartNew(() => GetAllNotification()));
            //Task.WaitAll();
        }

        //public JsonResult OnGetAllAARs()
        //{
        //    try
        //    {
        //        return new JsonResult(GetAllAARs());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving After Action Reports Data " + Environment.NewLine + ex.Message);
        //    }
        //}




        //public PartialViewResult OnGetNews(News News)
        //{
        //    NewsViewModel newsModel = new NewsViewModel();
        //    using (SubscriberService subscriberService = new SubscriberService())
        //    {
        //        newsModel.SubscriberList = new SelectList(subscriberService.List().ToList(), nameof(Subscriber.SubscriberId), nameof(Subscriber.SubscriberName));

        //        newsModel.News = new News();
        //        newsModel.News.SubscriberId = CurrentSubscriber.SubscriberId;
        //    }

        //    using (NewsTypeService newsTypeService = new NewsTypeService())
        //    {
        //        newsModel.NewsTypeList = new SelectList(newsTypeService.List().ToList(), nameof(NewsType.NewsTypeId), nameof(NewsType.NewsTypeName));
        //    }

        //    if (News.NewsId > 0)
        //    {
        //        using (NewsService COIService = new NewsService())
        //        {
        //            newsModel.News = COIService.GetById(News.NewsId);
        //            if (!string.IsNullOrWhiteSpace(newsModel.News.ReportedTo))
        //                newsModel.SelectedReportedTo = newsModel.News.ReportedTo.Split(',').Select(x => int.Parse(x)).ToArray();
        //        }
        //    }

        //    return new PartialViewResult
        //    {
        //        ViewName = "NewsModal",
        //        ViewData = new ViewDataDictionary<NewsViewModel>(ViewData, newsModel)
        //    };
        //}
        //public IActionResult OnPostNews(NewsViewModel NewsViewModel)
        //{
        //    try
        //    {
        //        int subsId = CurrentSubscriber.SubscriberId;
        //        using (NewsService NewsService = new NewsService())
        //        {
        //            if (NewsViewModel.News != null && NewsViewModel.News.NewsId > 0)
        //            {
        //                NewsViewModel.News.NewsStatusId = Convert.ToInt32(Request.Form["StatusId"]);
        //                if (!User.IsInRole("JMICC") || !User.IsInRole("ADMIN"))
        //                {
        //                    if (NewsViewModel.News.NewsStatusId == 1)
        //                    {
        //                        NewsViewModel.News.NewsActivationDate = DateTime.Now;
        //                    }
        //                    else if (NewsViewModel.News.NewsStatusId == 2)
        //                    {
        //                        NewsViewModel.News.NewsDeactivationDate = DateTime.Now;
        //                    }
        //                }
        //                NewsViewModel.News.ReportedTo = string.Join(",", NewsViewModel.SelectedReportedTo);
        //                NewsViewModel.News.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
        //                NewsViewModel.News.LastModifiedBy = User.Identity.Name;
        //                NewsService.Update(NewsViewModel.News);
        //                UserEventLogging(User.Identity.Name, "News Updated", Convert.ToInt32(EventTypes.News_Updated));
        //            }
        //            else
        //            {
        //                if (User.IsInRole("JMICC") || User.IsInRole("ADMIN"))
        //                {
        //                    NewsViewModel.News.NewsStatusId = 1;
        //                    NewsViewModel.News.NewsActivationDate = DateTime.Now;
        //                }
        //                else
        //                    NewsViewModel.News.NewsStatusId = 3;

        //                NewsViewModel.News.ReportedTo = string.Join(",", NewsViewModel.SelectedReportedTo);
        //                NewsViewModel.News.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + subsId));
        //                NewsViewModel.News.CreatedBy = User.Identity.Name;
        //                NewsViewModel.News = NewsService.Add(NewsViewModel.News);
        //                UserEventLogging(User.Identity.Name, "News Created", Convert.ToInt32(EventTypes.News_Created));
        //            }
        //            _hubContext.Clients.All.PushNews(NewsViewModel);
        //        }

        //        return new RedirectToPageResult("Canvas");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Saving Data " + Environment.NewLine + ex.Message);
        //    }

        //}

        //public PartialViewResult OnGetViewCOI(string Id)
        //{
        //    using (COIService COIService = new COIService())
        //    {
        //        COI coiModel = COIService.GetById(Convert.ToInt32(Id));
        //        return new PartialViewResult
        //        {
        //            ViewName = "COIActivationReportModal",
        //            ViewData = new ViewDataDictionary<COI>(ViewData, coiModel)
        //        };
        //    }
        //}
        #endregion
    }
}