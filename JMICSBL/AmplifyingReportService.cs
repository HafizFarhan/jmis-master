using MTC.JMICS.DAL;
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
    public class AmplifyingReportService : BaseService, IDisposable
    {
        public AmplifyingReportView GetById(int id)
        {
            try
            {
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    AmplifyingReportView ARViewModel = new AmplifyingReportView();
                    {
                        ARViewModel = ARRepo.Get<AmplifyingReportView>(id);
                        return ARViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AmplifyingReportView> GetByCOIId(int COIId)
        {
            try
            {
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    return ARRepo.GetList<AmplifyingReportView>(new { COI_Id = COIId }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AmplifyingReportView Add(Subscriber SubsModel, string UserName, AmplifyingReport ARModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (ARModel == null)
                    throw new Exception("Preliminary report model is null");

                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    AmplifyingReportView ARView = new AmplifyingReportView();
                    string coiNumber = "";
                    List<AmplifyingReportView> lstARs = ARRepo.GetList<AmplifyingReportView>(new { COI_ID = ARModel.COIId }).ToList();

                    using (COIRepository coiRepo = new COIRepository())
                    {
                        coiNumber = coiRepo.Get<COI>(ARModel.COIId).COINumber;
                    }
                    string generatedARNo = coiNumber + "-AR-" + 1;

                    ARView = lstARs.OrderByDescending(x => x.ARId).FirstOrDefault();
                   if (ARView != null)
                        generatedARNo = Common.GetNextSubReportNumber(ARView.ARNumber);

                    ARModel.ARNumber = generatedARNo;
                    ARModel.ActionAddressee = string.Join(",", ARModel.ActionAddresseeArray);
                    ARModel.InformationAddressee = string.Join(",", ARModel.InformationAddresseeArray);
                    ARModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    ARModel.SubscriberId = SubsModel.SubscriberId;
                    ARModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    ARModel.CreatedBy = UserName;

                    int rowId = ARRepo.Insert(ARModel);
                    return ARView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, AmplifyingReport ARModel)
        {
            try
            {
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    ARModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    ARModel.LastModifiedBy = UserName;
                    ARRepo.Update<AmplifyingReport>(ARModel);
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
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    var ARExisting = ARRepo.Get<AmplifyingReport>(id);
                    if (ARExisting == null)
                        return false;
                    
                    else
                    {
                        ARRepo.Delete<AmplifyingReport>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<AmplifyingReportView> GetSubsARs(Subscriber subsModel)
        {
            try
            {
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    return ARRepo.GetList<AmplifyingReportView>("WHERE Information_Addressee_Codes LIKE '%" + subsModel.SubscriberCode + "%'");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<AmplifyingReportView> List()
        {
            try
            {
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    return ARRepo.GetList<AmplifyingReportView>();
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
                string[] searchColumns = new string[] { "COI_Number", "Subscriber_Code", "COI_Type_Name", "PR_Number", "Action_Addressee_Codes", "Information_Addressee_Codes", "Remarks", "MMSI", "Threat_Name" };
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (AmplifyingReportRepository ARRepo = new AmplifyingReportRepository())
                {
                    dtModel.Data = ARRepo.GetListPaged<AmplifyingReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = ARRepo.RecordCount<AmplifyingReportView>(parameters, searchColumns);
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

                if (dic.TryGetValue("query[threatName]", out query))
                    dicAux.Add("Threat_Name", query);
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
        // ~AmplifyingReportSerivce()
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
