using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapster;
using MTC.JMICS.Models.Responses;

namespace MTC.JMICS.BL
{
    public class PreliminaryReportService : BaseService, IDisposable
    {
        public PreliminaryReportView GetById(int Id)
        {
            try
            {
                //if (GetFromDB == false && MemCache.IsIncache("AllPRsKey"))
                //{
                //    return MemCache.GetFromCache<List<PreliminaryReportView>>("AllPRsKey").Where<PreliminaryReportView>(x => x.Id == Id).FirstOrDefault();
                //}
                using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                {
                    PreliminaryReportView PRModel = new PreliminaryReportView();
                    {
                        PRModel = PRRepo.Get<PreliminaryReportView>(Id);
                        return PRModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PreliminaryReportView AddPR(Subscriber SubsModel, string UserName, PreliminaryReport prModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (prModel == null)
                    throw new Exception("Preliminary report model is null");

                using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                {
                    PreliminaryReportView prView = new PreliminaryReportView();

                    string generatedPRNo = SubsModel.SubscriberCode + "-PR-" + Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId)).ToString("yy") + "-" + 1 ;

                    Dictionary<string, object> dicFilter = new Dictionary<string, object>();
                    dicFilter.Add("@subscriber_id", SubsModel.SubscriberId);

                    PreliminaryReportView lastPRModel = PRRepo.GetListPaged<PreliminaryReportView>(1, 1, dicFilter, "Created_On DESC").FirstOrDefault();
                    if (lastPRModel != null)
                        generatedPRNo = Common.GetNextReportNumber(lastPRModel.PRNumber);

                    prModel.PRNumber = generatedPRNo;
                    prModel.ActionAddressee = string.Join(",", prModel.ActionAddresseeArray); //"1";
                    prModel.InformationAddressee = string.Join(",", prModel.InformationAddresseeArray);
                    prModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    prModel.SubscriberId = SubsModel.SubscriberId;
                    prModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    prModel.CreatedBy = UserName;

                    int rowId = PRRepo.Insert(prModel);
                    return prView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PreliminaryReportView Add(PreliminaryReport PRModel)
        {
            try
            {
                using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                {
                    PreliminaryReportView prReportView = new PreliminaryReportView();

                    if (PRModel != null)
                    {
                        int rowId = PRRepo.Insert(PRModel);
                        //PRModel.Id = rowId;

                        prReportView = GetById(rowId);
                        //if (MemCache.IsIncache("AllPRsKey"))
                        //    MemCache.GetFromCache<List<PreliminaryReportView>>("AllPRsKey").Add(prReportView);
                        //else
                        //{
                        //    List<PreliminaryReportView> preliminaryReports = new List<PreliminaryReportView>();
                        //    preliminaryReports.Add(prReportView);
                        //    MemCache.AddToCache("AllPRsKey", preliminaryReports);
                        //}
                    }
                    return prReportView;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, PreliminaryReport PRModel)
        {
            try
            {
                using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                {
                    //if (MemCache.IsIncache("AllPRsKey"))
                    //{
                    //    List<PreliminaryReportView> prViews = MemCache.GetFromCache<List<PreliminaryReportView>>("AllPRsKey");
                    //    if (prViews.Count > 0)
                    //        prViews.Remove(prViews.Find(x => x.Id == PRModel.Id));
                    //}
                    PRModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    PRModel.LastModifiedBy = UserName;
                    PRRepo.Update(PRModel);
                    //var updatedPRView = GetById(PRModel.Id, true);
                    //if (MemCache.IsIncache("AllPRsKey"))
                    //    MemCache.GetFromCache<List<PreliminaryReportView>>("AllPRsKey").Add(updatedPRView);

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
                using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                {
                    var PRExisting = PRRepo.Get<PreliminaryReport>(id);
                    if (PRExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        PRRepo.Delete<PreliminaryReport>(id);
                        //if (MemCache.IsIncache("AllPRsKey"))
                        //    MemCache.GetFromCache<List<PreliminaryReport>>("AllPRsKey").Remove(PRExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<PreliminaryReportView> GetAllPRsBySubsId(int subsId)
        {
            try
            {
                using (PreliminaryReportRepository PRRepository = new PreliminaryReportRepository())
                {
                    return PRRepository.GetList<PreliminaryReportView>(new { Subscriber_Id = subsId });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<PreliminaryReportView> List()
        {
            try
            {
                using (PreliminaryReportRepository PRRepository = new PreliminaryReportRepository())
                {
                    return PRRepository.GetList<PreliminaryReportView>();
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
                using (PreliminaryReportRepository PRRepository = new PreliminaryReportRepository())
                {
                    dtModel.Data = PRRepository.GetListPaged<PreliminaryReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = PRRepository.RecordCount<PreliminaryReportView>(parameters, searchColumns);
                }

                dtModel.Meta = meta;
                return dtModel;

                #region "Commented Code"
                //List<PreliminaryReportView> preliminaryReportViews = new List<PreliminaryReportView>();

                //if (MemCache.IsIncache("AllPRsKey"))
                //{
                //    return MemCache.GetFromCache<List<PreliminaryReportView>>("AllPRsKey");
                //}
                //else
                //{
                //if (dic == null)
                //    dic = new Dictionary<string, string>();

                //dic.Add("orderby", "Created_On");
                //dic.Add("offset", "1");
                //dic.Add("limit", "200");

                //var parameters = this.ParseParameters(dic);
                //using (PreliminaryReportRepository PRRepo = new PreliminaryReportRepository())
                //{
                //    preliminaryReportViews = PRRepo.GetListPaged<PreliminaryReportView>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                //    MemCache.AddToCache("AllPRsKey", preliminaryReportViews);
                //    return preliminaryReportViews;
                //}
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public DataTableModel GetFilteredPR(Dictionary<string, string> dic = null)
        //{
        //    try
        //    {
        //        var parameters = this.ParseParameters(dic);
        //        IEnumerable<PreliminaryReportView> result;
        //        using (PreliminaryReportRepository PRRepository = new PreliminaryReportRepository())
        //        {
        //            //if (dic != null)
        //            //{
        //            return result = PRRepository.GetListPaged<PreliminaryReportView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString()  + " "+ parameters["sortorder"].ToString(), new string[] { "COI_Number", "Subscriber_Code", "COI_Type_Name", "PR_Number", "Action_Addressee", "Information_Addressee", "Remarks", "MMSI", "Threat_Name" });
        //            //return result = PRRepository.GetListPaged<PreliminaryReportView>(Convert.ToInt32(parameters["page"]), Convert.ToInt32(parameters["perpage"]), parameters, parameters["orderby"].ToString()); 
        //            // }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();
            string orderby = "Created_On";
            string sort = "desc";
            //string page = "1";
            //string pages = "1";
            //string perpage = "10";
            //string total = "";
            string query = "";
            string keyfilter;
            string subscriberId = "";

            if (dic != null)
            {
                //if (dic.TryGetValue("pagination[page]", out page))
                //    dicAux.Add("page", page);

                //if (dic.TryGetValue("pagination[pages]", out pages))
                //    dicAux.Add("pages", pages);

                //if (dic.TryGetValue("pagination[perpage]", out perpage))
                //    dicAux.Add("perpage", perpage);

                //if (dic.TryGetValue("pagination[total]", out total))
                //    dicAux.Add("total", total);

                if (dic.TryGetValue("query[generalSearch]", out keyfilter))
                {
                    dicAux.Add("@keyfilter", keyfilter);
                }

                if (dic.TryGetValue("subscriberid", out subscriberId))
                {
                    dicAux.Add("@subscriber_id", subscriberId);
                }

                //if (dic.TryGetValue("query[generalSearch]", out query))
                //    dicAux.Add("PR_Number", query);

                if (dic.TryGetValue("query[threatName]", out query))
                    dicAux.Add("Threat_Name", query);
            }
            dicAux.Add("orderby", orderby);
            dicAux.Add("sortorder", sort);
            //dicAux.Add("Is_Lost", "0");
            //dicAux.Add("Is_Dropped", "0");

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
        // ~PriliminaryReportSerivce()
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
