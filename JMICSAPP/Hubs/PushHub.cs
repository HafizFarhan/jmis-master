using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Models;
using Microsoft.AspNetCore.Identity;
using JMICSAPP.Data;

namespace JMICSAPP.Hubs
{
    public class PushHub : Hub<IPushHub>
    {
        private readonly UserManager<AppUser> _userManager;
        int loggedInSubsId = 0;
        //public List<Dictionary<int, string>> connetedUsers = new List<Dictionary<int, string>>();
        public PushHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
       
        public override Task OnConnectedAsync()
        {
            //ConnetedUser.connetedUsers.Add(Context.User.Identity.Name, Context.ConnectionId);
            loggedInSubsId=GetSubscriberID(Context.User.Identity.Name);
            Groups.AddToGroupAsync(Context.ConnectionId, loggedInSubsId.ToString());
            return base.OnConnectedAsync();
        }

        public int GetSubscriberID(string userName)
        {
            var user = _userManager.FindByNameAsync(userName);
            loggedInSubsId = Convert.ToInt32(user.Result.Subscriber_Id);
            return Convert.ToInt32(user.Result.Subscriber_Id);
        }

    }

}
