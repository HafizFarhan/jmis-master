using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Security;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class UserService : BaseService, IDisposable
    {
        IRepository<User> userRepository = new UserRepository();
        //IRepository<Subscriber> subscriberRepository = new SubscriberRepository();
        public User GetById(int userId)
        {
            try
            {
                //if (MemCache.IsIncache("AllUsersKey"))
                //{
                //    return MemCache.GetFromCache<List<User>>("AllUsersKey").Where<User>(x => x.UserId == userId).FirstOrDefault();
                //}
                using (UserRepository userRepo = new UserRepository())
                {
                    User userModel = new User();
                    {
                        userModel = userRepo.Get<User>(userId);
                        return userModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public User Add(User userModel)
        {
            try
            {
                using (UserRepository userRepo = new UserRepository())
                {
                    // Validate and Map data over here
                    if (userModel != null)
                    {
                        //userModel.CreatedBy = createdBy;
                        //userModel.CreatedOn = Common.GetLocalDateTime(subscriberId);
                        userModel.Active = true;

                        var rowId = userRepo.Insert<User>(userModel);
                        userModel.UserId = rowId;
                    }

                    //if (MemCache.IsIncache("AllUsersKey"))
                    //    MemCache.GetFromCache<List<User>>("AllUsersKey").Add(userModel);
                    //else
                    //{
                    //    List<User> users = new List<User>();
                    //    users.Add(userModel);
                    //    MemCache.AddToCache("AllUsersKey", users);
                    //}
                    return userModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(User userModel)
        {
            try
            {
                using (UserRepository userRepo = new UserRepository())
                {
                    //if (MemCache.IsIncache("AllUsersKey"))
                    //{
                    //    List<User> users = MemCache.GetFromCache<List<User>>("AllUsersKey");
                    //    if (users.Count > 0)
                    //        users.Remove(users.Find(x => x.SubscriberId == userModel.SubscriberId));
                    //}

                    userRepo.Update<User>(userModel);
                    //if (MemCache.IsIncache("AllUsersKey"))
                    //    MemCache.GetFromCache<List<User>>("AllUsersKey").Add(userModel);
                    return true;
                   }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int userId)
        {
            try
            {
                using (UserRepository userRepo = new UserRepository())
                {
                    var userExisting = userRepo.Get<User>(userId);
                    if (userExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        userRepo.Delete<User>(userId);
                        //if (MemCache.IsIncache("AllUsersKey"))
                        //    MemCache.GetFromCache<List<User>>("AllUsersKey").Remove(userExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<User> List(Dictionary<string, string> dic=null)
        {
            try
            {
                List<User> lstUser = new List<User>();
                //if (MemCache.IsIncache("AllUsersKey"))
                //{
                //    return MemCache.GetFromCache<List<User>>("AllUsersKey");
                //}
                //else
                //{
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "First_Name");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (UserRepository userRepo = new UserRepository())
                    {
                    lstUser = userRepo.GetListPaged<User>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        //MemCache.AddToCache("AllUsersKey", lstUser);
                        return lstUser;
                    }
                //}
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
            string userid;

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
            if (dic.TryGetValue("userid", out userid))
            {
                dicAux.Add("@subscriberid", userid);
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
        // ~UserService() {
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
