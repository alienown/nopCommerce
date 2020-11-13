using Nop.Core;

namespace Nop.Plugin.Misc.IssueManagement.Domain
{
    public class QuickSearchCustomerInfo
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
