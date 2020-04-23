using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class COIStatusService : BaseService, IDisposable
    {
        IRepository<COIStatus> COIStatusRepository = new COIStatusRepository();
        public COIStatus GetById(int COIStatusId)
        {
            try
            {
                if (MemCache.IsIncache("AllCOIStatussKey"))
                {
                    return MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey").Where<COIStatus>(x => x.COIStatusId == COIStatusId).FirstOrDefault();
                }
                using (COIStatusRepository coiStatusRepo = new COIStatusRepository())
                {
                    COIStatus COIStatusModel = new COIStatus();
                    {
                        COIStatusModel = coiStatusRepo.Get<COIStatus>(COIStatusId);
                        return COIStatusModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public COIStatus Add(int SubscriberId, string UserName, COIStatus COIStatusModel)
        {
            try
            {
                if (COIStatusModel == null)
                    throw new Exception("COI Status model is null");

                using (COIStatusRepository coiStatusRepo = new COIStatusRepository())
                {
                    COIStatusModel.CreatedBy = UserName;
                    COIStatusModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    var rowId = coiStatusRepo.Insert<COIStatus>(COIStatusModel);
                    COIStatusModel.COIStatusId = rowId;

                    if (MemCache.IsIncache("AllCOIStatussKey"))
                        MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey").Add(COIStatusModel);
                    else
                    {
                        List<COIStatus> cOIStatuses = new List<COIStatus>();
                        cOIStatuses.Add(COIStatusModel);
                        MemCache.AddToCache("AllCOIStatussKey", cOIStatuses);
                    }
                    return COIStatusModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, COIStatus COIStatusModel)
        {
            try
            {
                using (COIStatusRepository coiStatusRepo = new COIStatusRepository())
                {
                    if (MemCache.IsIncache("AllCOIStatussKey"))
                    {
                        List<COIStatus> cOIStatuses = MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey");
                        if (cOIStatuses.Count > 0)
                            cOIStatuses.Remove(cOIStatuses.Find(x => x.COIStatusId == COIStatusModel.COIStatusId));
                    }

                    COIStatusModel.LastModifiedBy = UserName;
                    COIStatusModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    coiStatusRepo.Update<COIStatus>(COIStatusModel);
                    if (MemCache.IsIncache("AllCOIStatussKey"))
                        MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey").Add(COIStatusModel);
                    return true;
              }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int COIStatusId)
        {
            try
            {
                using (COIStatusRepository coiStatusRepo = new COIStatusRepository())
                {
                    var COIStatusExisting = coiStatusRepo.Get<COIStatus>(COIStatusId);
                    if (COIStatusExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        coiStatusRepo.Delete<COIStatus>(COIStatusId);
                        if (MemCache.IsIncache("AllCOIStatussKey"))
                            MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey").Remove(MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey").Where(x => x.COIStatusId == COIStatusExisting.COIStatusId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIStatus> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<COIStatus> lstCOIStatus = new List<COIStatus>();
                if (MemCache.IsIncache("AllCOIStatussKey"))
                {
                    return MemCache.GetFromCache<List<COIStatus>>("AllCOIStatussKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "COI_Status");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (COIStatusRepository coiStatusRepo = new COIStatusRepository())
                    {
                        lstCOIStatus = coiStatusRepo.GetListPaged<COIStatus>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllCOIStatussKey", lstCOIStatus);
                        return lstCOIStatus;
                    }
                }

            }
            catch (Exception ex)
            {
                //_trace.Error("Error Retrieving Data", exception: ex);
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
            string COIStatusid;
            string keyword;

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
            if (dic.TryGetValue("COIStatusid", out COIStatusid))
            {
                dicAux.Add("@COIStatusid", COIStatusid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@COI_Status", keyword);
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
        // ~COIStatusService()
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
