using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MTC.JMICS.Utility.Utils
{
    public enum HashType : short
    {
        [Description("SHA1CryptoServiceProvider")]
        SHA1 = 0,
        [Description("SHA256Managed")]
        SHA256 = 1,
        [Description("SHA384Managed")]
        SHA384 = 2,
        [Description("SHA512Managed")]
        SHA512 = 3,
        [Description("MD5CryptoServiceProvider")]
        MD5 = 4
    }

    public enum TimePeriodName : short
    {
        TODAY = 0,
        YESTERDAY = 1,
        THISWEEK = 2,
        LASTWEEK = 3,
        THISMONTH = 4,
        LASTMONTH = 5,
        THISYEAR = 6,
        LASTYEAR = 7,
        DAYS90 = 8
    }

    public enum ApplicationUserType : short
    {
        SysAdmin = 1,
        JMICC = 2,
        Other
        //Vendor = 2,
        //Client = 3,
        //API = 4,
    }

    public enum MasterUserRole : short
    {
        Admin = 1,
        Staff = 2,
    }
    public enum COIStatuses : short
    {
        Approved = 1,
        Declined = 2,
        Pending = 3
    }
    public enum EventTypes : short
    {
        Stakeholder_Created = 1,
        User_Created = 2,
        COI_Created = 3,
        COI_Updated = 4,
        COI_Deleted = 5,
        News_Created = 6,
        News_Updated = 7,
        News_Deleted = 8,
        COI_Type_Created = 9,
        COI_Type_Updated = 10,
        COI_Type_Deleted = 11,
        News_Type_Created = 12,
        News_Type_Updated = 13,
        News_Type_Deleted = 14,
        Drawing_Created = 15,
        Drawing_Updated = 16,
        Drawing_Deleted = 17,
        User_Signed_In = 18,
        User_Signed_Out = 19,
        User_Deleted = 20,
        Stakeholder_Deleted = 21,
        User_Account_Locked_Out = 22,
        Invalid_Login_Attempt = 23,
        Stakeholder_Updated = 24,
        Threat_Level_Created = 25,
        Threat_Level_Updated = 26,
        Threat_Level_Deleted = 27,
        User_Updated = 28,
        COI_Accepted = 29,
        COI_Declined = 30
    }

    public enum NotificationTypes : short
    {
        PR = 1,
        SR = 2,
        COI = 3,
        AR = 4,
        DR = 5,
        LR = 6,
        AAR = 7,
        Template = 8
    }
}
