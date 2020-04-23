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
    public class DropInfoSharingReportService : BaseService, IDisposable
    {
        public DropReportView GetById(int id)
        {
            try
            {
                using (DropInfoSharingReportRepository drRepo = new DropInfoSharingReportRepository())
                {
                    DropReportView DISRModel = new DropReportView();
                    {
                        DISRModel = drRepo.Get<DropReportView>(id);
                        return DISRModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DropReportView> GetAllDRsBySubsId(int SubsId)
        {
            try
            {
                using (DropInfoSharingReportRepository drRepo = new DropInfoSharingReportRepository())
                {
                    return drRepo.GetList<DropReportView>(new { Subscriber_Id = SubsId });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DropReportView Add(Subscriber SubsModel, string UserName, DropInfoSharingReport DRModel)
        {
            try
            {
                using (DropInfoSharingReportRepository drRepo = new DropInfoSharingReportRepository())
                {
                    DropReportView drView = new DropReportView();
                    DRModel.ActionAddressee = string.Join(",", DRModel.ActionAddresseeArray);
                    DRModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    DRModel.SubscriberId = SubsModel.SubscriberId;
                    DRModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    DRModel.CreatedBy = UserName;

                    int rowId = drRepo.Insert(DRModel);
                    return drView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, DropInfoSharingReport DISRModel)
        {
            try
            {
                using (DropInfoSharingReportRepository DISRRepo = new DropInfoSharingReportRepository())
                {
                    //var DISRExisting = DISRRepo.Get<DropInfoSharingReport>(DISRModel.Id);
                    //if (Common.AreObjectsEqual(DISRModel, DISRExisting))
                    //    return false;

                    //else
                    //{

                    DISRModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    DISRModel.LastModifiedBy = UserName;
                    DISRRepo.Update<DropInfoSharingReport>(DISRModel);
                    return true;
                    //}
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
                using (DropInfoSharingReportRepository DISRRepo = new DropInfoSharingReportRepository())
                {
                    //var DISRExisting = DISRRepo.Get<DropInfoSharingReport>(id);
                    //if (DISRExisting == null)
                    //    return false;
                    
                    //else
                    //{
                        DISRRepo.Delete<DropInfoSharingReport>(id);
                        return true;
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DropReportView> List()
        {
            try
            {
                using (DropInfoSharingReportRepository drRepo = new DropInfoSharingReportRepository())
                {
                    return drRepo.GetList<DropReportView>();
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
                using (DropInfoSharingReportRepository drRepo = new DropInfoSharingReportRepository())
                {
                    dtModel.Data = drRepo.GetListPaged<DropReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = drRepo.RecordCount<DropReportView>(parameters, searchColumns);
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
        // ~DropInfoSharingReportSerivce()
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
