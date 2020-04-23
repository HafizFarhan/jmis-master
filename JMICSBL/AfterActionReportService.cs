using MTC.JMICS.DAL;
using MTC.JMICS.Models;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Responses;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class AfterActionReportService : BaseService, IDisposable
    {
        public AfterActionReportView GetById(int id)
        {
            try
            {
                using (AfterActionReportRepository AARRepo = new AfterActionReportRepository())
                {

                    AfterActionReportView AARViewModel = new AfterActionReportView();
                    {
                        AARViewModel = AARRepo.Get<AfterActionReportView>(id);
                        return AARViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AfterActionReportView Add(Subscriber SubsModel, string UserName, AfterActionReport AARModel)
        {
            try
            {
                using (AfterActionReportRepository aarRepo = new AfterActionReportRepository())
                {
                    AfterActionReportView aarView = new AfterActionReportView();
                    AARModel.AddressedTo = string.Join(",", AARModel.AddressedToArray);
                    AARModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    AARModel.SubscriberId = SubsModel.SubscriberId;
                    AARModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    AARModel.CreatedBy = UserName;

                    int rowId = aarRepo.Insert(AARModel);
                    return aarView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, AfterActionReport AARModel)
        {
            try
            {
                using (AfterActionReportRepository AARRepo = new AfterActionReportRepository())
                {
                    AARModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    AARModel.LastModifiedBy = UserName;
                    AARRepo.Update<AfterActionReport>(AARModel);
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
                using (AfterActionReportRepository AARRepo = new AfterActionReportRepository())
                {
                    var AARExisting = AARRepo.Get<AfterActionReport>(id);
                    if (AARExisting == null)
                        return false;
                    
                    else
                    {
                        AARRepo.Delete<AfterActionReport>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<AfterActionReportView> GetAllAARsBySubsId(int subsId)
        {
            try
            {
                using (AfterActionReportRepository aarRepo = new AfterActionReportRepository())
                {
                    return aarRepo.GetList<AfterActionReportView>(new { Subscriber_Id = subsId });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<AfterActionReportView> List()
        {
            try
            {
                using (AfterActionReportRepository aarRepo = new AfterActionReportRepository())
                {
                    return aarRepo.GetList<AfterActionReportView>();
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
                string[] searchColumns = new string[] { "COI_Number", "Subscriber_Code", "PR_Number", "Remarks", "MMSI", "Latitude", "Longitude" };
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (AfterActionReportRepository aarRepo = new AfterActionReportRepository())
                {
                    dtModel.Data = aarRepo.GetListPaged<AfterActionReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), new string[] { "COI_Number", "PR_Number", "COI_Type_Name", "Threat_Name", "Info_Confidence_Level_Name", "Subscriber_Code", "Remarks" });
                    meta.total = aarRepo.RecordCount<AfterActionReportView>(parameters, searchColumns);
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
            string query;
            string keyfilter;
            string subscriberId = "";

            if (dic != null)
            {
                if (dic.TryGetValue("query[generalSearch]", out keyfilter))
                    dicAux.Add("@keyfilter", keyfilter);

                if (dic.TryGetValue("query[threatName]", out query))
                    dicAux.Add("Threat_Name", query);

                if (dic.TryGetValue("subscriberid", out subscriberId))
                    dicAux.Add("@subscriber_id", subscriberId);
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
        // ~AfterActionReportSerivce()
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
