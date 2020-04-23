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
    public class EventService : BaseService, IDisposable
    {
        public Event GetById(int EventId)
        {
            try
            {
                if (MemCache.IsIncache("AllEventsKey"))
                {
                    return MemCache.GetFromCache<List<Event>>("AllEventsKey").Where<Event>(x => x.EventId == EventId).FirstOrDefault();
                }
                using (EventRepository EventRepo = new EventRepository())
                {
                    Event EventModel = new Event();
                    {
                        EventModel = EventRepo.Get<Event>(EventId);
                        return EventModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EventView> GetBySubsId (int? SubsId, Dictionary<string, string> dic)
        {
            try
            {
                var parameters = this.ParseParameters(dic);
                using (EventRepository EventRepo = new EventRepository())
                {
                    string query = " WHERE 1 = 1 ";
                        
                    query += " AND Subscriber_Id = '" + SubsId + "' ";

                       
                    List<EventView> eventModelList =  EventRepo.GetList<EventView>(query, parameters)?.ToList();
                    return eventModelList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Event Add(Event EventModel)
        {
            try
            {
                using (EventRepository EventRepo = new EventRepository())
                {
                    if (EventModel != null)
                    {
                        var rowId = EventRepo.Insert<Event>(EventModel);
                        EventModel.EventId = rowId;
                    }
                    if (MemCache.IsIncache("AllEventsKey"))
                        MemCache.GetFromCache<List<Event>>("AllEventsKey").Add(EventModel);
                    else
                    {
                        List<Event> events = new List<Event>();
                        events.Add(EventModel);
                        MemCache.AddToCache("AllEventsKey", events);
                    }
                    return EventModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(Event EventModel)
        {
            try
            {
                using (EventRepository EventRepo = new EventRepository())
                {
                    if (MemCache.IsIncache("AllEventsKey"))
                    {
                        List<Event> events = MemCache.GetFromCache<List<Event>>("AllEventsKey");
                        if (events.Count > 0)
                            events.Remove(events.Find(x => x.EventId == EventModel.EventId));
                    }

                    EventRepo.Update<Event>(EventModel);
                    if (MemCache.IsIncache("AllEventsKey"))
                        MemCache.GetFromCache<List<Event>>("AllEventsKey").Add(EventModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int EventId)
        {
            try
            {
                using (EventRepository EventRepo = new EventRepository())
                {
                    var EventExisting = EventRepo.Get<Event>(EventId);
                    if (EventExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        EventRepo.Delete<Event>(EventId);
                        if (MemCache.IsIncache("AllEventsKey"))
                            MemCache.GetFromCache<List<Event>>("AllUsersKey").Remove(EventExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Event> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<Event> lstEvent = new List<Event>();
                if (MemCache.IsIncache("AllUsersKey"))
                {
                    return MemCache.GetFromCache<List<Event>>("AllUsersKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);

                    using (EventRepository EventRepo = new EventRepository())
                    {
                        lstEvent = EventRepo.GetListPaged<Event>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllUsersKey", lstEvent);
                        return lstEvent;
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
            string eventid;

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
            if (dic.TryGetValue("Event_Id", out eventid))
            {
                dicAux.Add("@Eventid", eventid);
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
