using MTC.JMICS.Utility;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using MTC.JMICS.DAL;
using System.Linq;
using MTC.JMICS.Utility.Cache;

namespace MTC.JMICS.BL
{
    public class DrawingService : BaseService, IDisposable
    {
        public Drawing GetById(int drawingId)
        {
            try
            {
                //if (MemCache.IsIncache("AllDrawingsKey"))
                //{
                //    return MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey").Where<Drawing>(x => x.DrawingId == drawingId).FirstOrDefault();
                //}
                using (DrawingRepository drawingRepo = new DrawingRepository())
                {
                    Drawing drawingModel = new Drawing();
                    {
                        drawingModel = drawingRepo.Get<Drawing>(drawingId);
                        return drawingModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Drawing Add(Drawing drawingModel)
        {
            try
            {
                //if (MemCache.IsIncache("AllDrawingsKey"))
                //    MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey").Add(drawingModel);
                //else
                //{
                //    List<Drawing> drawings= new List<Drawing>();
                //    drawings.Add(drawingModel);
                //    MemCache.AddToCache("AllDrawingsKey", drawings);
                //}
                using (DrawingRepository drawingRepo = new DrawingRepository())
                {
                    if (drawingModel != null)
                    {
                        var rowId = drawingRepo.Insert<Drawing>(drawingModel);
                        drawingModel.DrawingId = rowId;
                    }
                    return drawingModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(Drawing drawingModel)
        {
            try
            {
                using (DrawingRepository drawingRepo = new DrawingRepository())
                {
                    //if (MemCache.IsIncache("AllDrawingsKey"))
                    //{
                    //    List<Drawing> drawings = MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey");
                    //    if (drawings.Count > 0)
                    //        drawings.Remove(drawings.Find(x => x.DrawingId == drawingModel.DrawingId));
                    //}

                    drawingRepo.Update<Drawing>(drawingModel);
                    //var SubsExisting = subRepo.Get<Subscriber>(subsModel.SubscriberId);
                    //if (MemCache.IsIncache("AllDrawingsKey"))
                    //    MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey").Add(drawingModel);

                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int drawingId)
        {
            try
            {
                using (DrawingRepository drawingRepo = new DrawingRepository())
                {
                    var drawingExisting = drawingRepo.Get<Drawing>(drawingId);
                    if (drawingExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        drawingRepo.Delete<Drawing>(drawingId);
                        //if (MemCache.IsIncache("AllDrawingsKey"))
                        //    MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey").Remove(drawingExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Drawing> GetBySubsId(int? SubsId, Dictionary<string, string> dic)
        {
            try
            {
                var parameters = this.ParseParameters(dic);
                using (DrawingRepository drawingRepo = new DrawingRepository())
                {
                    string query = " WHERE 1 = 1 ";

                    query += " AND Subscriber_Id = '" + SubsId + "' ";


                    List<Drawing> drawingModelList = drawingRepo.GetList<Drawing>(query, parameters)?.ToList();
                    return drawingModelList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Drawing> List(int SubscriberId, Dictionary<string, string> dic = null)
        {
            try
            {
                List<Drawing> lstDrawing = new List<Drawing>();
                //if (MemCache.IsIncache("AllDrawingsKey"))
                //{
                //    return MemCache.GetFromCache<List<Drawing>>("AllDrawingsKey");
                //}
                //else
                //{
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Drawing_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");
                    dic.Add("subscriberid", SubscriberId.ToString());

                    var parameters = this.ParseParameters(dic);
                    using (DrawingRepository drawingRepo = new DrawingRepository())
                    {
                        lstDrawing = drawingRepo.GetListPaged<Drawing>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        //MemCache.AddToCache("AllDrawingsKey", lstDrawing);
                        return lstDrawing;
                    }
                //}
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
            string drawingid;
            string subscriberId = "";

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
            if (dic.TryGetValue("drawingid", out drawingid))
            {
                dicAux.Add("@drawingid", drawingid);
            }
            if (dic.TryGetValue("subscriberid", out subscriberId))
            {
                dicAux.Add("@subscriber_id", subscriberId);
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
        // ~DrawingService()
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
