using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class CityService : BaseService, IDisposable
    {
        public City GetById(int cityId)
        {
            try
            {
                if (MemCache.IsIncache("AllCitysKey"))
                {
                    return MemCache.GetFromCache<List<City>>("AllCitysKey").Where<City>(x => x.CityId == cityId).FirstOrDefault();
                }
                using (CityRepository cityRepo = new CityRepository())
                {
                    City cityModel = new City();
                    {
                        cityModel = cityRepo.Get<City>(cityId);
                        return cityModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public City Add(City cityModel)
        {
            try
            {
                using (CityRepository cityRepo = new CityRepository())
                {
                    if (cityModel != null)
                    {
                        var rowId = cityRepo.Insert<City>(cityModel);
                        cityModel.CityId = rowId;
                    }
                    if (MemCache.IsIncache("AllCitysKey"))
                        MemCache.GetFromCache<List<City>>("AllCitysKey").Add(cityModel);
                    else
                    {
                        List<City> cities = new List<City>();
                        cities.Add(cityModel);
                        MemCache.AddToCache("AllCitysKey", cities);
                    }
                    return cityModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(City cityModel)
        {
            try
            {
                using (CityRepository cityRepo = new CityRepository())
                {
                    var cityExisting = cityRepo.Get<City>(cityModel.CityId);
                    if (Common.AreObjectsEqual(cityModel, cityExisting))
                    {
                        return false;
                    }
                    else
                    {
                        cityRepo.Update<City>(cityModel);
                        if (MemCache.IsIncache("AllCitysKey"))
                        {
                            if (MemCache.GetFromCache<List<City>>("AllCitysKey").Remove(cityExisting))
                                MemCache.GetFromCache<List<City>>("AllCitysKey").Add(cityModel);
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int cityId)
        {
            try
            {
                using (CityRepository cityRepo = new CityRepository())
                {
                    var cityExisting = cityRepo.Get<City>(cityId);
                    if (cityExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        cityRepo.Delete<City>(cityId);
                        if (MemCache.IsIncache("AllCitysKey"))
                            MemCache.GetFromCache<List<City>>("AllCitysKey").Remove(cityExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<City> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<City> lstCity = new List<City>();
                if (MemCache.IsIncache("AllCitysKey"))
                {
                    return MemCache.GetFromCache<List<City>>("AllCitysKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "City_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (CityRepository cityRepo = new CityRepository())
                    {
                        lstCity = cityRepo.GetListPaged<City>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllCitysKey", lstCity);
                        return lstCity;
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
            string cityid;

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
            if (dic.TryGetValue("cityid", out cityid))
            {
                dicAux.Add("@cityid", cityid);
            }
            return dicAux;
        }
        public City GetSubscriberCity(string subsCity, Dictionary<string, string> dic = null)
        {
            try
            {
                if (dic == null)
                    dic = new Dictionary<string, string>();

                dic.Add("orderby", "City_Name");
                dic.Add("offset", "1");
                dic.Add("limit", "200");

                var parameters = this.ParseParameters(dic);
                using (SubscriberRepository subsRepo = new SubscriberRepository())
                {
                    string query = "WHERE 1 = 1";
                    if (!string.IsNullOrWhiteSpace(subsCity))
                        query += " AND City_Name = '" + subsCity + "'";

                    City city = subsRepo.GetList<City>(query, parameters)?.ToList().OrderByDescending(x => x.Id).FirstOrDefault();
                    return city;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        // ~CityService()
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
