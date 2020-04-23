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
    public class ShipService : BaseService, IDisposable
    {
        IRepository<Ship> shipRepository = new ShipRepository();
        public Ship GetById(int shipId)
        {
            try
            {
                if (MemCache.IsIncache("AllShipsKey"))
                    return MemCache.GetFromCache<List<Ship>>("AllShipsKey").Where<Ship>(x => x.ShipId == shipId).FirstOrDefault();

                using (ShipRepository shipRepo = new ShipRepository())
                {
                    Ship ShipModel = new Ship();
                    ShipModel = shipRepo.Get<Ship>(shipId);
                    return ShipModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Ship Add(Ship shipModel)
        {
            try
            {

                using (ShipRepository shipRepo = new ShipRepository())
                {
                    if (shipModel != null)
                    {
                        var rowId = shipRepo.Insert<Ship>(shipModel);
                        shipModel.ShipId = rowId;
                    }

                    if (MemCache.IsIncache("AllShipsKey"))
                        MemCache.GetFromCache<List<Ship>>("AllShipsKey").Add(shipModel);
                    else
                    {
                        List<Ship> Ships = new List<Ship>();
                        Ships.Add(shipModel);
                        MemCache.AddToCache("AllShipsKey", Ships);
                    }
                    return shipModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(Ship shipModel)
        {
            try
            {
                using (ShipRepository shipRepo = new ShipRepository())
                {
                    if (MemCache.IsIncache("AllShipsKey"))
                    {
                        List<Ship> Ships = MemCache.GetFromCache<List<Ship>>("AllShipsKey");
                        if (Ships.Count > 0)
                            Ships.Remove(Ships.Find(x => x.ShipId == shipModel.ShipId));
                    }
                    shipRepo.Update<Ship>(shipModel);
                    if (MemCache.IsIncache("AllShipsKey"))
                        MemCache.GetFromCache<List<Ship>>("AllShipsKey").Add(shipModel);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int ShipId)
        {
            try
            {
                using (ShipRepository shipRepo = new ShipRepository())
                {
                    var ShipExisting = shipRepo.Get<Ship>(ShipId);
                    if (ShipExisting == null)
                        return false;

                    else
                    {
                        shipRepo.Delete<Ship>(ShipId);

                        if (MemCache.IsIncache("AllShipsKey"))
                        {
                            Ship subsTodel = MemCache.GetFromCache<List<Ship>>("AllShipsKey").Where(x => x.ShipId == ShipExisting.ShipId).ToList().FirstOrDefault();
                            if (subsTodel != null)
                                MemCache._cache.Remove(subsTodel);
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
        public Ship GetShipDetails(string MMSI, string IMO)
        {
            try
            {
                using (ShipRepository shipRepo = new ShipRepository())
                {
                    Ship shipModel = new Ship();
                    shipModel = null;
                    string query = "WHERE 1 = 1";
                    if (!string.IsNullOrWhiteSpace(MMSI) || !string.IsNullOrWhiteSpace(IMO))
                    {
                        query += " AND (MMSI = '" + MMSI + "' OR IMO = '" + IMO + "')";
                        shipModel = shipRepo.GetList<Ship>(query).FirstOrDefault();
                    }
                    return shipModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Ship> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<Ship> lstShip = new List<Ship>();
                if (MemCache.IsIncache("AllShipsKey"))
                {
                    return MemCache.GetFromCache<List<Ship>>("AllShipsKey");
                }

                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "Ship_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (ShipRepository shipRepo = new ShipRepository())
                    {
                        lstShip = shipRepo.GetListPaged<Ship>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();

                        MemCache.AddToCache("AllShipsKey", lstShip);
                        return lstShip;
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
            string shipid;
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
            if (dic.TryGetValue("shipid", out shipid))
            {
                dicAux.Add("@shipid", shipid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@Ship_Name", keyword);
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
        // ~ShipService()
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
