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
    public class NewsStatusService : BaseService, IDisposable
    {
        IRepository<NewsStatus> NewsStatusRepository = new NewsStatusRepository();
        public NewsStatus GetById(int NewsStatusId)
        {
            try
            {
                if (MemCache.IsIncache("AllNewsStatusKey"))
                {
                    return MemCache.GetFromCache<List<NewsStatus>>("AllNewsStatusKey").Where<NewsStatus>(x => x.NewsStatusId == NewsStatusId).FirstOrDefault();
                }
                using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                {
                    NewsStatus NewsStatusModel = new NewsStatus();
                    {
                        NewsStatusModel = newsStatusRepo.Get<NewsStatus>(NewsStatusId);
                        return NewsStatusModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsStatus> GetFilteredNewsStatuses(string Name)
        {
            try
            {
                using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                {
                    List<NewsStatus> NewsStatusList = newsStatusRepo.GetList<NewsStatus>("WHERE News_Status = '" + Name + "' ")?.ToList();
                    return NewsStatusList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NewsStatus Add(NewsStatus NewsStatusModel)
        {
            try
            {
                using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                {
                    if (NewsStatusModel != null)
                    {
                        var rowId = newsStatusRepo.Insert<NewsStatus>(NewsStatusModel);
                        NewsStatusModel.NewsStatusId = rowId;
                    }
                    if (MemCache.IsIncache("AllNewsStatusKey"))
                        MemCache.GetFromCache<List<NewsStatus>>("AllNewsStatusKey").Add(NewsStatusModel);
                    else
                    {
                        List<NewsStatus> newsStatuses = new List<NewsStatus>();
                        newsStatuses.Add(NewsStatusModel);
                        MemCache.AddToCache("AllNewsStatusKey", newsStatuses);
                    }
                    return NewsStatusModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(NewsStatus NewsStatusModel)
        {
            try
            {
                using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                {
                    var NewsStatusExisting = newsStatusRepo.Get<NewsStatus>(NewsStatusModel.NewsStatusId);
                    if (Common.AreObjectsEqual(NewsStatusModel, NewsStatusExisting))
                    {
                        return false;
                    }
                    else
                    {
                        newsStatusRepo.Update<NewsStatus>(NewsStatusModel);
                        if (MemCache.IsIncache("AllNewsStatusKey"))
                        {
                            if (MemCache.GetFromCache<List<NewsStatus>>("AllNewsStatusKey").Remove(NewsStatusExisting))
                                MemCache.GetFromCache<List<NewsStatus>>("AllNewsStatusKey").Add(NewsStatusModel);
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
        public bool Delete(int NewsStatusId)
        {
            try
            {
                using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                {
                    var NewsStatusExisting = newsStatusRepo.Get<NewsStatus>(NewsStatusId);
                    if (NewsStatusExisting == null)
                        return false;
                    
                    else
                    {
                        newsStatusRepo.Delete<NewsStatus>(NewsStatusId);
                        if (MemCache.IsIncache("AllNewsStatusKey"))
                            MemCache.GetFromCache<List<NewsStatus>>("AllNewsStatusKey").Remove(NewsStatusExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsStatus> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<NewsStatus> lstNewsStatus = new List<NewsStatus>();
                if (MemCache.IsIncache("AllNewsKey"))
                {
                    return MemCache.GetFromCache<List<NewsStatus>>("AllNewsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (NewsStatusRepository newsStatusRepo = new NewsStatusRepository())
                    {
                        lstNewsStatus = newsStatusRepo.GetListPaged<NewsStatus>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNewsKey", lstNewsStatus);
                        return lstNewsStatus;
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
            string NewsStatusid;
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
            if (dic.TryGetValue("NewsStatusid", out NewsStatusid))
            {
                dicAux.Add("@NewsStatusid", NewsStatusid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@News_Status", keyword);
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
        // ~NewsStatusService()
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
