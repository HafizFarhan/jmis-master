using Mapster;
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
    public class SubsequentReportService : BaseService, IDisposable
    {
        public SubsequentReportView GetById(int id)
        {
            try
            {
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    SubsequentReportView SRModel = new SubsequentReportView();
                    {
                        SRModel = SRRepo.Get<SubsequentReportView>(id);
                        return SRModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SubsequentReportView> GetByPRId(int PRId)
        {
            try
            {
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    return SRRepo.GetList<SubsequentReportView>(new { PR_Id = PRId }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SubsequentReportView Add(Subscriber SubsModel, string UserName, SubsequentReport SRModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (SRModel == null)
                    throw new Exception("Subsequent report model is null");


                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    SubsequentReportView srView = new SubsequentReportView();
                    string prNumber = "";

                    List<SubsequentReportView> lstSRs = SRRepo.GetList<SubsequentReportView>(new { PR_Id = SRModel.PRId }).ToList();

                    using (PreliminaryReportRepository prRepo = new PreliminaryReportRepository())
                    {
                        prNumber = prRepo.Get<PreliminaryReport>(SRModel.PRId).PRNumber;
                    }
                    string generatedSRNo = prNumber + "-SR-" + lstSRs.Count + 1;

                    srView = lstSRs.OrderByDescending(x => x.Id).FirstOrDefault();
                    if(srView != null)
                        generatedSRNo = Common.GetNextSubReportNumber(srView.SRNumber);
                    
                    SRModel.SRNumber = generatedSRNo;
                    SRModel.ActionAddressee = string.Join(",", SRModel.ActionAddresseeArray);
                    SRModel.InformationAddressee = string.Join(",", SRModel.InformationAddresseeArray);
                    SRModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    SRModel.SubscriberId = SubsModel.SubscriberId;
                    SRModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    SRModel.CreatedBy = UserName;

                    int rowId = SRRepo.Insert(SRModel);
                    return srView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, SubsequentReport SRModel)
        {
            try
            {
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    SRModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    SRModel.LastModifiedBy = UserName;
                    SRRepo.Update(SRModel);
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
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    var SRExisting = SRRepo.Get<SubsequentReport>(id);
                    if (SRExisting == null)
                        return false;

                    else
                    {
                        SRRepo.Delete<SubsequentReport>(id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<SubsequentReportView> GetAllSRsBySubsId(int subsId)
        {
            try
            {
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    return SRRepo.GetList<SubsequentReportView>(new { Subscriber_Id = subsId });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<SubsequentReportView> List()
        {
            try
            {
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    return SRRepo.GetList<SubsequentReportView>();
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
                string[] searchColumns = new string[] { "COI_Number", "Subscriber_Code", "COI_Type_Name", "PR_Number", "Action_Addressee", "Information_Addressee", "Remarks", "MMSI", "Threat_Name" };
                DataTableModel dtModel = new DataTableModel();
                Meta meta = new Meta();
                if (dic.TryGetValue("pagination[page]", out string page))
                    meta.page = Convert.ToInt64(page);

                if (dic.TryGetValue("pagination[pages]", out string pages))
                    meta.pages = Convert.ToInt64(pages);

                if (dic.TryGetValue("pagination[perpage]", out string perpage))
                    meta.perpage = Convert.ToInt64(perpage);

                var parameters = this.ParseParameters(dic);
                using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                {
                    dtModel.Data = SRRepo.GetListPaged<SubsequentReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = SRRepo.RecordCount<SubsequentReportView>(parameters, searchColumns);
                }
                dtModel.Meta = meta;
                return dtModel;


                #region "Commented Code"
                //List<SubsequentReportView> lstSR = new List<SubsequentReportView>();
                //if (MemCache.IsIncache("AllSRsKey"))
                //{
                //    return MemCache.GetFromCache<List<SubsequentReportView>>("AllSRsKey");
                //}
                //else
                //{
                //    if (dic == null)
                //        dic = new Dictionary<string, string>();

                //    dic.Add("orderby", "Created_On");
                //    dic.Add("offset", "1");
                //    dic.Add("limit", "200");

                //    var parameters = this.ParseParameters(dic);
                //    using (SubsequentReportRepository SRRepo = new SubsequentReportRepository())
                //    {
                //        lstSR = SRRepo.GetListPaged<SubsequentReportView>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                //        MemCache.AddToCache("AllSRsKey", lstSR);
                //        return lstSR;
                //    }
                //} 
                #endregion
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
            //string active = "1";

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
            //dicAux.Add("Active", active);
            //bool Active = true;

            return dicAux;

            #region "Commented Code"

            //string offset;
            //string limit;
            //string orderby;
            //string sort;
            //string keyfilter;
            //string id;

            //if (dic.TryGetValue("offset", out offset))
            //{
            //    dicAux.Add("@offset", offset);
            //}

            //if (dic.TryGetValue("limit", out limit))
            //{
            //    dicAux.Add("@limit", limit);
            //}

            //if (dic.TryGetValue("orderby", out orderby))
            //{
            //    dicAux.Add("@orderby", orderby);
            //}

            //if (dic.TryGetValue("sortorder", out sort))
            //{
            //    dicAux.Add("@sortorder", sort);
            //}
            //if (dic.TryGetValue("keyfilter", out keyfilter))
            //{
            //    dicAux.Add("@keyfilter", keyfilter);
            //}
            //if (dic.TryGetValue("id", out id))
            //{
            //    dicAux.Add("@id", id);
            //}
            //return dicAux; 
            #endregion
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
        // ~SubsequentReportSerivce()
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
