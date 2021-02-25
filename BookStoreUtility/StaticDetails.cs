using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreUtility
{
    public static class StaticDetails
    {
        public const string ProcCreateCoverType = "usp_CreateCoverType";
        public const string ProcGetCoverType = "usp_GetCoverType";
        public const string ProcGetAllCoverType = "usp_GetCoverTypes";
        public const string ProcUpdateCoverType = "usp_UpdateCoverType";
        public const string ProcDeleteCoverType = "usp_DeleteCoverType";

        public const string Role_User_Independent = "Independent Customer";
        public const string Roler_User_Company = "Company Customer";
        public const string Role_Admin = "Administrator";
        public const string Role_Employee = "Employee";
    }
}
