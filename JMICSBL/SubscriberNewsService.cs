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
    public class SubscriberNewsService : BaseService, IDisposable
    {
        IRepository<SubscriberNews> SubscriberNewsRepository = new SubscriberNewsRepository();
        public SubscriberNews GetById(int Id)
        {
            try
            {
                if (MemCache.IsIncache("AllSubscriberNewsKey"))
                {
                    return MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey").Where<SubscriberNews>(x => x.Id == Id).FirstOrDefault();
                }
                using (SubscriberNewsRepository subNewsRepo = new SubscriberNewsRepository())
                {
                    SubscriberNews SubscriberNewsModel = new SubscriberNews();
                    {
                        SubscriberNewsModel = subNewsRepo.Get<SubscriberNews>(Id);
                        return SubscriberNewsModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SubscriberNews Add(SubscriberNews SubscriberNewsModel)
        {
            try
            {
                using (SubscriberNewsRepository subNewsRepo = new SubscriberNewsRepository())
                {
                    // Validate and Map data over here
                    if (SubscriberNewsModel != null)
                    {

                        var rowId = subNewsRepo.Insert<SubscriberNews>(SubscriberNewsModel);
                        SubscriberNewsModel.Id = rowId;
                    }
                    if (MemCache.IsIncache("AllSubscriberNewsKey"))
                        MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey").Add(SubscriberNewsModel);
                    else
                    {
                        List<SubscriberNews> subscriberNews= new List<SubscriberNews>();
                        subscriberNews.Add(SubscriberNewsModel);
                        MemCache.AddToCache("AllSubscriberNewsKey", subscriberNews);
                    }
                    return SubscriberNewsModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(SubscriberNews SubscriberNewsModel)
        {
            try
            {
                using (SubscriberNewsRepository subNewsRepo = new SubscriberNewsRepository())
                {
                    var SubscriberNewsExisting = subNewsRepo.Get<SubscriberNews>(SubscriberNewsModel.Id);
                    if (Common.AreObjectsEqual(SubscriberNewsModel, SubscriberNewsExisting))
                    {
                        return false;
                    }
                    else
                    {
                        subNewsRepo.Update<SubscriberNews>(SubscriberNewsModel);
                        if (MemCache.IsIncache("AllSubscriberNewsKey"))
                        {
                            if (MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey").Remove(SubscriberNewsExisting))
                                MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey").Add(SubscriberNewsModel);
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
        public bool Delete(int Id)
        {
            try
            {
                using (SubscriberNewsRepository subNewsRepo = new SubscriberNewsRepository())
                {
                    var SubscriberNewsExisting = subNewsRepo.Get<SubscriberNews>(Id);
                    if (SubscriberNewsExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        subNewsRepo.Delete<SubscriberNews>(Id);
                        if (MemCache.IsIncache("AllSubscriberNewsKey"))
                            MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey").Remove(SubscriberNewsExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SubscriberNews> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<SubscriberNews> lstsubscriberNews = new List<SubscriberNews>();
                if (MemCache.IsIncache("AllSubscriberNewsKey"))
                {
                    return MemCache.GetFromCache<List<SubscriberNews>>("AllSubscriberNewsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "News_Id");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");
                    var parameters = this.ParseParameters(dic);
                    using (SubscriberNewsRepository subNewsRepo = new SubscriberNewsRepository())
                    {
                 lstsubscriberNews = subNewsRepo.GetListPaged<SubscriberNews>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        return lstsubscriberNews;
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
            string Id;

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
            if (dic.TryGetValue("Id", out Id))
            {
                dicAux.Add("@Id", Id);
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
        // ~SubscriberNewsService()
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
