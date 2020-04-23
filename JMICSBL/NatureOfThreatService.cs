using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Security;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class NatureOfThreatService : BaseService, IDisposable
    {
        IRepository<NatureOfThreat> nOThreatRepository = new NatureOfThreatRepository();
        public NatureOfThreat GetById(int NatureOfThreatId)
        {
            try
            {
                if (MemCache.IsIncache("AllNOTKey"))
                    return MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey").Where<NatureOfThreat>(x => x.ThreatId == NatureOfThreatId).FirstOrDefault();
                
                using (NatureOfThreatRepository nOThreatRepo = new NatureOfThreatRepository())
                {
                    NatureOfThreat nOThreat = new NatureOfThreat();
                    {
                        nOThreat = nOThreatRepo.Get<NatureOfThreat>(NatureOfThreatId);
                        return nOThreat;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NatureOfThreat> GetFilteredNOT(string Name)
        {
            try
            {
                using (NatureOfThreatRepository NOTRepo = new NatureOfThreatRepository())
                {
                    List<NatureOfThreat> NOTList = NOTRepo.GetList<NatureOfThreat>("WHERE Threat_Name = '" + Name + "' ")?.ToList();
                    return NOTList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NatureOfThreat Add(int SubscriberId, string UserName, NatureOfThreat NatureOfThreatModel)
        {
            try
            {
                if (NatureOfThreatModel == null)
                    throw new Exception("Nature Of Threat model is null");

                using (NatureOfThreatRepository NatureOfThreatRepo = new NatureOfThreatRepository())
                {
                    NatureOfThreatModel.CreatedBy = UserName;
                    NatureOfThreatModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    
                    var rowId = NatureOfThreatRepo.Insert<NatureOfThreat>(NatureOfThreatModel);
                    NatureOfThreatModel.ThreatId = rowId;

                    if (MemCache.IsIncache("AllNOTKey"))
                        MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey").Add(NatureOfThreatModel);
                    else
                    {
                        List<NatureOfThreat> natureOfThreats = new List<NatureOfThreat>();
                        natureOfThreats.Add(NatureOfThreatModel);
                        MemCache.AddToCache("AllNOTKey", natureOfThreats);
                    }
                    return NatureOfThreatModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, NatureOfThreat NatureOfThreatModel)
        {
            try
            {
                using (NatureOfThreatRepository NatureOfThreatRepo = new NatureOfThreatRepository())
                {
                    if (MemCache.IsIncache("AllNOTKey"))
                    {
                        List<NatureOfThreat> natureOfThreats = MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey");
                        if (natureOfThreats.Count > 0)
                            natureOfThreats.Remove(natureOfThreats.Find(x => x.ThreatId == NatureOfThreatModel.ThreatId));
                    }

                    NatureOfThreatModel.LastModifiedBy = UserName;
                    NatureOfThreatModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    NatureOfThreatRepo.Update<NatureOfThreat>(NatureOfThreatModel);
                    if (MemCache.IsIncache("AllNOTKey"))
                        MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey").Add(NatureOfThreatModel);
                    return true;
                   }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int NatureOfThreatId)
        {
            try
            {
                using (NatureOfThreatRepository NatureOfThreatRepo = new NatureOfThreatRepository())
                {
                    var NatureOfThreatExisting = NatureOfThreatRepo.Get<NatureOfThreat>(NatureOfThreatId);
                    if (NatureOfThreatExisting == null)
                        return false;
                    
                    else
                    {
                        NatureOfThreatRepo.Delete<NatureOfThreat>(NatureOfThreatId);
                        if (MemCache.IsIncache("AllNOTKey"))
                            MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey").Remove(MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey").Where(x => x.ThreatId == NatureOfThreatExisting.ThreatId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NatureOfThreat> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<NatureOfThreat> lstNOT = new List<NatureOfThreat>();
                if (MemCache.IsIncache("AllNOTKey"))
                {
                    return MemCache.GetFromCache<List<NatureOfThreat>>("AllNOTKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Threat_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (NatureOfThreatRepository threatRepo = new NatureOfThreatRepository())
                    {
                        lstNOT = threatRepo.GetListPaged<NatureOfThreat>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNOTKey", lstNOT);
                        return lstNOT;
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
            string threatid;
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
            if (dic.TryGetValue("threatid", out threatid))
            {
                dicAux.Add("@threatid", threatid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@Threat_Name", keyword);
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
        // ~NatureOfThreatService()
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
