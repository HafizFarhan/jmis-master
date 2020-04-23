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
    public class AspNetUserService : BaseService, IDisposable
    {
        //public AppUser user;
        public AspNetUser GetById(string aspNetUserId)
        {
            try
            {
                //if (MemCache.IsIncache("AllANUsKey"))
                //{
                //    return MemCache.GetFromCache<List<AspNetUser>>("AllANUsKey").Where<AspNetUser>(x => x.Id == aspNetUserId).FirstOrDefault();
                //}
                using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                {
                    AspNetUser aspNetUserModel = new AspNetUser();
                    {
                        aspNetUserModel = aspNetUserRepo.Get<AspNetUser>(aspNetUserId);
                        return aspNetUserModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AspNetUserView Add(AspNetUserView aspNetUserViewModel)
        {
            try
            {
                using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                {
                    if (aspNetUserViewModel != null)
                    {

                        var rowId = aspNetUserRepo.Insert<AspNetUser>(aspNetUserViewModel);
                        aspNetUserViewModel.Id = rowId.ToString();
                    }
                    if (MemCache.IsIncache("AllANUsKey"))
                        MemCache.GetFromCache<List<AspNetUserView>>("AllANUsKey").Add(aspNetUserViewModel);
                    else
                    {
                        List<AspNetUserView> ANUView = new List<AspNetUserView>();
                        ANUView.Add(aspNetUserViewModel);
                        MemCache.AddToCache("AllANUsKey", ANUView);
                    }
                    return aspNetUserViewModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<AspNetUserView> AddAspNetUser(AppUserViewModel appUserView)
        //{
        //    try
        //    {
        //        var appUser = new AppUser()
        //        {
        //            UserName = appUserView.Username,
        //            Email = appUserView.Email,
        //            Subscriber_Id = appUserView.SubscriberId
        //        };

        //        var createdUser = await _userManager.CreateAsync(appUser, appUserView.Password);
        //        if (createdUser.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(appUser, appUserView.Role);
        //            return StatusCode(200);
        //        }
        //        return aspNetUserViewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public bool Update(AspNetUser aspNetUserModel)
        {
            try
            {
                using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                {

                    //if (MemCache.IsIncache("AllANUsKey"))
                    //{
                    //    List<AspNetUser> aspNetUsers = MemCache.GetFromCache<List<AspNetUser>>("AllANUsKey");
                    //    if (aspNetUsers.Count > 0)
                    //        aspNetUsers.Remove(aspNetUsers.Find(x => x.Id == aspNetUserModel.Id));
                    //}

                    aspNetUserRepo.Update<AspNetUser>(aspNetUserModel);
                    //if (MemCache.IsIncache("AllANUsKey"))
                    //    MemCache.GetFromCache<List<AspNetUser>>("AllANUsKey").Add(aspNetUserModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(string aspNetUserId)
        {
            try
            {
                using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                {
                    var aspNetUserExisting = aspNetUserRepo.Get<AspNetUser>(aspNetUserId);
                    if (aspNetUserExisting == null)
                    {
                        return false;
                    }
                    else
                    {
                        aspNetUserRepo.Delete<AspNetUser>(aspNetUserId);
                        //if (MemCache.IsIncache("AllANUsKey"))
                        //    MemCache.GetFromCache<List<AspNetUser>>("AllANUsKey").Remove(aspNetUserExisting);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AspNetUserView> GetFilteredUsers(string Keyword, Dictionary<string, string> dic)
        {
            try
            {
                var parameters = this.ParseParameters(dic);
                using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                {
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(Keyword))
                        query += " WHERE 1 = 1 AND (UserName Like '%" + Keyword + "%' OR Email Like '%" + Keyword + "%' OR Subscriber_Name Like '%" + Keyword + "%' )";

                    List<AspNetUserView> aspNetUserList = aspNetUserRepo.GetList<AspNetUserView>(query, parameters)?.ToList();
                    return aspNetUserList;
                    //return result = aspNetUserRepo.GetListPaged<AspNetUser>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AspNetUserView> List(Dictionary<string, string> dic = null)
        {
            try
            {
                List<AspNetUserView> lstANUs = new List<AspNetUserView>();
                //if (MemCache.IsIncache("AllANUsKey"))
                //{
                //    return MemCache.GetFromCache<List<AspNetUserView>>("AllANUsKey");
                //}
                //else
                //{
                    if (dic == null)
                        dic = new Dictionary<string, string>();

                    dic.Add("orderby", "UserName");
                    dic.Add("offset", "1");
                    dic.Add("limit", "200");

                    var parameters = this.ParseParameters(dic);
                    using (AspNetUserRepository aspNetUserRepo = new AspNetUserRepository())
                    {
                        lstANUs = aspNetUserRepo.GetListPaged<AspNetUserView>(Convert.ToInt32(dic["offset"]), Convert.ToInt32(dic["limit"]), parameters, dic["orderby"]).ToList();
                        //MemCache.AddToCache("AllANUsKey", lstANUs);
                        return lstANUs;
                    }
               // }
            }
            catch (Exception ex)
            {
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
            string aspnetuserid;

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
            if (dic.TryGetValue("aspnetuserid", out aspnetuserid))
            {
                dicAux.Add("@aspnetuserid", aspnetuserid);
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
        // ~AspNetUserService()
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
