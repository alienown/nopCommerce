using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueAssignmentsSearchModel : BaseSearchModel
    {
        public int IssueId { get; set; }
    }
}