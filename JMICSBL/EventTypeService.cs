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
    public class EventTypeService : BaseService, IDisposable
    {
        public EventType GetById(int EventTypeId)
        {
            try
            {
                if (MemCache.IsIncache("AllEventTypeKey"))
                {
                    return MemCache.GetFromCache<List<EventType>>("AllEventTypeKey").Where<EventType>(x => x.EventTypeId == EventTypeId).FirstOrDefault();
                }
                using (EventTypeRepository EventTypeRepo = new EventTypeRepository())
                {
                    EventType EventTypeModel = new EventType();
                    {
                        EventTypeModel = EventTypeRepo.Get<EventType>(EventTypeId);
                        return EventTypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
        public EventType Add(EventType EventTypeModel)
        {
            try
            {
                using (EventTypeRepository EventTypeRepo = new EventTypeRepository())
                {
                    if (EventTypeModel != null)
                    {
                        var rowId = EventTypeRepo.Insert<EventType>(EventTypeModel);
                        EventTypeModel.EventTypeId = rowId;
                    }
                    if (MemCache.IsIncache("AllEventTypeKey"))
                        MemCache.GetFromCache<List<EventType>>("AllEventTypeKey").Add(EventTypeModel);
                    else
                    {
                        List<EventType> eventTypes= new List<EventType>();
                        eventTypes.Add(EventTypeModel);
                        MemCache.AddToCache("AllEventTypeKey", eventTypes);
                    }
                    return EventTypeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(EventType EventTypeModel)
        {
            try
            {
                using (EventTypeRepository EventTypeRepo = new EventTypeRepository())
                {
                    if (MemCache.IsIncache("AllEventTypeKey"))
                    {
                        List<EventType> eventTypes = MemCache.GetFromCache<List<EventType>>("AllEventTypeKey");
                        if (eventTypes.Count > 0)
                            eventTypes.Remove(eventTypes.Find(x => x.EventTypeId == EventTypeModel.EventTypeId));
                    }

                    EventTypeRepo.Update<EventType>(EventTypeModel);
                    if (MemCache.IsIncache("AllEventTypeKey"))
                        MemCache.GetFromCache<List<EventType>>("AllEventTypeKey").Add(EventTypeModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int EventTypeId)
        {
            try
            {
                using (EventTypeRepository EventTypeRepo = new EventTypeRepository())
                {
                    var EventTypeExisting = EventTypeRepo.Get<EventType>(EventTypeId);
                    if (EventTypeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        EventTypeRepo.Delete<EventType>(EventTypeId);
                        if (MemCache.IsIncache("AllEventTypeKey"))
                            MemCache.GetFromCache<List<EventType>>("AllEventTypeKey").Remove(EventTypeExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EventType> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<EventType> lstEventTypes = new List<EventType>();
                if (MemCache.IsIncache("AllEventTypeKey"))
                {
                    return MemCache.GetFromCache<List<EventType>>("AllEventTypeKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Event_Type_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);

                    using (EventTypeRepository EventTypeRepo = new EventTypeRepository())
                    {
                        lstEventTypes = EventTypeRepo.GetListPaged<EventType>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllEventTypeKey", lstEventTypes);
                        return lstEventTypes;
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
            string eventtypeid;

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
            if (dic.TryGetValue("Event_Type_Id", out eventtypeid))
            {
                dicAux.Add("@eventtypeid", eventtypeid);
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
        // ~EventTypeService()
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
