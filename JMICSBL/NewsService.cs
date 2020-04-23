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
    public class NewsService : BaseService, IDisposable
    {
        IRepository<News> NewsRepository = new NewsRepository();

        public News GetById(int NewsId)
        {
            try
            {
                if (MemCache.IsIncache("AllNewsKey"))
                {
                    return MemCache.GetFromCache<List<News>>("AllNewsKey").Where<News>(x => x.NewsId == NewsId).FirstOrDefault();
                }
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    News NewsModel = new News();
                    {
                        NewsModel = newsRepo.Get<News>(NewsId);
                        return NewsModel;
                    }
                } 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsView> GetByActivationStatus()
        {
            try
            {
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    List<NewsView> NewsList = newsRepo.GetList<NewsView>("WHERE News_Status_Id = 3 ")?.ToList(); // Is_Activated IS NULL")?.ToList();
                    return NewsList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsView> GetApprovedNews()
        {
            try
            {
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    List<NewsView> NewsList = newsRepo.GetList<NewsView>("WHERE News_Status_Id = 1 ")?.ToList(); // Is_Activated IS NULL")?.ToList();
                    return NewsList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public List<NewsView> GetFilteredNews(string Name, string IsActivated, DateTime FromDate, DateTime ToDate)
        //{
        //    try
        //    {
        //        using (NewsRepository newsRepo = new NewsRepository())
        //        {
        //            List<NewsView> newsList = newsRepo.GetList<NewsView>("WHERE News_Description LIKE '%" + Name + "%' AND Is_Activated = '" + IsActivated + "' AND Created_On >= '" + FromDate + "' And Created_On <= '" + ToDate + "' ")?.ToList();
        //            return newsList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public News Add(News NewsModel)
        {
            try
            {
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    if (NewsModel != null)
                    {
                        var rowId = newsRepo.Insert<News>(NewsModel);
                        NewsModel.NewsId = rowId;
                    }
                    if (MemCache.IsIncache("AllNewsKey"))
                        MemCache.GetFromCache<List<News>>("AllNewsKey").Add(NewsModel);
                    else
                    {
                        List<News> news = new List<News>();
                        news.Add(NewsModel);
                        MemCache.AddToCache("AllNewsKey", news);
                    }
                    return NewsModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(News NewsModel)
        {
            try
            {
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    if (MemCache.IsIncache("AllNewsKey"))
                    {
                        List<News> News = MemCache.GetFromCache<List<News>>("AllNewsKey");
                        if (News.Count > 0)
                            News.Remove(News.Find(x => x.NewsId == NewsModel.NewsId));
                    }

                    newsRepo.Update<News>(NewsModel);
                    if (MemCache.IsIncache("AllNewsKey"))
                        MemCache.GetFromCache<List<News>>("AllNewsKey").Add(NewsModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int NewsId)
        {
            try
            {
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    var NewsExisting = newsRepo.Get<News>(NewsId);
                    if (NewsExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        newsRepo.Delete<News>(NewsId);
                        if (MemCache.IsIncache("AllNewsKey"))
                            MemCache.GetFromCache<List<News>>("AllNewsKey").Remove(NewsExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsView> GetFilteredNews(string Keyword, string ProfileStatusId, DateTime? FromDate, DateTime? ToDate, Dictionary<string, string> dic)
        {
            try
            {
                var parameters = this.ParseParameters(dic);
                
                using (NewsRepository newsRepo = new NewsRepository())
                {
                    string query = " WHERE 1 = 1";
                    if (!string.IsNullOrWhiteSpace(Keyword))
                        query += " AND (News_Heading Like '%" + Keyword + "%' OR News_Type_Name Like '%" + Keyword + "%' OR News_Description Like '%" + Keyword + "%' OR Subscriber_Name Like '%" + Keyword + "%' )";

                    if(int.TryParse(ProfileStatusId, out int statusId))
                        query += " AND News_Status_Id = '" + statusId + "' ";

                    if (FromDate != null)
                        query += " AND Created_On >= '" + FromDate + "' ";
                    if (ToDate != null)
                        query += " And Created_On <= '" + ToDate + "' ";


                    List<NewsView> NewsList = newsRepo.GetList<NewsView>(query, parameters)?.ToList();
                    return NewsList;
                }
            }
            catch (Exception ex)
            {
                //_trace.Error("Error Retrieving Data", exception: ex);
                throw ex;
            }
        }
        public List<NewsView> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<NewsView> lstNews = new List<NewsView>();
                if (MemCache.IsIncache("AllNewsKey"))
                {
                    return MemCache.GetFromCache<List<NewsView>>("AllNewsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (NewsRepository newsRepo = new NewsRepository())
                    {
                        lstNews = newsRepo.GetListPaged<NewsView>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNewsKey", lstNews);
                        return lstNews;
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
            string newsid;

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
            if (dic.TryGetValue("newsid", out newsid))
            {
                dicAux.Add("@newsid", newsid);
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
        // ~NewsService()
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
