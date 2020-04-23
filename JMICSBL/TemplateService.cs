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
    public class TemplateService : BaseService, IDisposable
    {
        public TemplateView GetById(int TemplateId)
        {
            try
            {
                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    TemplateView templateViewModel = new TemplateView();
                    {
                        templateViewModel = templateRepo.Get<TemplateView>(TemplateId);
                        return templateViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TemplateView Add(Subscriber SubsModel, string UserName, Template TemplateModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (TemplateModel == null)
                    throw new Exception("Template model is null");

                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    TemplateView templateViewModel = new TemplateView();
                    TemplateModel.AddressedTo = string.Join(",", TemplateModel.AddressedToArray);
                    TemplateModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    TemplateModel.SubscriberId = SubsModel.SubscriberId;
                    TemplateModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    TemplateModel.CreatedBy = UserName;

                    int rowId = templateRepo.Insert(TemplateModel);
                    return templateViewModel = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, Template TemplateModel)
        {
            try
            {
                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    TemplateModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    TemplateModel.LastModifiedBy = UserName;
                    templateRepo.Update<Template>(TemplateModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int TemplateId)
        {
            try
            {
                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    var templateExisting = templateRepo.Get<Template>(TemplateId);
                    if (templateExisting == null)
                        return false;

                    else
                    {
                        templateRepo.Delete<Template>(TemplateId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<TemplateView> List()
        {
            try
            {
                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    return templateRepo.GetList<TemplateView>();
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
                string[] searchColumns = new string[] {"Subscriber_Code", "Template_Type_Name", "Addressed_To_Codes", "Remarks"};
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (TemplateRepository templateRepo = new TemplateRepository())
                {
                    dtModel.Data = templateRepo.GetListPaged<TemplateView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = templateRepo.RecordCount<TemplateView>(parameters, searchColumns);
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
            string subscriberId = "";

            if (dic != null)
            {
                if (dic.TryGetValue("query[generalSearch]", out keyfilter))
                    dicAux.Add("@keyfilter", keyfilter);

                if (dic.TryGetValue("subscriberid", out subscriberId))
                    dicAux.Add("@subscriber_id", subscriberId);

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
        // ~TemplateService()
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
