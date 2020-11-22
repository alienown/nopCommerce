using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryPersonInvolvedDetails : IssueHistoryChangeDetails
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}
