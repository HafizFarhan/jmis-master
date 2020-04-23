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
    public class NotificationService : BaseService, IDisposable
    {
        public NotificationView GetById(int NotificationId)
        {
            try
            {
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    NotificationView notificationModel = new NotificationView();
                    {
                        notificationModel = notificationRepo.Get<NotificationView>(NotificationId);
                        return notificationModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(Notifications NotificationModel, string UserName, int SubsId)
        {
            try
            {
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    NotificationModel.IsRead = true;
                    NotificationModel.LastModifiedBy = UserName;
                    NotificationModel.ReadBy = UserName;
                    NotificationModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));
                    NotificationModel.ReadOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsId));

                    notificationRepo.Update(NotificationModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int NotificationId)
        {
            try
            {
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    var notificationExisting = notificationRepo.Get<Notifications>(NotificationId);
                    if (notificationExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        notificationRepo.Delete<Notifications>(NotificationId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<NotificationView> GetSubsUnreadNitifications(int SubsId)
        {
            try
            {
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    return notificationRepo.GetList<NotificationView>("WHERE Subscriber_Id = " + SubsId + " AND Is_Read = 0").OrderByDescending(x => x.CreatedOn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NotificationView> List(Dictionary<string, string> Dic = null)
        {
            try
            {
                if (Dic == null)
                    Dic = new Dictionary<string, string>();

                Dic.Add("orderby", "Created_On");
                Dic.Add("offset", "1");
                Dic.Add("limit", "200");

                var parameters = this.ParseParameters(Dic);
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    return notificationRepo.GetListPaged<NotificationView>(Convert.ToInt32(Dic["offset"]), Convert.ToInt32(Dic["limit"]), parameters, Dic["orderby"]).ToList();
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
            string notificationid;

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
            if (Dic.TryGetValue("notificationid", out notificationid))
            {
                dicAux.Add("@notificationid", notificationid);
            }
            return dicAux;
        }
        public NotificationView Add(Notifications NotificationModel, int SubscriberId, string UserName)
        {
            try
            {
                using (NotificationRepository notificationRepo = new NotificationRepository())
                {
                    NotificationModel.SubscriberId = SubscriberId;
                    NotificationModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    NotificationModel.CreatedBy = UserName;

                    int rowId = notificationRepo.Insert(NotificationModel);
                    return GetById(rowId);
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
        // ~EventService()
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
