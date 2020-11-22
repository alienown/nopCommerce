namespace Nop.Plugin.Misc.IssueManagement.Domain
{
    public class IssueHistoryAssignmentValue
    {
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public IssueAssignmentType AssignmentType { get; set; }
    }
}