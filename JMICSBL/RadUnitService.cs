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
    public class RadUnitService : BaseService, IDisposable
    {
        public RadUnit GetById(int radUnitId)
        {
            try
            {
                if (MemCache.IsIncache("AllRadUnitKey"))
                {
                    return MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey").Where<RadUnit>(x => x.RadUnitId == radUnitId).FirstOrDefault();
                }
                using (RadUnitRepository radUnitRepo = new RadUnitRepository())
                {
                    RadUnit radUnitModel = new RadUnit();
                    {
                        radUnitModel = radUnitRepo.Get<RadUnit>(radUnitId);
                        return radUnitModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RadUnit Add(RadUnit radUnitModel)
        {
            try
            {
                using (RadUnitRepository radUnitRepo = new RadUnitRepository())
                {
                    if (radUnitModel != null)
                    {
                        var rowId = radUnitRepo.Insert<RadUnit>(radUnitModel);
                        radUnitModel.RadUnitId = rowId;
                    }
                    if (MemCache.IsIncache("AllRadUnitKey"))
                        MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey").Add(radUnitModel);
                    else
                    {
                        List<RadUnit> radUnits = new List<RadUnit>();
                        radUnits.Add(radUnitModel);
                        MemCache.AddToCache("AllRadUnitKey", radUnits);
                    }
                    return radUnitModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(RadUnit radUnitModel)
        {
            try
            {
                using (RadUnitRepository radUnitRepo = new RadUnitRepository())
                {
                    if (MemCache.IsIncache("AllRadUnitKey"))
                    {
                        List<RadUnit> radUnits = MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey");
                        if (radUnits.Count > 0)
                            radUnits.Remove(radUnits.Find(x => x.RadUnitId == radUnitModel.RadUnitId));
                    }

                    radUnitRepo.Update<RadUnit>(radUnitModel);
                    if (MemCache.IsIncache("AllRadUnitKey"))
                        MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey").Add(radUnitModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int radUnitId)
        {
            try
            {
                using (RadUnitRepository radUnitRepo = new RadUnitRepository())
                {
                    var radUnitExisting = radUnitRepo.Get<RadUnit>(radUnitId);
                    if (radUnitExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        radUnitRepo.Delete<RadUnit>(radUnitId);
                        if (MemCache.IsIncache("AllRadUnitKey"))
                            MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey").Remove(radUnitExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RadUnit> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<RadUnit> radUnits = new List<RadUnit>();
                if (MemCache.IsIncache("AllRadUnitKey"))
                {
                    return MemCache.GetFromCache<List<RadUnit>>("AllRadUnitKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");
                    var parameters = this.ParseParameters(dic);
                    using (RadUnitRepository radUnitRepo = new RadUnitRepository())
                    {
                        radUnits = radUnitRepo.GetListPaged<RadUnit>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();

                        MemCache.AddToCache("AllRadUnitKey", radUnits);
                        return radUnits;
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
            string radunitid;

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
            if (dic.TryGetValue("radunitid", out radunitid))
            {
                dicAux.Add("@radunitid", radunitid);
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
        // ~RadUnitService()
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
