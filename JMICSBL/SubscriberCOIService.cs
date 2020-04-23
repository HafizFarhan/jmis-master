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
    public class SubscriberCOIService : BaseService, IDisposable
    {
        IRepository<SubscriberCOI> subCOIRepository = new SubscriberCOIRepository();
        public SubscriberCOI GetById(int SubscriberCOIId)
        {
            try
            {
                if (MemCache.IsIncache("AllSubscriberCOIKey"))
                {
                    return MemCache.GetFromCache<List<SubscriberCOI>>("AllUsersKey").Where<SubscriberCOI>(x => x.Id == SubscriberCOIId).FirstOrDefault();
                }
                using (SubscriberCOIRepository subCOIRepo = new SubscriberCOIRepository())
                {
                    SubscriberCOI subCOIModel = new SubscriberCOI();
                    {
                        subCOIModel = subCOIRepo.Get<SubscriberCOI>(SubscriberCOIId);
                        return subCOIModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SubscriberCOI Add(SubscriberCOI SubscriberCOIModel)
        {
            try
            {
                using (SubscriberCOIRepository subCOIRepo = new SubscriberCOIRepository())
                {
                    // Validate and Map data over here
                    if (SubscriberCOIModel != null)
                    {

                        var rowId = subCOIRepo.Insert<SubscriberCOI>(SubscriberCOIModel);
                        SubscriberCOIModel.Id = rowId;
                    }
                    if (MemCache.IsIncache("AllSubscriberCOIKey"))
                        MemCache.GetFromCache<List<SubscriberCOI>>("AllSubscriberCOIKey").Add(SubscriberCOIModel);
                    else
                    {
                        List<SubscriberCOI> subscriberCOIs = new List<SubscriberCOI>();
                        subscriberCOIs.Add(SubscriberCOIModel);
                        MemCache.AddToCache("AllSubscriberCOIKey", subscriberCOIs);
                    }
                    return SubscriberCOIModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(SubscriberCOI SubscriberCOIModel)
        {
            try
            {
                using (SubscriberCOIRepository subCOIRepo = new SubscriberCOIRepository())
                {
                    if (MemCache.IsIncache("AllSubscriberCOIKey"))
                    {
                        List<SubscriberCOI> subscriberCOIs = MemCache.GetFromCache<List<SubscriberCOI>>("AllSubscriberCOIKey");
                        if (subscriberCOIs.Count > 0)
                            subscriberCOIs.Remove(subscriberCOIs.Find(x => x.Id == SubscriberCOIModel.Id));
                    }

                    subCOIRepo.Update<SubscriberCOI>(SubscriberCOIModel);
                    if (MemCache.IsIncache("AllSubscriberCOIKey"))
                        MemCache.GetFromCache<List<SubscriberCOI>>("AllSubscriberCOIKey").Add(SubscriberCOIModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int SubscriberCOIId)
        {
            try
            {
                using (SubscriberCOIRepository subCOIRepo = new SubscriberCOIRepository())
                {
                    var SubCOIExisting = subCOIRepo.Get<SubscriberCOI>(SubscriberCOIId);
                    if (SubCOIExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        subCOIRepo.Delete<SubscriberCOI>(SubscriberCOIId);
                        if (MemCache.IsIncache("AllSubscriberCOIKey"))
                            MemCache.GetFromCache<List<SubscriberCOI>>("AllSubscriberCOIKey").Remove(SubCOIExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        // ~SubscriberCOIService()
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
