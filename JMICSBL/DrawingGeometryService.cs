using MTC.JMICS.Utility;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using MTC.JMICS.DAL;
using MTC.JMICS.Utility.Cache;
using System.Linq;

namespace MTC.JMICS.BL
{
    public class DrawingGeometryService : BaseService, IDisposable
    {
        public DrawingGeometry GetById(int DrawingGeometryId)
        {
            try
            {
                if (MemCache.IsIncache("AllDGsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingGeometry>>("AllDGsKey").Where<DrawingGeometry>(x => x.DrawingGeometryId == DrawingGeometryId).FirstOrDefault();
                }
                using (DrawingGeometryRepository DrawingGeometryRepo = new DrawingGeometryRepository())
                {
                    DrawingGeometry DrawingGeometryModel = new DrawingGeometry();
                    {
                        DrawingGeometryModel = DrawingGeometryRepo.Get<DrawingGeometry>(DrawingGeometryId);
                        return DrawingGeometryModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DrawingGeometry Add(DrawingGeometry DrawingGeometryModel)
        {
            try
            {
                using (DrawingGeometryRepository DrawingGeometryRepo = new DrawingGeometryRepository())
                {
                    if (DrawingGeometryModel != null)
                    {
                        var rowId = DrawingGeometryRepo.Insert<DrawingGeometry>(DrawingGeometryModel);
                        DrawingGeometryModel.DrawingGeometryId = rowId;
                    }
                    if (MemCache.IsIncache("AllDGsKey"))
                        MemCache.GetFromCache<List<DrawingGeometry>>("AllUsersKey").Add(DrawingGeometryModel);
                    else
                    {
                        List<DrawingGeometry> drawings = new List<DrawingGeometry>();
                        drawings.Add(DrawingGeometryModel);
                        MemCache.AddToCache("AllDGsKey", drawings);
                    }
                    return DrawingGeometryModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(DrawingGeometry DrawingGeometryModel)
        {
            try
            {
                using (DrawingGeometryRepository DrawingGeometryRepo = new DrawingGeometryRepository())
                {
                    if (MemCache.IsIncache("AllDGsKey"))
                    {
                        List<DrawingGeometry> drawingGeometries = MemCache.GetFromCache<List<DrawingGeometry>>("AllDGsKey");
                        if (drawingGeometries.Count > 0)
                            drawingGeometries.Remove(drawingGeometries.Find(x => x.DrawingGeometryId == DrawingGeometryModel.DrawingGeometryId));
                    }

                    DrawingGeometryRepo.Update<DrawingGeometry>(DrawingGeometryModel);
                    if (MemCache.IsIncache("AllDGsKey"))
                        MemCache.GetFromCache<List<DrawingGeometry>>("AllDGsKey").Add(DrawingGeometryModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int DrawingGeometryId)
        {
            try
            {
                using (DrawingGeometryRepository DrawingGeometryRepo = new DrawingGeometryRepository())
                {
                    var drawingExisting = DrawingGeometryRepo.Get<DrawingGeometry>(DrawingGeometryId);
                    if (drawingExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        DrawingGeometryRepo.Delete<DrawingGeometry>(DrawingGeometryId);
                        if (MemCache.IsIncache("AllDGsKey"))
                            MemCache.GetFromCache<List<DrawingGeometry>>("AllDGsKey").Remove(drawingExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingGeometry> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<DrawingGeometry> lstDrawingGeometry = new List<DrawingGeometry>();
                if (MemCache.IsIncache("AllDGsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingGeometry>>("AllDGsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (DrawingGeometryRepository DrawingGeometryRepo = new DrawingGeometryRepository())
                    {
                        lstDrawingGeometry = DrawingGeometryRepo.GetListPaged<DrawingGeometry>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllDGsKey", lstDrawingGeometry);
                        return lstDrawingGeometry;
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
            string drawinggeometryid;

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
            if (dic.TryGetValue("drawinggeometryid", out drawinggeometryid))
            {
                dicAux.Add("@drawinggeometryid", drawinggeometryid);
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
        // ~DrawingGeometryService()
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
