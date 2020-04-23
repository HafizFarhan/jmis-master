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
    public class DrawingShapeService : BaseService, IDisposable
    {
        public DrawingShape GetById(int shapeId)
        {
            try
            {
                if (MemCache.IsIncache("AllDrawingShapeKey"))
                {
                    return MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey").Where<DrawingShape>(x => x.ShapeId == shapeId).FirstOrDefault();
                }
                using (DrawingShapeRepository drShapeRepo = new DrawingShapeRepository())
                {
                    DrawingShape drShapeModel = new DrawingShape();
                    {
                        drShapeModel = drShapeRepo.Get<DrawingShape>(shapeId);
                        return drShapeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DrawingShape Add(DrawingShape drShapeModel)
        {
            try
            {
                if (MemCache.IsIncache("AllDrawingShapeKey"))
                    MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey").Add(drShapeModel);
                else
                {
                    List<DrawingShape> drawingShapes = new List<DrawingShape>();
                    drawingShapes.Add(drShapeModel);
                    MemCache.AddToCache("AllDrawingShapeKey", drawingShapes);
                }
                using (DrawingShapeRepository drShapeRepo = new DrawingShapeRepository())
                {
                    if (drShapeModel != null)
                    {
                        var rowId = drShapeRepo.Insert<DrawingShape>(drShapeModel);
                        drShapeModel.ShapeId = rowId;
                    }
                    return drShapeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(DrawingShape drShapeModel)
        {
            try
            {
                using (DrawingShapeRepository drShapeRepo = new DrawingShapeRepository())
                {
                    if (MemCache.IsIncache("AllDrawingShapeKey"))
                    {
                        List<DrawingShape> drawingShapes = MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey");
                        if (drawingShapes.Count > 0)
                            drawingShapes.Remove(drawingShapes.Find(x => x.ShapeId == drShapeModel.ShapeId));
                    }

                    drShapeRepo.Update<DrawingShape>(drShapeModel);
                    if (MemCache.IsIncache("AllDrawingShapeKey"))
                        MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey").Add(drShapeModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int shapeId)
        {
            try
            {
                using (DrawingShapeRepository drShapeRepo = new DrawingShapeRepository())
                {
                    var drShapeExisting = drShapeRepo.Get<DrawingShape>(shapeId);
                    if (drShapeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        drShapeRepo.Delete<DrawingShape>(shapeId);
                        if (MemCache.IsIncache("AllDrawingShapeKey"))
                            MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey").Remove(drShapeExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingShape> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<DrawingShape> lstDrawingShapes = new List<DrawingShape>();
                if (MemCache.IsIncache("AllDrawingShapeKey"))
                {
                    return MemCache.GetFromCache<List<DrawingShape>>("AllDrawingShapeKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Shape_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (DrawingShapeRepository drShapeRepo = new DrawingShapeRepository())
                    {
                        lstDrawingShapes = drShapeRepo.GetListPaged<DrawingShape>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllDrawingShapeKey", lstDrawingShapes);
                        return lstDrawingShapes;
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
            string shapeid;

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
            if (dic.TryGetValue("shapeid", out shapeid))
            {
                dicAux.Add("@shapeid", shapeid);
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
        // ~DrawingShapeSerivce()
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
