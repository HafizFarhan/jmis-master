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
    public class COITypeService : BaseService, IDisposable
    {
        IRepository<COIType> COITypeRepository = new COITypeRepository();
        public COIType GetById(int COITypeId)
        {
            try
            {
                if (MemCache.IsIncache("AllCOITypesKey"))
                {
                    return MemCache.GetFromCache<List<COIType>>("AllCOITypesKey").Where<COIType>(x => x.COITypeId == COITypeId).FirstOrDefault();
                }
                using (COITypeRepository COITypeRepo = new COITypeRepository())
                {
                    COIType COITypeModel = new COIType();
                    {
                        COITypeModel = COITypeRepo.Get<COIType>(COITypeId);
                        return COITypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public COIType Add(int SubscriberId, string UserName, COIType COITypeModel)
        {
            try
            {
                if (COITypeModel == null)
                    throw new Exception("COI Type model is null");

                using (COITypeRepository COITypeRepo = new COITypeRepository())
                {
                    COITypeModel.CreatedBy = UserName;
                    COITypeModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    var rowId = COITypeRepo.Insert<COIType>(COITypeModel);
                    COITypeModel.COITypeId = rowId;

                    if (MemCache.IsIncache("AllCOITypesKey"))
                        MemCache.GetFromCache<List<COIType>>("AllCOITypesKey").Add(COITypeModel);
                    else
                    {
                        List<COIType> cOIType= new List<COIType>();
                        cOIType.Add(COITypeModel);
                        MemCache.AddToCache("AllCOITypesKey", cOIType);
                    }
                    return COITypeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, COIType COITypeModel)
        {
            try
            {
                using (COITypeRepository COITypeRepo = new COITypeRepository())
                {
                    if (MemCache.IsIncache("AllCOITypesKey"))
                    {
                        List<COIType> coiTypeModel = MemCache.GetFromCache<List<COIType>>("AllCOITypesKey");
                        if (coiTypeModel.Count > 0)
                            coiTypeModel.Remove(coiTypeModel.Find(x => x.COITypeId == COITypeModel.COITypeId));
                    }
                    COITypeModel.LastModifiedBy = UserName;
                    COITypeModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    COITypeRepo.Update<COIType>(COITypeModel);
                   
                    if (MemCache.IsIncache("AllCOITypesKey"))
                        MemCache.GetFromCache<List<COIType>>("AllCOITypesKey").Add(COITypeModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int COITypeId)
        {
            try
            {
                using (COITypeRepository COITypeRepo = new COITypeRepository())
                {
                    var COITypeExisting = COITypeRepo.Get<COIType>(COITypeId);
                    if (COITypeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        COITypeRepo.Delete<COIType>(COITypeId);
                        if (MemCache.IsIncache("AllCOITypesKey"))
                            MemCache.GetFromCache<List<COIType>>("AllCOITypesKey").Remove(MemCache.GetFromCache<List<COIType>>("AllCOITypesKey").Where(x => x.COITypeId == COITypeExisting.COITypeId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIType> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<COIType> lstCOIType = new List<COIType>();
                if (MemCache.IsIncache("AllCOITypesKey"))
                {
                    return MemCache.GetFromCache<List<COIType>>("AllCOITypesKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "COI_Type_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (COITypeRepository COITypeRepo = new COITypeRepository())
                    {
                        lstCOIType = COITypeRepo.GetListPaged<COIType>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllCOITypesKey", lstCOIType);
                        return lstCOIType;
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
            string COItypeid;
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
            if (dic.TryGetValue("COItypeid", out COItypeid))
            {
                dicAux.Add("@COItypeid", COItypeid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@News_Type_Name", keyword);
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
        // ~COITypeService()
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
