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
    public class LostContactReportService : BaseService, IDisposable
    {
        public LostContactReportView GetById(int id)
        {
            try
            {
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    LostContactReportView LCRViewModel = new LostContactReportView();
                    {
                        LCRViewModel = LCRRepo.Get<LostContactReportView>(id);
                        return LCRViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LostContactReportView Add(Subscriber SubsModel, string UserName, LostContactReport LRModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (LRModel == null)
                    throw new Exception("Lost report model is null");

                using (LostContactReportRepository lrRepo = new LostContactReportRepository())
                {
                    LostContactReportView lrView = new LostContactReportView();

                    LRModel.ActionAddressee = string.Join(",", LRModel.ActionAddresseeArray);
                    LRModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    LRModel.SubscriberId = SubsModel.SubscriberId;
                    LRModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    LRModel.CreatedBy = UserName;

                    int rowId = lrRepo.Insert(LRModel);
                    return lrView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, LostContactReport LCRModel)
        {
            try
            {
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    LCRModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    LCRModel.LastModifiedBy = UserName;
                    LCRRepo.Update(LCRModel);
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
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    var LCRExisting = LCRRepo.Get<LostContactReport>(id);
                    if (LCRExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        LCRRepo.Delete<LostContactReport>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<LostContactReportView> GetAllLRsBySubsId(int subsId)
        {
            try
            {
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    return LCRRepo.GetList<LostContactReportView>(new { Subscriber_Id = subsId });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<LostContactReportView> List()
        {
            try
            {
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    return LCRRepo.GetList<LostContactReportView>();
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
                using (LostContactReportRepository LCRRepo = new LostContactReportRepository())
                {
                    dtModel.Data = LCRRepo.GetListPaged<LostContactReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString());
                    meta.total = LCRRepo.RecordCount<LostContactReportView>(parameters, searchColumns);
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
        // ~LostContactReportSerivce()
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
