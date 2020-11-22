using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryChangeDetails
    {
        public int IssueHistoryId { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }
    }
}