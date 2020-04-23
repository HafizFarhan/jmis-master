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
    public class DrawingCoordinateService : BaseService, IDisposable
    {
        public DrawingCoordinate GetById(int DrawingCoordinateId)
        {
            try
            {
                if (MemCache.IsIncache("AllDCsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey").Where<DrawingCoordinate>(x => x.DrawingCoordinateId == DrawingCoordinateId).FirstOrDefault();
                }
                using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                {
                    DrawingCoordinate DrawingCoordinateModel = new DrawingCoordinate();
                    {
                        DrawingCoordinateModel = DrawingCoordinateRepo.Get<DrawingCoordinate>(DrawingCoordinateId);
                        return DrawingCoordinateModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingCoordinate> GetByDrawingId(int DrawingId)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                dic.Add("orderby", "Drawing_Id");
                dic.Add("offset", "1");
                dic.Add("limit", "200");

                var parameters = this.ParseParameters(dic);
                using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                {
                    string query = " WHERE 1 = 1 ";

                    query += " AND Drawing_Id = '" + DrawingId + "' ";

                    List<DrawingCoordinate> drCoordModelList = DrawingCoordinateRepo.GetList<DrawingCoordinate>(query, parameters)?.ToList();
                    return drCoordModelList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DrawingCoordinate Add(DrawingCoordinate drawingCoordinateModel)
        {
            try
            {
                if (MemCache.IsIncache("AllDCsKey"))
                    MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey").Add(drawingCoordinateModel);
                else
                {
                    List<DrawingCoordinate> drawingCoordinates = new List<DrawingCoordinate>();
                    drawingCoordinates.Add(drawingCoordinateModel);
                    MemCache.AddToCache("AllDCsKey", drawingCoordinates);
                }
                using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                {
                    if (drawingCoordinateModel != null)
                    {
                        var rowId = DrawingCoordinateRepo.Insert<DrawingCoordinate>(drawingCoordinateModel);
                        drawingCoordinateModel.DrawingCoordinateId = rowId;
                    }
                    return drawingCoordinateModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(DrawingCoordinate drawingCoordinateModel)
        {
            try
            {
                using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                {
                    if (MemCache.IsIncache("AllDCsKey"))
                    {
                        List<DrawingCoordinate> drawingCoordinates = MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey");
                        if (drawingCoordinates.Count > 0)
                            drawingCoordinates.Remove(drawingCoordinates.Find(x => x.DrawingCoordinateId == drawingCoordinateModel.DrawingCoordinateId));
                    }

                    DrawingCoordinateRepo.Update<DrawingCoordinate>(drawingCoordinateModel);
                    if (MemCache.IsIncache("AllDCsKey"))
                        MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey").Add(drawingCoordinateModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int drawingCoordinateId)
        {
            try
            {
                using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                {
                    var drawingExisting = DrawingCoordinateRepo.Get<DrawingCoordinate>(drawingCoordinateId);
                    if (drawingExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        DrawingCoordinateRepo.Delete<DrawingCoordinate>(drawingCoordinateId);
                        if (MemCache.IsIncache("AllDCsKey"))
                            MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey").Remove(drawingExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingCoordinate> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<DrawingCoordinate> lstDrawingCoordinate = new List<DrawingCoordinate>();
                if (MemCache.IsIncache("AllDCsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingCoordinate>>("AllDCsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Created_On");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (DrawingCoordinateRepository DrawingCoordinateRepo = new DrawingCoordinateRepository())
                    {
                        lstDrawingCoordinate = DrawingCoordinateRepo.GetListPaged<DrawingCoordinate>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllDCsKey", lstDrawingCoordinate);
                        return lstDrawingCoordinate;
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
            string drawingcoordinateid;

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
            if (dic.TryGetValue("drawingcoordinateid", out drawingcoordinateid))
            {
                dicAux.Add("@drawingcoordinateid", drawingcoordinateid);
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
        // ~DrawingCoordinateService()
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
