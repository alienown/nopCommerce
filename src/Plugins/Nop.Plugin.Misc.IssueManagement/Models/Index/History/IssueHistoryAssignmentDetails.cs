using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryAssignmentDetails : IssueHistoryChangeDetails
    {
        public IssueAssignmentType AssignmentType { get; set; }
    }
}