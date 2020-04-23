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
    public class ShipPictureService : BaseService, IDisposable
    {
        IRepository<ShipPicture> shipPictureRepository = new ShipPictureRepository();
        public ShipPicture GetById(int shipPictureId)
        {
            try
            {
                if (MemCache.IsIncache("AllShipPicturesKey"))
                    return MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey").Where<ShipPicture>(x => x.ShipPictureId == shipPictureId).FirstOrDefault();
                
                using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                {
                    ShipPicture shipPicModel = new ShipPicture();
                    shipPicModel = shipPicRepo.Get<ShipPicture>(shipPictureId);
                    return shipPicModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ShipPicture Add(ShipPicture shipPicModel)
        {
            try
            {

                using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                {
                    if (shipPicModel != null)
                    {
                        var rowId = shipPicRepo.Insert<ShipPicture>(shipPicModel);
                        shipPicModel.ShipPictureId = rowId;
                    }

                    if (MemCache.IsIncache("AllShipPicturesKey"))
                        MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey").Add(shipPicModel);
                    else
                    {
                        List<ShipPicture> ShipPictures = new List<ShipPicture>();
                        ShipPictures.Add(shipPicModel);
                        MemCache.AddToCache("AllShipPicturesKey", ShipPictures);
                    }
                    return shipPicModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(ShipPicture shipPicModel)
        {
            try
            {
                using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                {
                    if (MemCache.IsIncache("AllShipPicturesKey"))
                    {
                        List<ShipPicture> ShipPictures = MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey");
                        if(ShipPictures.Count > 0)
                            ShipPictures.Remove(ShipPictures.Find(x => x.ShipPictureId == shipPicModel.ShipPictureId));
                    }

                    shipPicRepo.Update<ShipPicture>(shipPicModel);
                    if (MemCache.IsIncache("AllShipPicturesKey"))
                        MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey").Add(shipPicModel);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int ShipPictureId)
        {
            try
            {
                using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                {
                    var ShipPictureExisting = shipPicRepo.Get<ShipPicture>(ShipPictureId);
                    if (ShipPictureExisting == null)
                        return false;
                    
                    else
                    {
                        shipPicRepo.Delete<ShipPicture>(ShipPictureId);
                        if (MemCache.IsIncache("AllShipPicturesKey"))
                        {
                            ShipPicture shipPicTodel = MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey").Where(x => x.ShipPictureId == ShipPictureExisting.ShipPictureId).ToList().FirstOrDefault();
                            if (shipPicTodel != null)
                                MemCache._cache.Remove(shipPicTodel);
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
        public ShipPicture GetShipPicByIMO(string IMO)
        {
            try
            {
                using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                {
                    string query = "WHERE 1 = 1";
                    if (!string.IsNullOrWhiteSpace(IMO))
                        query += " AND IMO IN ('" + IMO + "')";

                    ShipPicture shipPicModel = shipPicRepo.GetList<ShipPicture>(query).OrderByDescending(x => x.ShipPictureId).FirstOrDefault();
                    return shipPicModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ShipPicture> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<ShipPicture> lstShipPicture = new List<ShipPicture>();
                if (MemCache.IsIncache("AllShipPicturesKey"))
                    return MemCache.GetFromCache<List<ShipPicture>>("AllShipPicturesKey");
                
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "ShipPicture_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (ShipPictureRepository shipPicRepo = new ShipPictureRepository())
                    {
                        lstShipPicture = shipPicRepo.GetListPaged<ShipPicture>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();

                        MemCache.AddToCache("AllShipPicturesKey", lstShipPicture);
                        return lstShipPicture;
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
            string shippictureid;
            string keyword;

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
            if (dic.TryGetValue("shippictureid", out shippictureid))
            {
                dicAux.Add("@shippictureid", shippictureid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@ShipPicture_Name", keyword);
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
        // ~ShipPictureService()
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
