using System.ComponentModel;

namespace GoodDeal.API.Auth
{
    public enum CustomerClaim
    {
        [Description("tenantId")]
        TenantId = 0,

        [Description("id")]
        Id,

        [Description("firstName")]
        FirstName,

        [Description("lastName")]
        LastName,

        [Description("email")]
        Email,

        [Description("prefix")]
        Prefix,

        [Description("phone")]
        Phone
    }
};

