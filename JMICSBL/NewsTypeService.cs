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
    public class NewsTypeService : BaseService, IDisposable
    {
        IRepository<NewsType> NewsTypeRepository = new NewsTypeRepository();
        public NewsType GetById(int NewsTypeId)
        {
            try
            {
                if (MemCache.IsIncache("AllNewsTypeKey"))
                {
                    return MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey").Where<NewsType>(x => x.NewsTypeId == NewsTypeId).FirstOrDefault();
                }
                using (NewsTypeRepository newsTypeRepo = new NewsTypeRepository())
                {
                    NewsType NewsTypeModel = new NewsType();
                    {
                        NewsTypeModel = newsTypeRepo.Get<NewsType>(NewsTypeId);
                        return NewsTypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NewsType Add(NewsType NewsTypeModel)
        {
            try
            {
                using (NewsTypeRepository newsTypeRepo = new NewsTypeRepository())
                {
                    // Validate and Map data over here
                    if (NewsTypeModel != null)
                    {

                        var rowId = newsTypeRepo.Insert<NewsType>(NewsTypeModel);
                        NewsTypeModel.NewsTypeId = rowId;
                    }
                    if (MemCache.IsIncache("AllNewsTypeKey"))
                        MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey").Add(NewsTypeModel);
                    else
                    {
                        List<NewsType> newsTypes = new List<NewsType>();
                        newsTypes.Add(NewsTypeModel);
                        MemCache.AddToCache("AllNewsTypeKey", newsTypes);
                    }
                    return NewsTypeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(NewsType NewsTypeModel)
        {
            try
            {
                using (NewsTypeRepository newsTypeRepo = new NewsTypeRepository())
                {
                    var NewsTypeExisting = newsTypeRepo.Get<NewsType>(NewsTypeModel.NewsTypeId);
                    if (Common.AreObjectsEqual(NewsTypeModel, NewsTypeExisting))
                    {
                        return false;
                    }
                    else
                    {
                        newsTypeRepo.Update<NewsType>(NewsTypeModel);
                        if (MemCache.IsIncache("AllNewsTypeKey"))
                        {
                            if (MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey").Remove(NewsTypeExisting))
                                MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey").Add(NewsTypeModel);
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
        public bool Delete(int NewsTypeId)
        {
            try
            {
                using (NewsTypeRepository newsTypeRepo = new NewsTypeRepository())
                {
                    var NewsTypeExisting = newsTypeRepo.Get<NewsType>(NewsTypeId);
                    if (NewsTypeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        newsTypeRepo.Delete<NewsType>(NewsTypeId);
                        if (MemCache.IsIncache("AllNewsTypeKey"))
                            MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey").Remove(NewsTypeExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsType> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<NewsType> lstNewsType = new List<NewsType>();
                if (MemCache.IsIncache("AllNewsTypeKey"))
                {
                    return MemCache.GetFromCache<List<NewsType>>("AllNewsTypeKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (NewsTypeRepository newsTypeRepo = new NewsTypeRepository())
                    {
                        lstNewsType = newsTypeRepo.GetListPaged<NewsType>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNewsTypeKey", lstNewsType);
                        return lstNewsType;
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
            string NewsTypeid;
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
            if (dic.TryGetValue("NewsTypeid", out NewsTypeid))
            {
                dicAux.Add("@NewsTypeid", NewsTypeid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@News_Type_Name", keyword);
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
        // ~NewsTypeService()
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
