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
    public class AISTrackService : BaseService, IDisposable
    {
        IRepository<AISTrack> AISTrackRepository = new AISTrackRepository();

        public AISTrack GetById(int AISTrackId)
        {
            try
            {
                if (MemCache.IsIncache("AllAISTracksKey"))
                {
                    return MemCache.GetFromCache<List<AISTrack>>("AllAISTracksKey").Where<AISTrack>(x => x.AISTrackId == AISTrackId).FirstOrDefault();
                }
                using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                {
                    AISTrack AISTrackModel = new AISTrack();
                    {
                        AISTrackModel = aisTrackRepo.Get<AISTrack>(AISTrackId);
                        return AISTrackModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AISTrack Add(AISTrack AISTrackModel)
        {
            try
            {
                using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                {
                    // Validate and Map data over here
                    if (AISTrackModel != null)
                    {
                        
                        var rowId = aisTrackRepo.Insert<AISTrack>(AISTrackModel);
                        AISTrackModel.AISTrackId = rowId;
                    }
                    if (MemCache.IsIncache("AllAISTracksKey"))
                        MemCache.GetFromCache<List<AISTrack>>("AllAISTracksKey").Add(AISTrackModel);
                    else
                    {
                        List<AISTrack> tracks = new List<AISTrack>();
                        tracks.Add(AISTrackModel);
                        MemCache.AddToCache("AllAISTracksKey", tracks);
                    }
                    return AISTrackModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(AISTrack AISTrackModel)
        {
            try
            {
                using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                {
                    if (MemCache.IsIncache("AllAISTracksKey"))
                    {
                        List<AISTrack> aISTracks = MemCache.GetFromCache<List<AISTrack>>("AllStakeholdersKey");
                        if (aISTracks.Count > 0)
                            aISTracks.Remove(aISTracks.Find(x => x.AISTrackId == AISTrackModel.AISTrackId));
                    }

                    aisTrackRepo.Update<AISTrack>(AISTrackModel);
                    if (MemCache.IsIncache("AllStakeholdersKey"))
                        MemCache.GetFromCache<List<AISTrack>>("AllStakeholdersKey").Add(AISTrackModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int AISTrackId)
        {
            try
            {
                using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                {
                    var AISTrackExisting = aisTrackRepo.Get<AISTrack>(AISTrackId);
                    if (AISTrackExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        aisTrackRepo.Delete<AISTrack>(AISTrackId);
                        if (MemCache.IsIncache("AllAISTracksKey"))
                            MemCache.GetFromCache<List<AISTrack>>("AllAISTracksKey").Remove(AISTrackExisting);
                        return true;
                    } 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AISTrack> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<AISTrack> lstAISTracks = new List<AISTrack>();
                if (MemCache.IsIncache("AllAISTracksKey"))
                {
                    return MemCache.GetFromCache<List<AISTrack>>("AllAISTracksKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "MMSI");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (AISTrackRepository aisTrackRepo = new AISTrackRepository())
                    {
                        lstAISTracks = aisTrackRepo.GetListPaged<AISTrack>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        return lstAISTracks;
                    }
                }
            }
            catch (Exception ex)
            {
                //_trace.Error("Error Retrieving Data", exception: ex);
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
            string AISTrackid;

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
            if (dic.TryGetValue("AISTrackid", out AISTrackid))
            {
                dicAux.Add("@AISTrackid", AISTrackid);
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
        // ~AISTrackService()
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
