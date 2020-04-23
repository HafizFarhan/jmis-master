using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using JMICSAPP.Data;
using JMICSAPP.Hubs;
using JMICSAPP.Models;
using JMICSAPP.Properties;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

namespace JMICSAPP.Pages
{
    public class IndexModel : PageModel
    {
        #region "Getters And Setters"
        //public AppUser user;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CanvasModel> _logger;
        private readonly MemCache _memCache;
        private IHubContext<PushHub, IPushHub> _hubContext;

        public IndexModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<CanvasModel> logger, IMemoryCache cache, IHubContext<PushHub, IPushHub> hubContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _memCache = new MemCache(cache);
            _hubContext = hubContext;
        }
        #endregion

        #region "Functions"
        public int GetSubscriberID()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            int loggedInSubsId = Convert.ToInt32(user.Result.Subscriber_Id);
            return Convert.ToInt32(user.Result.Subscriber_Id);
        }
        public string GetSubscriberName()
        {
            return GetAllStakeholders().FindAll(x => x.SubscriberId == GetSubscriberID()).FirstOrDefault().SubscriberName;
        }
        public string GetNextPRNumber(PreliminaryReportView model)
        {
            string[] prNumberParts = model.PRNumber.Split('-');
            if (prNumberParts.Length > 2)
            {
                prNumberParts[2] = Convert.ToString(Convert.ToInt64(model.PRNumber.Split('-')[2]) + 1);
                return string.Join('-', prNumberParts);
            }
            else
                throw new Exception("Invalid PR Number");
        }
        public string GetNextSRNumber(SubsequentReportView model)
        {
            string[] srNumberParts = model.SRNumber.Split('-');
            if (srNumberParts.Length > 2)
            {
                srNumberParts[2] = Convert.ToString(Convert.ToInt64(model.SRNumber.Split('-')[2]) + 1);
                return string.Join('-', srNumberParts);
            }
            else
                throw new Exception("Invalid SR Number");
        }
        public string GetNextCOINumber(COIView model)
        {
            string[] COINumberParts = model.COINumber.Split('-');
            if (COINumberParts.Length > 2)
            {
                COINumberParts[2] = Convert.ToString(Convert.ToInt64(model.COINumber.Split('-')[2]) + 1);
                return string.Join('-', COINumberParts);
            }
            else
                throw new Exception("Invalid COI Number");
        }
        public string GetNextARNumber(AmplifyingReportView model)
        {
            string[] arNumberParts = model.ARNumber.Split('-');
            if (arNumberParts.Length > 2)
            {
                arNumberParts[2] = Convert.ToString(Convert.ToInt64(model.ARNumber.Split('-')[2]) + 1);
                return string.Join('-', arNumberParts);
            }
            else
                throw new Exception("Invalid AR Number");
            //return StatusCode(400).ToString();
        }
        //public List<PreliminaryReportView> ShowPRsToStakeholder()
        //{
        //    try
        //    {
        //        if (User.IsInRole("ADMIN"))
        //            return GetAllPRs();
        //        else
        //            return GetAllPRs().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName())).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
        //    }
        //}
        public List<SubsequentReportView> ShowSRsToStakeholder()
        {
            try
            {
                if (User.IsInRole("ADMIN"))
                    return GetAllSRs();
                else
                    return GetAllSRs().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName())).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
            }
        }
        public List<COIView> ShowCOIsToStakeholder()
        {
            try
            {
                if (User.IsInRole("ADMIN"))
                    return GetAllCOIs();

                else
                    return GetAllCOIs().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName())).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
            }
        }
        public List<LostContactReportView> ShowLRsToStakeholder()
        {
            try
            {
                if (User.IsInRole("ADMIN"))
                    return GetAllLRs();

                else
                    return GetAllLRs().FindAll(x => x.SubscriberId == GetSubscriberID()).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Data " + Environment.NewLine + ex.Message);
            }
        }
        private List<Drawing> GetStakeholderDrawings()
        {
            try
            {
                return GetAllDrawings().FindAll(Drawing => Drawing.SubscriberId == GetSubscriberID());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Drawing Data " + Environment.NewLine + ex.Message);
            }
        }
        private void ShowWeatherInfo()
        {
            try
            {
                string cityId = "1174872";
                string currentDateTime = DateTime.Now.AddMinutes(-10).ToString();
                using (WeatherDetailService wdService = new WeatherDetailService())
                {
                    WeatherDetail weatherModel = GetAllWeatherDetails().FindAll(x => x.CityId == Convert.ToInt32(cityId) && x.CreatedOn >= Convert.ToDateTime(currentDateTime) && x.CreatedOn <= (Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID())))).OrderByDescending(x => x.WeatherId).FirstOrDefault();
                    //wdService.Get10MinPrevDetails(cityId, currentDateTime);

                    if (weatherModel != null && weatherModel.WeatherId > 0)
                        weatherModel = GetAllWeatherDetails().FindAll(x => x.CityId == Convert.ToInt32(cityId)).OrderByDescending(x => x.WeatherId).FirstOrDefault(); //wdService.GetLastUpdatedDetails(cityId);
                    
                    else
                        UpdateWeather(cityId, currentDateTime);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving weather. " + Environment.NewLine + ex.Message);
            }
        }
        private void UpdateWeather(string cityId, string currentDateTime)
        {
            try
            {
                RestClient restClient = new RestClient(Resources.OpenWeatherBaseURL);
                RestRequest trackRequest = new RestRequest("/data/2.5/weather?id=" + cityId + "&appid=" + Common.WeatherAPIKey, Method.GET);
                trackRequest.RequestFormat = DataFormat.Json;
                IRestResponse<WeatherResponse> response = restClient.Execute<WeatherResponse>(trackRequest);
                WeatherDetail weatherModel = new WeatherDetail();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (WeatherDetailService wdService = new WeatherDetailService())
                    {
                        weatherModel.Temp = (decimal)(response.Data.Main.Temp - 273.15); //Common.KelvinToCalcius(response.Data)
                        weatherModel.TempMax = (decimal)(response.Data.Main.TempMax - 273.15);
                        weatherModel.TempMin = (decimal)(response.Data.Main.TempMin - 273.15);
                        weatherModel.Pressure = response.Data.Main.Pressure;
                        weatherModel.Humidity = response.Data.Main.Humidity;

                        weatherModel.Main = response.Data.Weather[0].Main;
                        weatherModel.Description = response.Data.Weather[0].Description;

                        weatherModel.Speed = (decimal)response.Data.Wind.Speed;
                        weatherModel.Deg = response.Data.Wind.Deg;

                        weatherModel.Sunrise = Common.UnixTimeToDateTime(response.Data.Sys.Sunrise);
                        weatherModel.Sunset = Common.UnixTimeToDateTime(response.Data.Sys.Sunset);
                        weatherModel.Country = response.Data.Sys.Country;

                        weatherModel.Dt = Common.UnixTimeToDateTime(response.Data.Dt);
                        weatherModel.Timezone = Common.UnixTimeToDateTime(response.Data.Timezone).ToString();
                        weatherModel.CityId = unchecked((int)response.Data.Id);
                        weatherModel.Name = response.Data.Name;

                        weatherModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        weatherModel.CreatedBy = User.Identity.Name;
                        weatherModel = null;    // wdService.Add(weatherModel);
                    }
                }
                else
                    weatherModel = GetAllWeatherDetails().FindAll(x => x.CityId == Convert.ToInt32(cityId)).OrderByDescending(x => x.WeatherId).FirstOrDefault(); //wdService.GetLastUpdatedDetails(cityId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Updating weather. " + Environment.NewLine + ex.Message);
            }
        }


        public List<Subscriber> GetAllStakeholders()
        {
            try
            {
                using (SubscriberService subscriberService = new SubscriberService())
                    return subscriberService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AspNetUserView> GetAllUsers()
        {
            try
            {
                using (AspNetUserService userService = new AspNetUserService())
                    return userService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserType> GetAllUserTypes()
        {
            try
            {
                using (UserTypeService userTypeService = new UserTypeService())
                {
                    return userTypeService.List();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<IdentityRole> GetAllRoles()
        {
            try
            {
                return _roleManager.Roles.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsView> GetAllNews()
        {
            try
            {
                using (NewsService newsService = new NewsService())
                    return newsService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsFeedType> GetAllNewsTypes()
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                    return newsFeedTypeService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsFeedView> GetAllNewsFeed()
        {
            try
            {
                using (NewsFeedService newsFeedService = new NewsFeedService())
                    return newsFeedService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsFeedType> GetAllNewsFeedTypes()
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                {
                    return newsFeedTypeService.List();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIView> GetAllCOIs()
        {
            try
            {
                using (COIService COIService = new COIService())
                {
                    if (User.IsInRole("ADMIN"))
                        return null; //COIService.List();
                    else
                        return null; //COIService.List().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIType> GetAllCOITypes()
        {
            try
            {
                using (COITypeService coiTypeService = new COITypeService())
                    return coiTypeService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIStatus> GetAllCOIStatuses()
        {
            try
            {
                using (COIStatusService COIStatusService = new COIStatusService())
                {
                    return COIStatusService.List();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public List<PreliminaryReportView> GetAllPRs()
        //{
        //    try
        //    {
        //        using (PreliminaryReportService PRService = new PreliminaryReportService())
        //        {
        //            if (User.IsInRole("ADMIN"))
        //                //return PRService.List();
        //            else
        //                //return PRService.List().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private List<SubsequentReportView> GetAllSRs()
        {
            try
            {
                using (SubsequentReportService SRService = new SubsequentReportService())
                {
                    return null;
                    //if (User.IsInRole("ADMIN"))
                    //    return SRService.List();
                    //else
                    //    return SRService.List().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AmplifyingReportView> GetAllARs()
        {
            try
            {
                using (AmplifyingReportService ARService = new AmplifyingReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return null; //ARService.List();
                    else
                        return null; //ARService.List().FindAll(x => x.SubscriberId == GetSubscriberID() || x.InformationAddressee.Contains(GetSubscriberName()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AfterActionReportView> GetAllAARs()
        {
            try
            {
                using (AfterActionReportService AARService = new AfterActionReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return null; //AARService.ListPaged();

                    else
                        return null; //AARService.ListPaged().FindAll(x => x.SubscriberId == GetSubscriberID() || x.AddressedTo.Contains(GetSubscriberName()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LostContactReportView> GetAllLRs()
        {
            try
            {
                using (LostContactReportService LostReportService = new LostContactReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return LostReportService.List().ToList();
                    else
                        return LostReportService.List().ToList().FindAll(x => x.SubscriberId == GetSubscriberID());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LostContactReportView> GetAllLostReports()
        {
            try
            {
                using (LostContactReportService lostReportService = new LostContactReportService())
                    return lostReportService.List().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DropReportView> GetAllDRs()
        {
            try
            {
                using (DropInfoSharingReportService DRService = new DropInfoSharingReportService())
                {
                    if (User.IsInRole("ADMIN"))
                        return null;    // DRService.List();
                    else
                        return null;    // DRService.List().FindAll(x => x.SubscriberId == GetSubscriberID());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RadUnit> GetAllRadiusUnits()
        {
            try
            {
                using (RadUnitService radUnitService = new RadUnitService())
                    return radUnitService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Drawing> GetAllDrawings()
        {
            try
            {
                using (DrawingService drService = new DrawingService())
                    return null;    //drService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingCoordinate> GetAllDrawingCoordinates()
        {
            try
            {
                using (DrawingCoordinateService drCoordService = new DrawingCoordinateService())
                    return drCoordService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NatureOfThreat> GetAllNatureOfThreats()
        {
            try
            {
                using (NatureOfThreatService notService = new NatureOfThreatService())
                    return notService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<InfoConfidenceLevel> GetAllInfoConLevels()
        {
            try
            {
                using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                {
                    return infoConLevelService.List();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsView> ShowNewsNotifications()
        {
            try
            {
                return GetAllNews().FindAll(News => News.NewsStatusId == Convert.ToInt32(COIStatuses.Pending));
                //Approved News:
                // return GetAllNews().FindAll(News => News.NewsStatusId == Convert.ToInt32(COIStatuses.Approved)));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<NotificationView> GetAllNotifications()
        {
            try
            {
                using (NotificationService notificationService = new NotificationService())
                {
                    if (User.IsInRole("ADMIN"))
                        return notificationService.List().OrderByDescending(x => x.CreatedOn).ToList();

                    else
                        return notificationService.List().FindAll(x => x.SubscriberId == GetSubscriberID()).OrderByDescending(x => x.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<WeatherDetail> GetAllWeatherDetails()
        {
            try
            {
                using (WeatherDetailService weatherDetailService = new WeatherDetailService())
                    return weatherDetailService.List();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ValidateFeed(string sourceURL)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(sourceURL);
                XmlNode node = xml.SelectSingleNode("//item");
                XmlNode title = node.SelectSingleNode("title");
                XmlNode description = node.SelectSingleNode("link");
                if (node != null && title != null && description != null)
                    return true;
                
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region "Event Handlers"
        public IActionResult OnGet()
        {
            if (User.Identity.Name == null)
                return LocalRedirect("~/Identity/Account/Login");
            else
            {
                // CheckIfUserLoggedIn();
                GetSubscriberID();
                ShowWeatherInfo();
                var tasks = new[]
                {
                    Task.Factory.StartNew(() => ShowWeatherInfo()),
                    Task.Factory.StartNew(() => GetStakeholderDrawings()),
                    Task.Factory.StartNew(() => GetAllNewsFeed()),
                    //Task.Factory.StartNew(() => GetAllPRs()),
                    Task.Factory.StartNew(() => GetAllSRs()),
                    Task.Factory.StartNew(() => GetAllLRs()),
                    Task.Factory.StartNew(() => GetAllCOIs()),
                    Task.Factory.StartNew(() => GetAllARs()),
                    Task.Factory.StartNew(() => GetAllAARs()),
                    //Task.Factory.StartNew(() => GetAllNotification())
                };
                GetAllNotifications();
                return null;
            }
        }

        public JsonResult OnGetAllStakeholders()
        {
            try
            {
                return new JsonResult(GetAllStakeholders());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Stakeholders. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllUsers()
        {
            try
            {
                return new JsonResult(GetAllUsers());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Users. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllUserTypes()
        {
            try
            {
                return new JsonResult(GetAllUserTypes());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving User Types. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllRoles()
        {
            try
            {
                return new JsonResult(GetAllRoles());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving User Roles. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllCOIs()
        {
            try
            {
                return new JsonResult(GetAllCOIs());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving COIs. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllCOITypes()
        {
            try
            {
                return new JsonResult(GetAllCOITypes());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving COI Types. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllCOIStatuses()
        {
            try
            {
                return new JsonResult(GetAllCOIStatuses());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving COI Statuses. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllNewsFeed()
        {
            try
            {
                return new JsonResult(GetAllNewsFeed());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving News Feeds." + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllNewsFeedTypes()
        {
            try
            {
                return new JsonResult(GetAllNewsTypes());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving News Feed Types. " + Environment.NewLine + ex.Message);
            }
        }
        //public JsonResult OnGetAllPRs()
        //{
        //    try
        //    {
        //        return new JsonResult(GetAllPRs());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Retrieving Preliminary Reports. " + Environment.NewLine + ex.Message);
        //    }
        //}
        public JsonResult OnGetAllSRs()
        {
            try
            {
                return new JsonResult(GetAllSRs());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Subsequent Reports. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllARs()
        {
            try
            {
                return new JsonResult(GetAllARs());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Amplifying Reports. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllAARs()
        {
            try
            {
                return new JsonResult(GetAllAARs());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving After Action Reports. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllDropReports()
        {
            try
            {
                return new JsonResult(GetAllDRs());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Drop Reports. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllLostReports()
        {
            try
            {
                return new JsonResult(GetAllLostReports());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Lost Reports. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllNatureOfThreats()
        {
            try
            {
                return new JsonResult(GetAllNatureOfThreats());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Nature Of Threats. " + Environment.NewLine + ex.Message);
            }
        }
        public JsonResult OnGetAllInfoConLevels()
        {
            try
            {
                return new JsonResult(GetAllInfoConLevels());
            }
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving Information Confidence Levels. " + Environment.NewLine + ex.Message);
            }
        }



        public PartialViewResult OnGetStakeholder(string stakeholderId)
        {
            try
            {
                if (Convert.ToInt32(stakeholderId) > 0)
                {
                    Subscriber subModel = GetAllStakeholders().FindAll(x => x.SubscriberId == Convert.ToInt32(stakeholderId)).OrderByDescending(x => x.SubscriberId).FirstOrDefault();
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Stakeholder. " + Environment.NewLine + ex.Message);
            }
        }
        public PartialViewResult OnGetUser()
        {
            return new PartialViewResult
            {
                ViewName = "CRUDUser"
            };
        }
        public PartialViewResult OnGetNewsFeed()
        {
            return new PartialViewResult
            {
                ViewName = "NewsFeedModal"
            };
        }
        public PartialViewResult OnGetNewsFeedType(string newsFeedTypeId)
        {
            using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
            {
                if (Convert.ToInt32(newsFeedTypeId) > 0)
                {
                    NewsFeedType newsFeedTypeModel = newsFeedTypeService.GetById(Convert.ToInt32(newsFeedTypeId)).Adapt<NewsFeedType>();
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
        public PartialViewResult OnGetCOIActivationReport()
        {
            return new PartialViewResult
            {
                ViewName = "COIActivationReportModal"
            };
        }
        public PartialViewResult OnGetCOIType(string coiTypeId)
        {
            try
            {
                if (Convert.ToInt32(coiTypeId) > 0)
                {
                    COIType coiTypeModel = GetAllCOITypes().FindAll(x => x.COITypeId == Convert.ToInt32(coiTypeId)).OrderByDescending(x => x.COITypeId).FirstOrDefault();
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
            catch (Exception ex)
            {
                throw new Exception("Error Retrieving COI Type. " + Environment.NewLine + ex.Message);
            }
        }
        public PartialViewResult OnGetPR()
        {
            PreliminaryReport prReport = new PreliminaryReport();
            prReport.SubscriberId = GetSubscriberID();
            prReport.ActionAddressee = "JMICC";
            return new PartialViewResult
            {
                ViewName = "PreliminaryReportModal",
                ViewData = new ViewDataDictionary<PreliminaryReport>(ViewData, prReport)
            };
        }
        public PartialViewResult OnGetSR(string prId)
        {
            PreliminaryReport prReport = new PreliminaryReport(); //GetAllPRs().FindAll(x => x.Id == Convert.ToInt32(prId)).OrderByDescending(x => x.Id).FirstOrDefault();
            SubsequentReport srModel = prReport.Adapt<SubsequentReport>();
            return new PartialViewResult
            {
                ViewName = "SubsequentReportModal",
                ViewData = new ViewDataDictionary<SubsequentReport>(ViewData, srModel)
            };
        }
        public PartialViewResult OnGetLR(string prId)
        {
            PreliminaryReport prReport = new PreliminaryReport(); //GetAllPRs().FindAll(x => x.Id == Convert.ToInt32(prId)).OrderByDescending(x => x.Id).FirstOrDefault();
            LostContactReport lostReportModel = prReport.Adapt<LostContactReport>();
            return new PartialViewResult
            {
                ViewName = "LostReportModal",
                ViewData = new ViewDataDictionary<LostContactReport>(ViewData, lostReportModel)
            };
        }
        public PartialViewResult OnGetDR(string prId)
        {
            PreliminaryReport prReport = new PreliminaryReport(); //GetAllPRs().FindAll(x => x.Id == Convert.ToInt32(prId)).OrderByDescending(x => x.Id).FirstOrDefault();
            DropInfoSharingReport DRModel = prReport.Adapt<DropInfoSharingReport>();
            return new PartialViewResult
            {
                ViewName = "DropReportModal",
                ViewData = new ViewDataDictionary<DropInfoSharingReport>(ViewData, DRModel)
            };
        }
        public PartialViewResult OnGetAmplifyingReport(string coiId)
        {
            COI COIReport = GetAllCOIs().FindAll(x => x.COIId == Convert.ToInt32(coiId)).OrderByDescending(x => x.COIId).FirstOrDefault();
            AmplifyingReport ARModel = COIReport.Adapt<AmplifyingReport>();
            return new PartialViewResult
            {
                ViewName = "AmplifyingReportModal",
                ViewData = new ViewDataDictionary<AmplifyingReport>(ViewData, ARModel)
            };
        }
        public PartialViewResult OnGetAfterActionReport(string drId)
        {
            DropInfoSharingReport DRModel = GetAllDRs().FindAll(x => x.Id == Convert.ToInt32(drId)).OrderByDescending(x => x.Id).FirstOrDefault();
            AfterActionReport AARModel = DRModel.Adapt<AfterActionReport>();
            AARModel.AddressedTo = "JMICC";

            PreliminaryReport PRModel = new PreliminaryReport(); //GetAllPRs().FindAll(x => x.Id == AARModel.PRId).OrderByDescending(x => x.Id).FirstOrDefault();
            if (PRModel != null)
            {
                AARModel.InitialReportedLatitude = PRModel.Latitude;
                AARModel.InitialReportedLongitude = PRModel.Longitude;
                AARModel.InitialReportedMMSI = PRModel.MMSI;
                AARModel.InitialReportedCourse = PRModel.Course;
                AARModel.InitialReportedSpeed = PRModel.Speed;
                AARModel.InitialReportedHeading = PRModel.Heading;

                if (PRModel.COIId != null)
                {
                    COI COIModel = GetAllCOIs().FindAll(x => x.COIId == AARModel.COIId).OrderByDescending(x => x.COIId).FirstOrDefault();

                    if (COIModel != null)
                    {
                        AmplifyingReport ARModel = GetAllARs().FindAll(x => x.COIId == AARModel.COIId).OrderByDescending(x => x.COIId).FirstOrDefault();
                        if (ARModel != null)
                        {
                            AARModel.LastReportedLatitude = ARModel.Latitude;
                            AARModel.LastReportedLongitude = ARModel.Longitude;
                            AARModel.LastReportedMMSI = ARModel.MMSI;
                            AARModel.LastReportedCourse = ARModel.Course;
                            AARModel.LastReportedSpeed = ARModel.Speed;
                            AARModel.LastReportedHeading = ARModel.Heading;
                        }
                        else
                        {
                            AARModel.LastReportedLatitude = COIModel.Latitude;
                            AARModel.LastReportedLongitude = COIModel.Longitude;
                            AARModel.LastReportedMMSI = COIModel.MMSI;
                            AARModel.LastReportedCourse = COIModel.Course;
                            AARModel.LastReportedSpeed = COIModel.Speed;
                            AARModel.LastReportedHeading = COIModel.Heading;
                        }
                        AARModel.COITypeId = COIModel.COITypeId;
                        AARModel.NatureOfThreatId = COIModel.NatureOfThreatId;
                        AARModel.COIActivationDateTime = COIModel.COIActivationDate;
                    }
                }
                else
                {
                    SubsequentReport SRModel = GetAllSRs().FindAll(x => x.PRId == AARModel.PRId).OrderByDescending(x => x.COIId).FirstOrDefault();
                    if (SRModel != null)
                    {
                        AARModel.LastReportedLatitude = SRModel.Latitude;
                        AARModel.LastReportedLongitude = SRModel.Longitude;
                        AARModel.LastReportedMMSI = SRModel.MMSI;
                        AARModel.LastReportedCourse = SRModel.Course;
                        AARModel.LastReportedSpeed = SRModel.Speed;
                        AARModel.LastReportedHeading = SRModel.Heading;
                    }
                    else
                    {
                        AARModel.LastReportedLatitude = PRModel.Latitude;
                        AARModel.LastReportedLongitude = PRModel.Longitude;
                        AARModel.LastReportedMMSI = PRModel.MMSI;
                        AARModel.LastReportedCourse = PRModel.Course;
                        AARModel.LastReportedSpeed = PRModel.Speed;
                        AARModel.LastReportedHeading = PRModel.Heading;
                    }
                }
                AARModel.COITypeId = PRModel.COITypeId;
                AARModel.NatureOfThreatId = PRModel.NatureOfThreatId;
            }
            return new PartialViewResult
            {
                ViewName = "AfterActionReportModal",
                ViewData = new ViewDataDictionary<AfterActionReport>(ViewData, AARModel)
            };
        }
        public PartialViewResult OnGetDrawing(Drawing drModel)
        {
            return new PartialViewResult
            {
                ViewName = "DrawingModal"
            };
        }
        public PartialViewResult OnGetNatureOfThreat(string threatId)
        {
            if (Convert.ToInt32(threatId) > 0)
            {
                NatureOfThreat threatModel = GetAllNatureOfThreats().FindAll(x => x.ThreatId == Convert.ToInt32(threatId)).OrderByDescending(x => x.ThreatId).FirstOrDefault();
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
        public PartialViewResult OnGetInfoConLevel(string infoConLevelId)
        {
            using (InfoConLevelService infoConLevelService = new InfoConLevelService())
            {
                if (Convert.ToInt32(infoConLevelId) > 0)
                {
                    InfoConfidenceLevel infoConLevelModel = infoConLevelService.GetById(Convert.ToInt32(infoConLevelId)).Adapt<InfoConfidenceLevel>();
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
        public IActionResult OnGetShip(string MMSI, string IMO)
        {
            try
            {
                Ship shipModel;
                ShipPicture shipPicModel;
                using (ShipService shipService = new ShipService())
                {
                    shipModel = shipService.GetShipDetails(MMSI, IMO);
                    if (shipModel != null)
                    {
                        if (shipModel.IMO != null)
                        {
                            using (ShipPictureService shipPicService = new ShipPictureService())
                            {
                                shipPicModel = shipPicService.GetShipPicByIMO(IMO);
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
                //return new JsonResult(shipModel);
                //return StatusCode(200);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        public IActionResult OnPostStakeholder(Subscriber subModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SubscriberService subscriberService = new SubscriberService())
                    {
                        if (subModel.SubscriberId > 0)
                        {
                            //subModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            //subModel.LastModifiedBy = User.Identity.Name;
                            //subscriberService.Update(subModel);
                            //UserEventLogging(User.Identity.Name, "Stakeholder Updated", Convert.ToInt32(EventTypes.Stakeholder_Updated));
                        }
                        else
                        {
                            //subModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            //subModel.CreatedBy = User.Identity.Name;
                            //subModel = subscriberService.Add(subModel);
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
                throw new Exception("Error Saving Stakeholder. " + Environment.NewLine + ex.Message);
            }
        }
        public async Task<IActionResult> OnPostUser(AppUserViewModel appUserView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _user = await _userManager.FindByEmailAsync(appUserView.Email);
                    if (_user != null)
                        return BadRequest("Email Already Exist");

                    else
                    {
                        var appUser = new AppUser()
                        {
                            UserName = appUserView.Username,
                            Email = appUserView.Email,
                            Subscriber_Id = appUserView.SubscriberId
                        };

                        var createdUser = await _userManager.CreateAsync(appUser, appUserView.Password);
                        if (createdUser.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(appUser, appUserView.Role);
                            return StatusCode(200);
                        }
                    }
                }
                return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving User. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostCOIActivationReport(COI COI)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Subscriber subsModel = GetAllStakeholders().FindAll(x => x.SubscriberId == GetSubscriberID()).OrderByDescending(x => x.SubscriberId).FirstOrDefault(); //subsService.GetById(loggedInSubsId);
                    string generatedCOINo = subsModel.SubscriberName + "-COI-" + 1;
                    using (COIService COIService = new COIService())
                    {
                        if (GetAllCOIs() != null && GetAllCOIs().Count > 0)
                        {
                            COIView lastCOIModel = GetAllCOIs().FindAll(x => x.SubscriberId == GetSubscriberID()).OrderByDescending(x => x.COIId).FirstOrDefault();
                            if (lastCOIModel != null)
                            {
                                generatedCOINo = GetNextCOINumber(lastCOIModel);
                            }
                        }
                        COI.COINumber = generatedCOINo;
                        COI.ActionAddressee = "JMICC";
                        //COI.ActionAddressee = string.Join(",", preliminaryReport.ActionAddresseeArray);
                        COI.InformationAddressee = string.Join(",", COI.InformationAddresseeArray);
                        string[] subscriberArray = null; //COI.InformationAddresseeArray;
                        COI.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        COI.SubscriberId = GetSubscriberID();
                        COI.COIActivationDate = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        COI.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        COI.CreatedBy = User.Identity.Name;
                        COI = null; //COIService.Add(COI);
                        COIView coiViewModel = COI.Adapt<COIView>();

                        using (PreliminaryReportService PRService = new PreliminaryReportService())
                        {
                            PreliminaryReportView PRViewModel = new PreliminaryReportView(); //GetAllPRs().FindAll(x => x.Id == COI.PRId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (PRViewModel != null)
                            {
                                PRViewModel.COIId = coiViewModel.COIId;
                                PRService.Update(0, "", PRViewModel);
                            }
                        }

                        using (SubsequentReportService SRService = new SubsequentReportService())
                        {
                            List<SubsequentReportView> lstSRModel = GetAllSRs().FindAll(x => x.PRId == COI.PRId);

                            foreach (var item in lstSRModel)
                            {
                                if (item != null)
                                {
                                    item.COIId = coiViewModel.COIId;
                                    SRService.Update(0,"",item);
                                }
                            }
                        }
                        _hubContext.Clients.All.PushCOIs(COI);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving COI Activation Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostCOIType(COIType COITypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (COITypeService COITypeService = new COITypeService())
                    {
                        if (COITypeModel.COITypeId > 0)
                        {
                            COITypeModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            COITypeModel.LastModifiedBy = User.Identity.Name;
                            //COITypeService.Update(COITypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            COITypeModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            COITypeModel.CreatedBy = User.Identity.Name;
                            COITypeModel = null;    // COITypeService.Add(COITypeModel);
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
                throw new Exception("Error Saving COI Type. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostCOIStatus(COIStatus COIStatusModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (COIStatusService coiStatusService = new COIStatusService())
                    {
                        if (COIStatusModel.COIStatusId > 0)
                        {
                            COIStatusModel.LastModifiedOn = DateTime.Now;
                            COIStatusModel.LastModifiedBy = User.Identity.Name;
                            //coiStatusService.Update(COIStatusModel);
                            _hubContext.Clients.All.PushCOIStatus(COIStatusModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            COIStatusModel.CreatedOn = DateTime.Now;
                            COIStatusModel.CreatedBy = User.Identity.Name;
                            COIStatusModel = null;    // coiStatusService.Add(COIStatusModel);
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
                throw new Exception("Error Saving COI Status. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostNewsFeed(NewsFeed newsFeedModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validRSS = ValidateFeed(newsFeedModel.NewsSourceUrl);
                    if (validRSS)
                    {
                        using (NewsFeedService newsFeedService = new NewsFeedService())
                        {
                            //int loggedInSubsId = GetSubscriberID();
                            //// NewsFeed newsFeedModel = newsFeedView.Adapt<NewsFeed>();
                            //newsFeedModel.SubscriberId = loggedInSubsId;
                            //newsFeedModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            //newsFeedModel.CreatedBy = User.Identity.Name;
                            //newsFeedModel = newsFeedService.Add(newsFeedModel);
                            //_hubContext.Clients.All.PushNewsFeed(newsFeedModel);
                        }
                    }
                    else
                        return BadRequest();

                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving News. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostNewsFeedType(NewsFeedType newsFeedTypeModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                    {
                        if (newsFeedTypeModel.NewsFeedTypeId > 0)
                        {
                            newsFeedTypeModel.LastModifiedOn = DateTime.Now;
                            newsFeedTypeModel.LastModifiedBy = User.Identity.Name;
                            //newsFeedTypeService.Update(newsFeedTypeModel);
                            _hubContext.Clients.All.PushNewsFeedType(newsFeedTypeModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            newsFeedTypeModel.CreatedOn = DateTime.Now;
                            newsFeedTypeModel.CreatedBy = User.Identity.Name;
                            newsFeedTypeModel = null;    //  newsFeedTypeService.Add(newsFeedTypeModel);
                            _hubContext.Clients.All.PushNewsFeedType(newsFeedTypeModel);
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
                throw new Exception("Error Saving News Feed Type. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostPR(PreliminaryReport preliminaryReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SubscriberService subsService = new SubscriberService())
                    {
                        int loggedInSubsId = GetSubscriberID();
                        Subscriber subsModel = subsService.GetById(loggedInSubsId);
                        string generatedPRNo = subsModel.SubscriberName + "-PR-" + 1;
                        using (PreliminaryReportService PRService = new PreliminaryReportService())
                        {
                            //if (GetAllPRs() != null && GetAllPRs().Count > 0)
                            //{
                            //    PreliminaryReportView lastPRModel = GetAllPRs().FindAll(x => x.SubscriberId == loggedInSubsId).OrderByDescending(x => x.Id).FirstOrDefault();
                            //    if (lastPRModel != null)
                            //    {
                            //        generatedPRNo = GetNextPRNumber(lastPRModel);
                            //    }
                            //}

                            preliminaryReport.PRNumber = generatedPRNo;
                            preliminaryReport.ActionAddressee = "JMICC";
                            //preliminaryReport.ActionAddressee = string.Join(",", preliminaryReport.ActionAddresseeArray);
                            preliminaryReport.InformationAddressee = string.Join(",", preliminaryReport.InformationAddresseeArray);
                            //string[] subscriberArray = preliminaryReport.InformationAddresseeArray;
                            string[] subscriberArray = { "1" };
                            preliminaryReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            preliminaryReport.SubscriberId = loggedInSubsId;
                            preliminaryReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            preliminaryReport.CreatedBy = User.Identity.Name;
                            preliminaryReport = PRService.Add(preliminaryReport);
                            _hubContext.Clients.All.PushPR(preliminaryReport);
                            PreliminaryReportView PRView = preliminaryReport.Adapt<PreliminaryReportView>();
                            foreach (var item in subscriberArray)
                            {
                                using (NotificationService notificationService = new NotificationService())
                                {
                                    //Notifications notificationModel = new Notifications();
                                    //notificationModel.NotificationContent = preliminaryReport.PRNumber;
                                    ////notificationModel.SubscriberId = GetAllStakeholders().FindAll(x => x.SubscriberName == item).FirstOrDefault().SubscriberId;
                                    ////notificationModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                                    ////notificationModel.CreatedBy = User.Identity.Name;
                                    //notificationModel = notificationService.Add(notificationModel);
                                    //_hubContext.Clients.Group((notificationModel.SubscriberId).ToString()).PushNotification(notificationModel);
                                }
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
                throw new Exception("Error Saving Preliminary Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostSR(SubsequentReport subsequentReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SubscriberService subsService = new SubscriberService())
                    {
                        int loggedInSubsId = GetSubscriberID();
                        Subscriber subsModel = subsService.GetById(loggedInSubsId);
                        string generatedSRNo = subsModel.SubscriberName + "-SR-" + 1;
                        using (SubsequentReportService SRService = new SubsequentReportService())
                        {
                            if (GetAllSRs() != null && GetAllSRs().Count > 0)
                            {
                                SubsequentReportView lastSRModel = GetAllSRs().FindAll(x => x.SubscriberId == loggedInSubsId).OrderByDescending(x => x.Id).FirstOrDefault();
                                if (lastSRModel != null)
                                    generatedSRNo = GetNextSRNumber(lastSRModel);
                            }
                            subsequentReport.SRNumber = generatedSRNo;
                            subsequentReport.ActionAddressee = "JMICC";
                            //subsequentReport.ActionAddressee = string.Join(",", subsequentReport.ActionAddresseeArray);
                            subsequentReport.InformationAddressee = string.Join(",", subsequentReport.InformationAddresseeArray);
                            subsequentReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            subsequentReport.SubscriberId = loggedInSubsId;
                            subsequentReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            subsequentReport.CreatedBy = User.Identity.Name;
                            subsequentReport = null; //SRService.Add(subsequentReport);
                            _hubContext.Clients.All.PushSR(subsequentReport);
                        }
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Subsequent Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostLR(LostContactReport LostContactReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (LostContactReportService LRService = new LostContactReportService())
                    {
                        LostContactReport.ActionAddressee = "JMICC";
                        LostContactReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        LostContactReport.SubscriberId = GetSubscriberID();
                        LostContactReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        LostContactReport.CreatedBy = User.Identity.Name;
                        LostContactReport = null;    // LRService.Add(LostContactReport);
                        _hubContext.Clients.All.PushLR(LostContactReport);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Lost Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostDR(DropInfoSharingReport DropInfoSharingReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DropInfoSharingReportService DRService = new DropInfoSharingReportService())
                    {
                        DropInfoSharingReport.ActionAddressee = "JMICC";
                        DropInfoSharingReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        DropInfoSharingReport.SubscriberId = GetSubscriberID();
                        DropInfoSharingReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        DropInfoSharingReport.CreatedBy = User.Identity.Name;
                        DropInfoSharingReport = null;    // DRService.Add(DropInfoSharingReport);
                        _hubContext.Clients.All.PushDR(DropInfoSharingReport);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Drop Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostAR(AmplifyingReport amplifyingReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Subscriber subsModel = GetAllStakeholders().FindAll(x => x.SubscriberId == GetSubscriberID()).OrderByDescending(x => x.SubscriberId).FirstOrDefault(); //subsService.GetById(loggedInSubsId);
                    string generatedARNo = subsModel.SubscriberName + "-AR-" + 1;
                    using (AmplifyingReportService ARService = new AmplifyingReportService())
                    {
                        if (GetAllARs() != null && GetAllARs().Count > 0)
                        {
                            AmplifyingReportView lastARModel = GetAllARs().FindAll(x => x.SubscriberId == GetSubscriberID()).OrderByDescending(x => x.ARId).FirstOrDefault();
                            if (lastARModel != null)
                            {
                                generatedARNo = GetNextARNumber(lastARModel);
                            }
                        }
                        amplifyingReport.ARNumber = generatedARNo;
                        amplifyingReport.ActionAddressee = "JMICC";
                        amplifyingReport.InformationAddressee = string.Join(",", amplifyingReport.InformationAddresseeArray);
                        amplifyingReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        amplifyingReport.SubscriberId = GetSubscriberID();
                        amplifyingReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        amplifyingReport.CreatedBy = User.Identity.Name;
                        amplifyingReport = null; //ARService.Add(amplifyingReport);
                        _hubContext.Clients.All.PushAR(amplifyingReport);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Form");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Amplifying Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostAAR(AfterActionReport afterActionReport)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (AfterActionReportService AARService = new AfterActionReportService())
                    {
                        afterActionReport.AddressedTo = "JMICC";
                        afterActionReport.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        afterActionReport.SubscriberId = GetSubscriberID();
                        afterActionReport.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        afterActionReport.CreatedBy = User.Identity.Name;
                        afterActionReport = null;    // AARService.Add(afterActionReport);
                        _hubContext.Clients.All.PushAAR(afterActionReport);
                        AfterActionReportView AARView = afterActionReport.Adapt<AfterActionReportView>();
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving After Action Report. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostNatureOfThreat(NatureOfThreat threatModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (NatureOfThreatService threatService = new NatureOfThreatService())
                    {
                        if (threatModel.ThreatId > 0)
                        {
                            threatModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            threatModel.LastModifiedBy = User.Identity.Name;
                            //threatService.Update(threatModel);
                            //UserEventLogging(User.Identity.Name, "Threat Level Updated", Convert.ToInt32(EventTypes.Threat_Level_Updated));
                        }
                        else
                        {
                            threatModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                            threatModel.CreatedBy = User.Identity.Name;
                            threatModel = null;    // threatService.Add(threatModel);
                            //UserEventLogging(User.Identity.Name, "Threat Level Created", Convert.ToInt32(EventTypes.Threat_Level_Created));
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
                throw new Exception("Error Saving Nature Of Threat. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostInfoConLevel(InfoConfidenceLevel infoConLevelModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                    {
                        if (infoConLevelModel.InfoConfidenceLevelId > 0)
                        {
                            infoConLevelModel.LastModifiedOn = DateTime.Now;
                            infoConLevelModel.LastModifiedBy = User.Identity.Name;
                            //infoConLevelService.Update(infoConLevelModel);
                            _hubContext.Clients.All.PushInfoConLevel(infoConLevelModel);
                            //UserEventLogging(User.Identity.Name, "COI Type Updated", Convert.ToInt32(EventTypes.COI_Type_Updated));
                        }
                        else
                        {
                            infoConLevelModel.CreatedOn = DateTime.Now;
                            infoConLevelModel.CreatedBy = User.Identity.Name;
                            infoConLevelModel = null;    // infoConLevelService.Add(infoConLevelModel);
                            _hubContext.Clients.All.PushInfoConLevel(infoConLevelModel);
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
                throw new Exception("Error Saving Information Confidence Level. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostDrawing(DrawingViewModel drViewModel)
        {
            try
            {
                if (drViewModel != null && drViewModel.Drawing != null && !string.IsNullOrWhiteSpace(drViewModel.Drawing.DrawingSource))
                {
                    DrawingModel drModel = new DrawingModel();
                    PolygonDrawingModel polygonDrawingModel = new PolygonDrawingModel();
                    if (drViewModel.Drawing.ShapeId == 2 || drViewModel.Drawing.ShapeId == 3 || drViewModel.Drawing.ShapeId == 6)
                        polygonDrawingModel = JsonConvert.DeserializeObject<PolygonDrawingModel>(drViewModel.Drawing.DrawingSource);
                    else
                        drModel = JsonConvert.DeserializeObject<DrawingModel>(drViewModel.Drawing.DrawingSource);

                    using (DrawingService drService = new DrawingService())
                    {
                        Drawing drawing = drViewModel.Drawing.Adapt<Drawing>();
                        drawing.SubscriberId = GetSubscriberID();

                        if (drModel.Type != null)
                            drawing.DrawingType = drModel.Type;
                        else
                            drawing.DrawingType = polygonDrawingModel.Type;

                        drawing.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        drawing.CreatedBy = User.Identity.Name;
                        drawing = drService.Add(drawing);

                        if (drawing.ShapeId != 4)
                        {
                            using (DrawingCoordinateService drCoordService = new DrawingCoordinateService())
                            {
                                DrawingCoordinate dCoord = new DrawingCoordinate();
                                dCoord.DrawingId = drawing.DrawingId;

                                if (drModel.Geometry != null && drModel.Geometry.Type != "Polygon")
                                {
                                    foreach (var item in drModel.Geometry.Coordinates)
                                    {
                                        dCoord.DrawingCoordinateLongitude = item[0];
                                        dCoord.DrawingCoordinateLatitude = item[1];
                                        dCoord.CreatedOn = DateTime.Now;
                                        dCoord.CreatedBy = User.Identity.Name;
                                        dCoord = drCoordService.Add(dCoord);
                                    }
                                }
                                else
                                {
                                    foreach (var item1 in polygonDrawingModel.Geometry.Coordinates)
                                    {
                                        //PolygonDrawingModel
                                        foreach (var item in polygonDrawingModel.Geometry.Coordinates[0])
                                        {
                                            dCoord.DrawingCoordinateLongitude = Convert.ToDecimal(item[0]);
                                            dCoord.DrawingCoordinateLatitude = Convert.ToDecimal(item[1]);
                                            dCoord.CreatedOn = DateTime.Now;
                                            dCoord.CreatedBy = User.Identity.Name;
                                            dCoord = drCoordService.Add(dCoord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Drawing. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnPostNotification(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (NotificationService notiService = new NotificationService())
                    {
                        //Notifications notification = GetAllNotifications().FindAll(x => x.Id == Id).OrderByDescending(x => x.Id).FirstOrDefault(); //notiService.GetById(Id);
                        //notification.IsRead = true;
                        //notification.LastModifiedBy = User.Identity.Name;
                        //notification.ReadBy = User.Identity.Name;
                        //notification.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        //notification.ReadOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + GetSubscriberID()));
                        //notiService.Update(notification);
                    }
                    return StatusCode(200);
                }
                else
                    return BadRequest("Invalid Data");
            }
            catch (Exception ex)
            {
                throw new Exception("Error Saving Notification. " + Environment.NewLine + ex.Message);
            }
        }
        
        public IActionResult OnGetDeleteStakeholder(string subsId)
        {
            try
            {
                using (SubscriberService subscriberService = new SubscriberService())
                {
                    if (Convert.ToInt32(subsId) > 0)
                    {
                        subscriberService.Delete(Convert.ToInt32(subsId));
                        //UserEventLogging(User.Identity.Name, "Stakeholder Deleted", Convert.ToInt32(EventTypes.Stakeholder_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting Stakeholder. " + Environment.NewLine + ex.Message);
            }
        }
        public async Task<IActionResult> OnGetDeleteUser(string Id)
        {
            try
            {
                var appuser = await _userManager.FindByIdAsync(Id);
                await _userManager.DeleteAsync(appuser);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting User. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteCOIType(string coiTypeId)
        {
            try
            {
                using (COITypeService COITypeService = new COITypeService())
                {
                    if (Convert.ToInt32(coiTypeId) > 0)
                    {
                        COITypeService.Delete(Convert.ToInt32(coiTypeId));
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting COI Type. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteCOIStatus(string coiStatusId)
        {
            try
            {
                using (COIStatusService coiStatusService = new COIStatusService())
                {
                    if (Convert.ToInt32(coiStatusId) > 0)
                    {
                        coiStatusService.Delete(Convert.ToInt32(coiStatusId));
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting COI Status. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteNewsFeedType(string newsFeedTypeId)
        {
            try
            {
                using (NewsFeedTypeService newsFeedTypeService = new NewsFeedTypeService())
                {
                    if (Convert.ToInt32(newsFeedTypeId) > 0)
                    {
                        newsFeedTypeService.Delete(Convert.ToInt32(newsFeedTypeId));
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting News Feed Type. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteNatureOfThreat(string threatId)
        {
            try
            {
                using (NatureOfThreatService threatService = new NatureOfThreatService())
                {
                    if (Convert.ToInt32(threatId) > 0)
                    {
                        threatService.Delete(Convert.ToInt32(threatId));
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting Nature Of Threat. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteInfoConLevel(string infoConLevelId)
        {
            try
            {
                using (InfoConLevelService infoConLevelService = new InfoConLevelService())
                {
                    if (Convert.ToInt32(infoConLevelId) > 0)
                    {
                        infoConLevelService.Delete(Convert.ToInt32(infoConLevelId));
                        //UserEventLogging(User.Identity.Name, "COI Type Deleted", Convert.ToInt32(EventTypes.COI_Type_Deleted));
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting Information Confidence Level. " + Environment.NewLine + ex.Message);
            }
        }
        public IActionResult OnGetDeleteDrawing(int drID)
        {
            try
            {
                using (DrawingService drService = new DrawingService())
                {
                    using (DrawingCoordinateService drCoordService = new DrawingCoordinateService())
                    {
                        List<DrawingCoordinate> lstDrawingCoords = GetAllDrawingCoordinates().FindAll(x => x.DrawingId == drID);
                        if (lstDrawingCoords != null && lstDrawingCoords.Count > 0)
                        {
                            foreach (var item in lstDrawingCoords)
                            {
                                drCoordService.Delete(item.DrawingCoordinateId);
                            }
                        }
                    }
                    drService.Delete(drID);
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleting Drawing. " + Environment.NewLine + ex.Message);
            }
        }
        #endregion
    }
}
