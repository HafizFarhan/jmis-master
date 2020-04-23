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
    public class NewsFeedService : BaseService, IDisposable
    {
        IRepository<NewsFeed> newsFeedRepository = new NewsFeedRepository();
        public NewsFeedView GetById(int NewsFeedId)
        {
            try
            {
                //if (MemCache.IsIncache("AllNewsFeedsKey"))
                //{
                //    return MemCache.GetFromCache<List<NewsFeed>>("AllNewsFeedsKey").Where<NewsFeed>(x => x.NewsFeedId == NewsFeedId).FirstOrDefault();
                //}
                using (NewsFeedRepository newsFeedRepo = new NewsFeedRepository())
                {
                   NewsFeedView newsFeedModel = new NewsFeedView();
                    {
                        newsFeedModel = newsFeedRepo.Get<NewsFeedView>(NewsFeedId);
                        return newsFeedModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NewsFeedView Add(NewsFeed NewsFeedModel, int SubsId, string UserName)
        {
            try
            {
                if (NewsFeedModel == null)
                    throw new Exception("News Feed model is null");

                using (NewsFeedRepository newsFeedRepo = new NewsFeedRepository())
                {
                  NewsFeedView newsFeedview = new NewsFeedView();
                    
                    NewsFeedModel.SubscriberId = SubsId;
                    NewsFeedModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));
                    NewsFeedModel.CreatedBy = UserName;

                    var rowId = newsFeedRepo.Insert<NewsFeed>(NewsFeedModel);
                    NewsFeedModel.NewsFeedId = rowId;

                    newsFeedview = GetById(rowId);
                    if (MemCache.IsIncache("AllNewsFeedsKey"))
                        MemCache.GetFromCache<List<NewsFeedView>>("AllNewsFeedsKey").Add(newsFeedview);
                    else
                    {
                        List<NewsFeedView> newsFeeds = new List<NewsFeedView>();
                        newsFeeds.Add(newsFeedview);
                        MemCache.AddToCache("AllNewsFeedsKey", newsFeeds);
                    }
                    return newsFeedview;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(NewsFeed NewsFeedModel, int SubsId, string UserName)
        {
            try
            {
                using (NewsFeedRepository newsFeedRepo = new NewsFeedRepository())
                {
                    if (MemCache.IsIncache("AllNewsFeedsKey"))
                    {
                        List<NewsFeed> newsFeeds = MemCache.GetFromCache<List<NewsFeed>>("AllNewsFeedsKey");
                        if (newsFeeds.Count > 0)
                            newsFeeds.Remove(newsFeeds.Find(x => x.NewsFeedId == NewsFeedModel.NewsFeedId));
                    }
                    NewsFeedModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));
                    NewsFeedModel.LastModifiedBy = UserName;
                    newsFeedRepo.Update<NewsFeed>(NewsFeedModel);
                    if (MemCache.IsIncache("AllNewsFeedsKey"))
                        MemCache.GetFromCache<List<NewsFeed>>("AllNewsFeedsKey").Add(NewsFeedModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int NewsFeedId)
        {
            try
            {
                using (NewsFeedRepository newsFeedRepo = new NewsFeedRepository())
                {
                    var newsFeedExisting = newsFeedRepo.Get<NewsFeed>(NewsFeedId);
                    if (newsFeedExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        newsFeedRepo.Delete<NewsFeed>(NewsFeedId);
                        if (MemCache.IsIncache("AllNewsFeedsKey"))
                            MemCache.GetFromCache<List<NewsFeed>>("AllNewsFeedsKey").Remove(newsFeedExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsFeedView> List(Dictionary<string, string> Dic=null)
        {
            try
            {
                List<NewsFeedView> lstNewsFeeds = new List<NewsFeedView>();
                if (MemCache.IsIncache("AllNewsFeedsKey"))
                {
                    return MemCache.GetFromCache<List<NewsFeedView>>("AllNewsFeedsKey");
                }
                else
                {
                    if (Dic == null)
                        Dic = new Dictionary<string, string>();

                    Dic.Add("orderby", "Created_On");
                    Dic.Add("offset", "1");
                    Dic.Add("limit", "200");

                    var parameters = this.ParseParameters(Dic);
                    using (NewsFeedRepository newsFeedRepo = new NewsFeedRepository())
                    {
                        lstNewsFeeds = newsFeedRepo.GetListPaged<NewsFeedView>(Convert.ToInt32(Dic["offset"]), Convert.ToInt32(Dic["limit"]), parameters, Dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNewsFeedsKey", lstNewsFeeds);
                        return lstNewsFeeds;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> Dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();

            string offset;
            string limit;
            string orderby;
            string sort;
            string keyfilter;
            string newsfeedid;
            string keyword;

            if (Dic.TryGetValue("offset", out offset))
            {
                dicAux.Add("@offset", offset);
            }

            if (Dic.TryGetValue("limit", out limit))
            {
                dicAux.Add("@limit", limit);
            }

            if (Dic.TryGetValue("orderby", out orderby))
            {
                dicAux.Add("@orderby", orderby);
            }

            if (Dic.TryGetValue("sortorder", out sort))
            {
                dicAux.Add("@sortorder", sort);
            }
            if (Dic.TryGetValue("keyfilter", out keyfilter))
            {
                dicAux.Add("@keyfilter", keyfilter);
            }
            if (Dic.TryGetValue("newsfeedid", out newsfeedid))
            {
                dicAux.Add("@newsfeedid", newsfeedid);
            }
            if (Dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@News_Feed_Type_Name", keyword);
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
        // ~NewsFeedService()
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
