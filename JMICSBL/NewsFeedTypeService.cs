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
    public class NewsFeedTypeService : BaseService, IDisposable
    {
        IRepository<NewsFeedType> newsFeedTypeRepository = new NewsFeedTypeRepository();
        public NewsFeedType GetById(int newsFeedTypeId)
        {
            try
            {
                if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                {
                    return MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey").Where<NewsFeedType>(x => x.NewsFeedTypeId == newsFeedTypeId).FirstOrDefault();
                }
                using (NewsFeedTypeRepository newsFeedTypeRepo = new NewsFeedTypeRepository())
                {
                    NewsFeedType newsFeedTypeModel = new NewsFeedType();
                    {
                        newsFeedTypeModel = newsFeedTypeRepo.Get<NewsFeedType>(newsFeedTypeId);
                        return newsFeedTypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NewsFeedType Add(int SubscriberId, string UserName, NewsFeedType NewsFeedTypeModel)
        {
            try
            {
                if (NewsFeedTypeModel == null)
                    throw new Exception("News Feed Type model is null");

                using (NewsFeedTypeRepository newsFeedTypeRepo = new NewsFeedTypeRepository())
                {
                    // Validate and Map data over here
                    NewsFeedTypeModel.CreatedBy = UserName;
                    NewsFeedTypeModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    var rowId = newsFeedTypeRepo.Insert<NewsFeedType>(NewsFeedTypeModel);
                    NewsFeedTypeModel.NewsFeedTypeId = rowId;

                    if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                        MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey").Add(NewsFeedTypeModel);
                    else
                    {
                        List<NewsFeedType> newsFeedTypes = new List<NewsFeedType>();
                        newsFeedTypes.Add(NewsFeedTypeModel);
                        MemCache.AddToCache("AllNewsFeedTypeKey", newsFeedTypes);
                    }
                    return NewsFeedTypeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, NewsFeedType NewsFeedTypeModel)
        {
            try
            {
                using (NewsFeedTypeRepository newsFeedTypeRepo = new NewsFeedTypeRepository())
                {
                    if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                    {
                        List<NewsFeedType> newsFeedTypes = MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey");
                        if (newsFeedTypes.Count > 0)
                            newsFeedTypes.Remove(newsFeedTypes.Find(x => x.NewsFeedTypeId == NewsFeedTypeModel.NewsFeedTypeId));
                    }
                    NewsFeedTypeModel.LastModifiedBy = UserName;
                    NewsFeedTypeModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    newsFeedTypeRepo.Update<NewsFeedType>(NewsFeedTypeModel);
                    
                    if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                        MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey").Add(NewsFeedTypeModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int newsFeedTypeId)
        {
            try
            {
                using (NewsFeedTypeRepository newsFeedTypeRepo = new NewsFeedTypeRepository())
                {
                    var NewsFeedTypeExisting = newsFeedTypeRepo.Get<NewsFeedType>(newsFeedTypeId);
                    if (NewsFeedTypeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        newsFeedTypeRepo.Delete<NewsFeedType>(newsFeedTypeId);
                        if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                            MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey").Remove(MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey").Where(x => x.NewsFeedTypeId == NewsFeedTypeExisting.NewsFeedTypeId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsFeedType> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<NewsFeedType> lstnewsFeedTypes = new List<NewsFeedType>();
                if (MemCache.IsIncache("AllNewsFeedTypeKey"))
                {
                    return MemCache.GetFromCache<List<NewsFeedType>>("AllNewsFeedTypeKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (NewsFeedTypeRepository newsFeedTypeRepo = new NewsFeedTypeRepository())
                    {
                        lstnewsFeedTypes = newsFeedTypeRepo.GetListPaged<NewsFeedType>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNewsFeedTypeKey", lstnewsFeedTypes);
                        return lstnewsFeedTypes;
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
            string newsfeedtypeid;
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
            if (dic.TryGetValue("newsfeedtypeid", out newsfeedtypeid))
            {
                dicAux.Add("@newsfeedtypeid", newsfeedtypeid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
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
        // ~NewsFeedTypeService()
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
