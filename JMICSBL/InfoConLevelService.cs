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
    public class InfoConLevelService : BaseService, IDisposable
    {
        IRepository<InfoConfidenceLevel> InfoConLevelRepository = new InfoConLevelRepository();
        public InfoConfidenceLevel GetById(int infoConLevelId)
        {
            try
            {
                if (MemCache.IsIncache("AllInfoConLevelsKey"))
                {
                    return MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey").Where<InfoConfidenceLevel>(x => x.InfoConfidenceLevelId == infoConLevelId).FirstOrDefault();
                }
                using (InfoConLevelRepository infoConLevelRepo = new InfoConLevelRepository())
                {
                    InfoConfidenceLevel infoConLevelModel = new InfoConfidenceLevel();
                    {
                        infoConLevelModel = infoConLevelRepo.Get<InfoConfidenceLevel>(infoConLevelId);
                        return infoConLevelModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InfoConfidenceLevel Add(int SubscriberId, string UserName, InfoConfidenceLevel InfoConLevelModel)
        {
            try
            {
                if (InfoConLevelModel == null)
                    throw new Exception("Information Confidence Level model is null");

                using (InfoConLevelRepository infoConLevelRepo = new InfoConLevelRepository())
                {
                    InfoConLevelModel.CreatedBy = UserName;
                    InfoConLevelModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    var rowId = infoConLevelRepo.Insert<InfoConfidenceLevel>(InfoConLevelModel);
                    InfoConLevelModel.InfoConfidenceLevelId = rowId;
                    
                    if (MemCache.IsIncache("AllInfoConLevelsKey"))
                        MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey").Add(InfoConLevelModel);
                    else
                    {
                        List<InfoConfidenceLevel> InfoConLevel= new List<InfoConfidenceLevel>();
                        InfoConLevel.Add(InfoConLevelModel);
                        MemCache.AddToCache("AllInfoConLevelsKey", InfoConLevel);
                    }
                    return InfoConLevelModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, InfoConfidenceLevel InfoConLevelModel)
        {
            try
            {
                using (InfoConLevelRepository infoConLevelRepo = new InfoConLevelRepository())
                {
                    if (MemCache.IsIncache("AllInfoConLevelsKey"))
                    {
                        List<InfoConfidenceLevel> infoConLevel = MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey");
                        if (infoConLevel.Count > 0)
                            infoConLevel.Remove(infoConLevel.Find(x => x.InfoConfidenceLevelId == InfoConLevelModel.InfoConfidenceLevelId));
                    }
                    InfoConLevelModel.LastModifiedBy = UserName;
                    InfoConLevelModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    infoConLevelRepo.Update<InfoConfidenceLevel>(InfoConLevelModel);
                    if (MemCache.IsIncache("AllInfoConLevelsKey"))
                        MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey").Add(InfoConLevelModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int infoConLevelId)
        {
            try
            {
                using (InfoConLevelRepository infoConLevelRepo = new InfoConLevelRepository())
                {
                    var infoConLevelExisting = infoConLevelRepo.Get<InfoConfidenceLevel>(infoConLevelId);
                    if (infoConLevelExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        infoConLevelRepo.Delete<InfoConfidenceLevel>(infoConLevelId);
                        if (MemCache.IsIncache("AllInfoConLevelsKey"))
                            MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey").Remove(MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey").Where(x => x.InfoConfidenceLevelId == infoConLevelExisting.InfoConfidenceLevelId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<InfoConfidenceLevel> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<InfoConfidenceLevel> lstInfoConLevel = new List<InfoConfidenceLevel>();
                if (MemCache.IsIncache("AllInfoConLevelsKey"))
                {
                    return MemCache.GetFromCache<List<InfoConfidenceLevel>>("AllInfoConLevelsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Info_Confidence_Level_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (InfoConLevelRepository infoConLevelRepo = new InfoConLevelRepository())
                    {
                        lstInfoConLevel = infoConLevelRepo.GetListPaged<InfoConfidenceLevel>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllInfoConLevelsKey", lstInfoConLevel);
                        return lstInfoConLevel;
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
            string infoConLevelid;
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
            if (dic.TryGetValue("infoConLevelid", out infoConLevelid))
            {
                dicAux.Add("@infoConLevelid", infoConLevelid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@Info_Confidence_Level_Name", keyword);
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
        // ~InfoConLevelService()
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
