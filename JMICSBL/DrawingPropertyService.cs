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
    public class DrawingPropertyService : BaseService, IDisposable
    {
        public DrawingProperty GetById(int drawingPropertyId)
        {
            try
            {
                if (MemCache.IsIncache("AllDPsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey").Where<DrawingProperty>(x => x.DrawingPropertyId == drawingPropertyId).FirstOrDefault();
                }
                using (DrawingPropertyRepository drawingPropertyRepo = new DrawingPropertyRepository())
                {
                    DrawingProperty drawingPropertyModel = new DrawingProperty();
                    {
                        drawingPropertyModel = drawingPropertyRepo.Get<DrawingProperty>(drawingPropertyId);
                        return drawingPropertyModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DrawingProperty Add(DrawingProperty drawingPropertyModel)
        {
            try
            {
                if (MemCache.IsIncache("AllDPsKey"))
                    MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey").Add(drawingPropertyModel);
                else
                {
                    List<DrawingProperty> drawingProperties = new List<DrawingProperty>();
                    drawingProperties.Add(drawingPropertyModel);
                    MemCache.AddToCache("AllDPsKey", drawingProperties);
                }
                using (DrawingPropertyRepository drawingPropertyRepo = new DrawingPropertyRepository())
                {
                    if (drawingPropertyModel != null)
                    {
                        var rowId = drawingPropertyRepo.Insert<DrawingProperty>(drawingPropertyModel);
                        drawingPropertyModel.DrawingPropertyId = rowId;
                    }
                    return drawingPropertyModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(DrawingProperty drawingPropertyModel)
        {
            try
            {
                using (DrawingPropertyRepository drawingPropertyRepo = new DrawingPropertyRepository())
                {
                    if (MemCache.IsIncache("AllDPsKey"))
                    {
                        List<DrawingProperty> drawingProperties = MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey");
                        if (drawingProperties.Count > 0)
                            drawingProperties.Remove(drawingProperties.Find(x => x.DrawingPropertyId == drawingPropertyModel.DrawingPropertyId));
                    }

                    drawingPropertyRepo.Update<DrawingProperty>(drawingPropertyModel);
                    if (MemCache.IsIncache("AllDPsKey"))
                        MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey").Add(drawingPropertyModel);
                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int drawingPropertyId)
        {
            try
            {
                using (DrawingPropertyRepository drawingPropertyRepo = new DrawingPropertyRepository())
                {
                    var drawingExisting = drawingPropertyRepo.Get<DrawingProperty>(drawingPropertyId);
                    if (drawingExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        drawingPropertyRepo.Delete<DrawingProperty>(drawingPropertyId);
                        if (MemCache.IsIncache("AllDPsKey"))
                            MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey").Remove(drawingExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DrawingProperty> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<DrawingProperty> lstDrawingProp = new List<DrawingProperty>();
                if (MemCache.IsIncache("AllDPsKey"))
                {
                    return MemCache.GetFromCache<List<DrawingProperty>>("AllDPsKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Drawing_Property_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (DrawingPropertyRepository drawingPropertyRepo = new DrawingPropertyRepository())
                    {
                        lstDrawingProp = drawingPropertyRepo.GetListPaged<DrawingProperty>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllDPsKey", lstDrawingProp);
                        return lstDrawingProp;
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
            string drawingpropertyid;

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
            if (dic.TryGetValue("drawingpropertyid", out drawingpropertyid))
            {
                dicAux.Add("@drawingpropertyid", drawingpropertyid);
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
        // ~DrawingPropertyService()
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
