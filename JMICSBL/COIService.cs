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
    public class COIService : BaseService, IDisposable
    {
        public COIView GetById(int COIId)
        {
            try
            {
                using (COIRepository coiRepo = new COIRepository())
                {
                    COIView coiViewModel = new COIView();
                    {
                        coiViewModel = coiRepo.Get<COIView>(COIId);
                        return coiViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     
        public COIView Add(Subscriber SubsModel, string UserName, COI COIModel)
        {
            try
            {
                if (SubsModel == null)
                    throw new Exception("Subscriber model is null");
                else if (COIModel == null)
                    throw new Exception("COI model is null");

                using (COIRepository coiRepo = new COIRepository())
                {
                    COIView coiView = new COIView();
                    string generatedCOINo = SubsModel.SubscriberCode + "-COI-" + Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId)).ToString("yy") + "-" + 1;//+ "-COI-" + 1;

                    Dictionary<string, object> dicFilter = new Dictionary<string, object>();
                    dicFilter.Add("@subscriber_id", SubsModel.SubscriberId);

                    COIView lastCOIModel = coiRepo.GetListPaged<COIView>(1, 1, dicFilter, "Created_On DESC").FirstOrDefault();
                    if (lastCOIModel != null)
                        generatedCOINo = Common.GetNextReportNumber(lastCOIModel.COINumber);

                    COIModel.COINumber = generatedCOINo;
                    COIModel.ActionAddressee = string.Join(",", COIModel.ActionAddresseeArray);
                    COIModel.InformationAddressee = string.Join(",", COIModel.InformationAddresseeArray);
                    COIModel.ReportingDatetime = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    COIModel.SubscriberId = SubsModel.SubscriberId;
                    COIModel.COIActivationDate = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    COIModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubsModel.SubscriberId));
                    COIModel.CreatedBy = UserName;

                    int rowId = coiRepo.Insert(COIModel);
                    return coiView = GetById(rowId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, COI COIModel)
        {
            try
            {
                using (COIRepository COIRepo = new COIRepository())
                {
                    COIModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    COIModel.LastModifiedBy = UserName;
                    COIRepo.Update<COI>(COIModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int COIId)
        {
            try
            {
                using (COIRepository coiRepo = new COIRepository())
                {
                    var COIExisting = coiRepo.Get<COI>(COIId);
                    if (COIExisting == null)
                        return false;
                    
                    else
                    {
                        coiRepo.Delete<COI>(COIId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<COIView> GetFilteredCOIs(string Keyword, string ProfileStatusId, DateTime? FromDate, DateTime? ToDate, Dictionary<string, string> dic)
        {
            try
            {
                var parameters = this.ParseParameters(dic);

                using (COIRepository coiRepo = new COIRepository())
                {
                    string query = " WHERE 1 = 1 ";
                    if (!string.IsNullOrWhiteSpace(Keyword))
                        query += "  AND (COI_Information Like '%" + Keyword + "%' OR Area_Information Like '%" + Keyword + "%' OR Last_Observation Like '%" + Keyword + "%' OR COI_Remarks Like '%" + Keyword + "%' OR COI_Type_Name Like '%" + Keyword + "%' OR Threat_Name Like '%" + Keyword + "%' )";

                    if (int.TryParse(ProfileStatusId, out int statusId))
                        query += " AND COI_Status_Id = '" + statusId + "' ";

                    if (FromDate != null)
                        query += " AND Created_On >= '" + FromDate + "' ";
                    if (ToDate != null)
                        query += " And Created_On <= '" + ToDate + "' ";

                    List<COIView> COIList = coiRepo.GetList<COIView>(query, parameters)?.ToList();
                    return COIList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<COIView> GetSubsCOIs(Subscriber subsModel)
        {
            try
            {
                using (COIRepository coiRepo = new COIRepository())
                {
                    return coiRepo.GetList<COIView>("WHERE Information_Addressee_Codes LIKE '%" + subsModel.SubscriberCode + "%'");
                }

                //using (SubscriberCOIRepository subscoiRepo = new SubscriberCOIRepository())
                //{
                //    List<COIView> SubscriberCOIList = subscoiRepo.GetList<COIView>(" WHERE Subscriber_Id = " + subsId + "  AND COI_Status_Id = 1 ")?.ToList();
                //    SubscriberCOIList.AddRange(subscoiRepo.GetList<COIView>(" WHERE COI_Id IN (SELECT COI_Id FROM tbl_Subscriber_COIs  WHERE Subscriber_Id = 5) ")?.ToList());
                //    return SubscriberCOIList;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<COIView> List()
        {
            try
            {
                using (COIRepository coiRepo = new COIRepository())
                {
                    return coiRepo.GetList<COIView>();
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
                using (COIRepository coiRepo = new COIRepository())
                {
                    dtModel.Data = coiRepo.GetListPaged<COIView>(Convert.ToInt32(dic["pagination[page]"]), Convert.ToInt32(dic["pagination[perpage]"]), parameters, parameters["orderby"].ToString() + " " + parameters["sortorder"].ToString(), searchColumns);
                    meta.total = coiRepo.RecordCount<COIView>(parameters, searchColumns);
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
        // ~COIService()
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
