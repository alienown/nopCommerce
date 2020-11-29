using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryVendorAssignmentDetails : IssueHistoryAssignmentDetails
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }
}