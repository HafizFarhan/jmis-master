using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class UserTypeService : BaseService, IDisposable
    {
        IRepository<UserType> userTypeRepository = new UserTypeRepository();
        public UserType GetById(int userTypeId)
        {
            try
            {
                if (MemCache.IsIncache("AllUserTypeKey"))
                {
                    return MemCache.GetFromCache<List<UserType>>("AllUserTypeKey").Where<UserType>(x => x.UserTypeId == userTypeId).FirstOrDefault();
                }
                using (UserTypeRepository userTypeRepo = new UserTypeRepository())
                {
                    UserType userTypeModel = new UserType();
                    {
                        userTypeModel = userTypeRepo.Get<UserType>(userTypeId);
                        return userTypeModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserType Add(int SubscriberId, string UserName, UserType UserTypeModel)
        {
            try
            {
                if (UserTypeModel == null)
                    throw new Exception("User Type model is null");

                using (UserTypeRepository userTypeRepo = new UserTypeRepository())
                {
                    UserTypeModel.CreatedBy = UserName;
                    UserTypeModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    UserTypeModel.Active = true;

                    var rowId = userTypeRepo.Insert<UserType>(UserTypeModel);
                    UserTypeModel.UserTypeId = rowId;
                    
                    if (MemCache.IsIncache("AllUserTypeKey"))
                        MemCache.GetFromCache<List<UserType>>("AllUserTypeKey").Add(UserTypeModel);
                    else
                    {
                        List<UserType> userTypes = new List<UserType>();
                        userTypes.Add(UserTypeModel);
                        MemCache.AddToCache("AllUserTypeKey", userTypes);
                    }
                    return UserTypeModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(int SubscriberId, string UserName, UserType UserTypeModel)
        {
            try
            {
                using (UserTypeRepository userTypeRepo = new UserTypeRepository())
                {
                    // Validate and Map data over here  
                    if (MemCache.IsIncache("AllUserTypeKey"))
                    {
                        List<UserType> userTypes = MemCache.GetFromCache<List<UserType>>("AllUserTypeKey");
                        if (userTypes.Count > 0)
                            userTypes.Remove(userTypes.Find(x => x.UserTypeId == UserTypeModel.UserTypeId));
                    }
                    UserTypeModel.LastModifiedBy = UserName;
                    UserTypeModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    userTypeRepo.Update<UserType>(UserTypeModel);
                    
                    if (MemCache.IsIncache("AllUserTypeKey"))
                        MemCache.GetFromCache<List<UserType>>("AllUserTypeKey").Add(UserTypeModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int UserTypeId)
        {
            try
            {
                using (UserTypeRepository userTypeRepo = new UserTypeRepository())
                {
                    var userTypeExisting = userTypeRepo.Get<UserType>(UserTypeId);
                    if (userTypeExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        userTypeRepo.Delete<UserType>(UserTypeId);
                        if (MemCache.IsIncache("AllUserTypeKey"))
                            MemCache.GetFromCache<List<UserType>>("AllUserTypeKey").Remove(MemCache.GetFromCache<List<UserType>>("AllUserTypeKey").Where(x => x.UserTypeId == userTypeExisting.UserTypeId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserType> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<UserType> lstUserTypes = new List<UserType>();
                if (MemCache.IsIncache("AllUserTypeKey"))
                {
                    return MemCache.GetFromCache<List<UserType>>("AllUserTypeKey");
                }
                else
                {
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "User_Type_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");
                    var parameters = this.ParseParameters(dic);
                    using (UserTypeRepository userTypeRepo = new UserTypeRepository())
                    {
                       lstUserTypes = userTypeRepo.GetListPaged<UserType>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        MemCache.AddToCache("AllUserTypeKey", lstUserTypes);
                        return lstUserTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                //_trace.Error("Error Retrieving Data", exception: ex);
                throw ex;
            }
        }
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();

            string offset;
            string limit;
            string orderby;
            string sort;
            string keyfilter;
            string userTypeid;

            if (dic.TryGetValue("offset", out offset))
            {
                dicAux.Add("@offset", offset);
            }

            if (dic.TryGetValue("limit", out limit))
            {
                dicAux.Add("@limit", limit);
            }

            if (dic.TryGetValue("orderby", out orderby))
            {
                dicAux.Add("@orderby", orderby);
            }

            if (dic.TryGetValue("sortorder", out sort))
            {
                dicAux.Add("@sortorder", sort);
            }
            if (dic.TryGetValue("keyfilter", out keyfilter))
            {
                dicAux.Add("@keyfilter", keyfilter);
            }
            if (dic.TryGetValue("userTypeid", out userTypeid))
            {
                dicAux.Add("@subscriberid", userTypeid);
            }
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
        // ~UserTypeService()
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
