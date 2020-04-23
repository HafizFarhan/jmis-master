using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Responses;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class WeatherDetailService : BaseService, IDisposable
    {
        public WeatherDetail GetById(int Weather_Id)
        {
            try
            {
                //if (MemCache.IsIncache("AllWeatherDetailKey"))
                //{
                //    return MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey").Where<WeatherDetail>(x => x.WeatherId == Weather_Id).FirstOrDefault();
                //}

                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    WeatherDetail weatherModel = new WeatherDetail();
                    {
                        weatherModel = weatherRepo.Get<WeatherDetail>(Weather_Id);
                        return weatherModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public WeatherDetail Get10MinPrevDetails(int SubscriberId, int CityId, string TenMinPreviousDateTime)
        {
            //WeatherDetail WdModel = new WeatherDetail();
            try
            {
                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    DateTime.TryParse(TenMinPreviousDateTime, out DateTime previousDate);
                    DateTime tenMinPreviousDate = previousDate;

                    WeatherDetail WdModel = weatherRepo.GetList<WeatherDetail>(" WHERE City_Id = '" + CityId + "' AND Created_On > '" + tenMinPreviousDate + "' AND Created_On < '" + Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId)) + "'").OrderByDescending(x => x.WeatherId).FirstOrDefault();
                    //if (WdModelList != null && WdModelList.Count > 0)
                    //{
                    //    WdModel = WdModelList.First<WeatherDetail>();
                    //}
                    return WdModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public WeatherDetail GetLastUpdatedDetails(int CityId)
        {
            try
            {
                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    WeatherDetail WdModel = weatherRepo.GetList<WeatherDetail>(" WHERE City_Id = '" + CityId + "' ").OrderByDescending(x => x.WeatherId).FirstOrDefault();
                    return WdModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public WeatherDetail Add(int SubscriberId, string UserName, WeatherDetail WeatherModel, IRestResponse<WeatherResponse> response)
        {
            try
            {
                if (WeatherModel == null)
                    throw new Exception("Weather model is null");

                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    WeatherModel.Temp = (decimal)(response.Data.Main.Temp - 273.15);
                    WeatherModel.TempMax = (decimal)(response.Data.Main.TempMax - 273.15);
                    WeatherModel.TempMin = (decimal)(response.Data.Main.TempMin - 273.15);
                    WeatherModel.Pressure = response.Data.Main.Pressure;
                    WeatherModel.Humidity = response.Data.Main.Humidity;

                    WeatherModel.Main = response.Data.Weather[0].Main;
                    WeatherModel.Description = response.Data.Weather[0].Description;

                    WeatherModel.Speed = (decimal)response.Data.Wind.Speed;
                    WeatherModel.Deg = response.Data.Wind.Deg;

                    WeatherModel.Sunrise = Common.UnixTimeToDateTime(response.Data.Sys.Sunrise);
                    WeatherModel.Sunset = Common.UnixTimeToDateTime(response.Data.Sys.Sunset);
                    WeatherModel.Country = response.Data.Sys.Country;

                    WeatherModel.Dt = Common.UnixTimeToDateTime(response.Data.Dt);
                    WeatherModel.Timezone = Common.UnixTimeToDateTime(response.Data.Timezone).ToString();
                    WeatherModel.CityId = unchecked((int)response.Data.Id);
                    WeatherModel.Name = response.Data.Name;

                    //DateTime.TryParse(DateTime.Now.ToString(), out DateTime Date);
                    WeatherModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    WeatherModel.CreatedBy = UserName;



                    var rowId = weatherRepo.Insert<WeatherDetail>(WeatherModel);
                    WeatherModel.WeatherId = rowId;
                    
                    if (MemCache.IsIncache("AllWeatherDetailKey"))
                        MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey").Add(WeatherModel);
                    else
                    {
                        List<WeatherDetail> weatherDetails = new List<WeatherDetail>();
                        weatherDetails.Add(WeatherModel);
                        MemCache.AddToCache("AllWeatherDetailKey", weatherDetails);
                    }
                    return WeatherModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(WeatherDetail weatherModel)
        {
            try
            {
                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    if (MemCache.IsIncache("AllWeatherDetailKey"))
                    {
                        List<WeatherDetail> weatherDetails = MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey");
                        if (weatherDetails.Count > 0)
                            weatherDetails.Remove(weatherDetails.Find(x => x.WeatherId == weatherModel.WeatherId));
                    }

                    weatherRepo.Update<WeatherDetail>(weatherModel);
                    if (MemCache.IsIncache("AllWeatherDetailKey"))
                        MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey").Add(weatherModel);
                    return true;
                   }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int Weather_Id)
        {
            try
            {
                using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                {
                    var weatherExisting = weatherRepo.Get<WeatherDetail>(Weather_Id);
                    if (weatherExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        weatherRepo.Delete<WeatherDetail>(Weather_Id);
                        if (MemCache.IsIncache("AllWeatherDetailKey"))
                            MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey").Remove(weatherExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<WeatherDetail> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<WeatherDetail> lstWeather = new List<WeatherDetail>();
                if (MemCache.IsIncache("AllWeatherDetailKey"))
                {
                    return MemCache.GetFromCache<List<WeatherDetail>>("AllWeatherDetailKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "City_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");
                    var parameters = this.ParseParameters(dic);
                    using (WeatherDetailRepository weatherRepo = new WeatherDetailRepository())
                    {
                       lstWeather = weatherRepo.GetListPaged<WeatherDetail>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllWeatherDetailKey", lstWeather);
                        return lstWeather;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();

            string offset;
            string limit;
            string orderby;
            string sort;
            string keyfilter;
            string weatherid;

            if (dic.TryGetValue("offset", out offset))
            {
                dicAux.Add("@offset", offset);
            }

            if (dic.TryGetValue("limit", out limit))
            {
                dicAux.Add("@limit", limit);
            }

            if (dic.TryGetValue("orderby", out orderby))
            {
                dicAux.Add("@orderby", orderby);
            }

            if (dic.TryGetValue("sortorder", out sort))
            {
                dicAux.Add("@sortorder", sort);
            }
            if (dic.TryGetValue("keyfilter", out keyfilter))
            {
                dicAux.Add("@keyfilter", keyfilter);
            }
            if (dic.TryGetValue("weatherid", out weatherid))
            {
                dicAux.Add("@weatherid", weatherid);
            }
            return dicAux;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WeatherDetailService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
