using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueAssignmentsGridItem : BaseNopEntityModel
    {
        public int ObjectId { get; set; }

        public IssueAssignmentType AssignmentType { get; set; }

        public IssueAssignmentDetails Details { get; set; }
    }
}