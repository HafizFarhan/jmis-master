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
    public class SubscriberService : BaseService, IDisposable
    {
        IRepository<Subscriber> subscriberRepository = new SubscriberRepository();
        public Subscriber GetById(int SubscriberId)
        {
            try
            {
                if (MemCache.IsIncache("AllStakeholdersKey"))
                {
                    return MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Where<Subscriber>(x => x.SubscriberId == SubscriberId).FirstOrDefault();
                }

                using (SubscriberRepository subRepo = new SubscriberRepository())
                {
                    Subscriber subscriberModel = new Subscriber();
                    subscriberModel = subRepo.Get<Subscriber>(SubscriberId);
                    return subscriberModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Subscriber Add(Subscriber SubsModel, int SubsId, string UserName)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");

                using (SubscriberRepository subRepo = new SubscriberRepository())
                {
                    SubsModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));
                    SubsModel.CreatedBy = UserName;
                    var rowId = subRepo.Insert<Subscriber>(SubsModel);
                    SubsModel.SubscriberId = rowId;
                    
                    if (MemCache.IsIncache("AllStakeholdersKey"))
                        MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Add(SubsModel);
                    else
                    {
                        List<Subscriber> subscribers = new List<Subscriber>();
                        subscribers.Add(SubsModel);
                        MemCache.AddToCache("AllStakeholdersKey", subscribers);
                    }
                    return SubsModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(Subscriber SubsModel, int SubsId, string UserName)
        {
            try
            {
                using (SubscriberRepository subRepo = new SubscriberRepository())
                {
                    if (MemCache.IsIncache("AllStakeholdersKey"))
                    {
                        List<Subscriber> subscribers = MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey");
                        if(subscribers.Count > 0)
                            subscribers.Remove(subscribers.Find(x => x.SubscriberId == SubsModel.SubscriberId));
                    }

                    SubsModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));
                    SubsModel.LastModifiedBy = UserName;
                    subRepo.Update<Subscriber>(SubsModel);
                    if (MemCache.IsIncache("AllStakeholdersKey"))
                        MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Add(SubsModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int SubscriberId)
        {
            try
            {
                using (SubscriberRepository subRepo = new SubscriberRepository())
                {
                    var subscriberExisting = subRepo.Get<Subscriber>(SubscriberId);
                    if (subscriberExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        subRepo.Delete<Subscriber>(SubscriberId);

                        //if (MemCache.IsIncache("AllStakeholdersKey"))
                        //    MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Remove(subscriberExisting);

                        if (MemCache.IsIncache("AllStakeholdersKey"))
                        {
                            MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Remove(MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Where(x => x.SubscriberId == subscriberExisting.SubscriberId).ToList().FirstOrDefault());

                            //(new System.Collections.Generic.ICollectionDebugView<MTC.JMICS.Models.DB.Subscriber>(MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Where(x => x.SubscriberId == subscriberExisting.SubscriberId).ToList()).Items[0]).SubscriberId;
                            // .Remove(subscriberExisting);

                            //MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey").Remove(subscriberExisting.SubscriberId);
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
        public List<Subscriber> GetFilteredSubscribers(string Keyword, string IsApproved, Dictionary<string, string> Dic)
        {
            try
            {
                var parameters = this.ParseParameters(Dic);
                using (SubscriberRepository subsRepo = new SubscriberRepository())
                {
                    string query = "WHERE 1 = 1";
                    if (!string.IsNullOrWhiteSpace(Keyword))
                        query += " AND (Subscriber_Name Like '%" + Keyword + "%' OR Email Like '%" + Keyword + "%' OR Address Like '%" + Keyword + "%' OR City Like '%" + Keyword + "%' )";

                    if (IsApproved.ToUpper() == "NULL")
                        query += " AND Is_Approved IS NULL";
                    else if (IsApproved != "")
                    {
                        query += " AND Is_Approved = '" + IsApproved + "'";
                    }
                    List<Subscriber> SubscriberList = subsRepo.GetList<Subscriber>(query, parameters)?.ToList();
                    return SubscriberList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Subscriber> List(Dictionary<string, string> Dic = null)
        {
            try
            {
                List<Subscriber> lstSubscriber = new List<Subscriber>();
                if (MemCache.IsIncache("AllStakeholdersKey"))
                {
                    return MemCache.GetFromCache<List<Subscriber>>("AllStakeholdersKey");
                }
                else
                {
                    if (Dic == null)
                        Dic = new Dictionary<string, string>();

                    Dic.Add("orderby", "Subscriber_Name");
                    Dic.Add("offset", "1");
                    Dic.Add("limit", "200");

                    var parameters = this.ParseParameters(Dic);
                    using (SubscriberRepository subRepo = new SubscriberRepository())
                    {
                        lstSubscriber = subRepo.GetListPaged<Subscriber>(Convert.ToInt32(Dic["offset"]), Convert.ToInt32(Dic["limit"]), parameters, Dic["orderby"]).ToList();

                        MemCache.AddToCache("AllStakeholdersKey", lstSubscriber);
                        return lstSubscriber;
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
            string subscriberid;
            string keyword;
            string isapproved;
            string isactive;

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
            if (Dic.TryGetValue("subscriberid", out subscriberid))
            {
                dicAux.Add("@subscriberid", subscriberid);
            }
            if (Dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@Subscriber_Name", keyword);
            }
            if (Dic.TryGetValue("IsApproved", out isapproved))
            {
                dicAux.Add("@Is_Approved", isapproved);
            }
            if (Dic.TryGetValue("IsActive", out isactive))
            {
                dicAux.Add("@Active", isactive);
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
        // ~SubscriberService()
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
