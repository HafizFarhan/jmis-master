using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Responses;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.BL
{
    public class AAASSOSService : BaseService , IDisposable
    {
        public AAASSOS GetById(int id)
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {
                    AAASSOS ASModel = new AAASSOS();
                    {
                        ASModel = ASRepo.Get<AAASSOS>(id);
                        return ASModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AAASSOS Add(AAASSOS ASModel, int SubscriberId, string UserName)
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {                  
                    if (ASModel != null)
                    {
                        ASModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                        ASModel.CreatedBy = UserName;
                        var rowId = ASRepo.Insert(ASModel);
                        ASModel.Id = rowId;
                    }
                    return ASModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(AAASSOS ASModel)
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {
                    ASRepo.Update(ASModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {
                    var ASExisting = ASRepo.Get<AAASSOS>(id);
                    if (ASExisting == null)
                        return false;
                    else
                    {
                        ASRepo.Delete<AAASSOS>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<AAASSOS> List()
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {
                    return ASRepo.GetList<AAASSOS>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTableModel ListPaged(Dictionary<string, string> dic = null)
        {
            try
            {

                string[] searchColumns = new string[] { "User_Contact_Number", "User_IMEI", "Address", "Created_By" };
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (AAASSOSRepository SosRepository = new AAASSOSRepository())
                {
                    dtModel.Data = SosRepository.GetListPaged<AAASSOS>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = SosRepository.RecordCount<AAASSOS>(parameters, searchColumns);
                }

                dtModel.Meta = meta;
                return dtModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();

            string orderby = "Created_On";
            string sort = "desc";
            string keyFilter;

            if (dic != null)
            {
                if (dic.TryGetValue("query[generalSearch]", out keyFilter))
                    dicAux.Add("@keyfilter", keyFilter);

            }
            dicAux.Add("orderby", orderby);
            dicAux.Add("sortorder", sort);

            return dicAux;
        }
        public long TotalRecord(object whereClause)
        {
            try
            {
                using (AAASSOSRepository ASRepo = new AAASSOSRepository())
                {
                    return ASRepo.RecordCount<AAASSOS>(whereClause);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        // ~AAASSOSService()
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
