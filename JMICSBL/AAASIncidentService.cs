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
    public class AAASIncidentService : BaseService , IDisposable
    {
        public AAASIncident GetById(int id) {
            try
            {
                using (AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {
                    AAASIncident AIModel = new AAASIncident();
                    {
                        AIModel = AIRepo.Get<AAASIncident>(id);
                        return AIModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AAASIncident Add(AAASIncident AIModel,int SubscriberId, string UserName) 
        {
            try
            {
                using (AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {                  
                    if(AIModel != null)
                    {
                        AIModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                        AIModel.CreatedBy = UserName;
                        var rowId = AIRepo.Insert(AIModel);
                        AIModel.Id = rowId;
                    }
                    return AIModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool Update(AAASIncident AIModel)
        {
            try
            {
                using (AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {
                    AIRepo.Update(AIModel);
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
                using (AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {
                    var AIExisting = AIRepo.Get<AAASIncident>(id);
                    if (AIExisting == null)
                        return false;
                    else 
                    {
                        AIRepo.Delete<AAASIncident>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<AAASIncident> List()
        {
            try
            {
                using(AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {
                    return AIRepo.GetList<AAASIncident>();
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
                string[] searchColumns = new string[] { "User_Contact_Number", "Incident_Type", "Description", "Created_By" };
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (AAASIncidentRepository IncidentRepository = new AAASIncidentRepository())
                {
                    dtModel.Data = IncidentRepository.GetListPaged<AAASIncident>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = IncidentRepository.RecordCount<AAASIncident>(parameters, searchColumns);
                }
                dtModel.Meta = meta;
                return dtModel;
            }           
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dictionary<string,object> ParseParameters(Dictionary<string, string> dic)
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
                using (AAASIncidentRepository AIRepo = new AAASIncidentRepository())
                {
                    return AIRepo.RecordCount<AAASIncident>(whereClause);
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
        // ~AAASIncidentService()
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
