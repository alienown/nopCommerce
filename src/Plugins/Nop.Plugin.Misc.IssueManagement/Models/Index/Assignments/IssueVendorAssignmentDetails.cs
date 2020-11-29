namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueVendorAssignmentDetails : IssueAssignmentDetails
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public bool CanViewEditPage { get; set; }
    }
}
