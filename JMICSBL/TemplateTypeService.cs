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
    public class TemplateTypeService : BaseService, IDisposable
    {
        public TemplateType GetById(int TemplateTypeId)
        {
            try
            {
                using (TemplateTypeRepository templateTypeRepo = new TemplateTypeRepository())
                {
                    TemplateType templateTypeModel = new TemplateType();
                    {
                        templateTypeModel = templateTypeRepo.Get<TemplateType>(TemplateTypeId);
                        return templateTypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TemplateType Add(int SubscriberId, string UserName, TemplateType TemplateTypeModel)
        {
            try
            {
                if (TemplateTypeModel == null)
                    throw new Exception("Template Type model is null");
                if (SubscriberId < 0)
                    throw new Exception("Invalid Subscriber id");

                using (TemplateTypeRepository templateTypeRepo = new TemplateTypeRepository())
                {
                    //TemplateType templateTypeModel = new TemplateType();
                    TemplateTypeModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    TemplateTypeModel.CreatedBy = UserName;

                    int rowId = templateTypeRepo.Insert(TemplateTypeModel);
                    return TemplateTypeModel = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, TemplateType TemplateTypeModel)
        {
            try
            {
                using (TemplateTypeRepository templateTypeRepo = new TemplateTypeRepository())
                {
                    TemplateTypeModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    TemplateTypeModel.LastModifiedBy = UserName;
                    templateTypeRepo.Update<TemplateType>(TemplateTypeModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int TemplateTypeId)
        {
            try
            {
                using (TemplateTypeRepository templateTypeRepo = new TemplateTypeRepository())
                {
                    var templateTypeExisting = templateTypeRepo.Get<TemplateType>(TemplateTypeId);
                    if (templateTypeExisting == null)
                        return false;

                    else
                    {
                        templateTypeRepo.Delete<TemplateType>(TemplateTypeId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<TemplateType> List()
        {
            try
            {
                using (TemplateTypeRepository templateTypeRepo = new TemplateTypeRepository())
                {
                    return templateTypeRepo.GetList<TemplateType>();
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
                string[] searchColumns = new string[] {"Subscriber_Code", "TemplateType_Type_Name", "Addressed_To_Codes", "Remarks"};
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (TemplateTypeRepository TemplateTypeRepo = new TemplateTypeRepository())
                {
                    dtModel.Data = TemplateTypeRepo.GetListPaged<TemplateType>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = TemplateTypeRepo.RecordCount<TemplateType>(parameters, searchColumns);
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
            string query = "";
            string keyfilter;
            //string subscriberId = "";

            if (dic != null)
            {
                if (dic.TryGetValue("query[generalSearch]", out keyfilter))
                    dicAux.Add("@keyfilter", keyfilter);

                //if (dic.TryGetValue("subscriberid", out subscriberId))
                //    dicAux.Add("@subscriber_id", subscriberId);

                //if (dic.TryGetValue("query[threatName]", out query))
                //    dicAux.Add("Threat_Name", query);
            }
            dicAux.Add("orderby", orderby);
            dicAux.Add("sortorder", sort);

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
        // ~TemplateTypeService()
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
