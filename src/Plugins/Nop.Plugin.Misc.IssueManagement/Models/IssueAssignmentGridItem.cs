using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueAssignmentGridItem : BaseNopEntityModel
    {
        public int? ObjectId { get; set; }

        public IssueProductAssignmentInfo ProductInfo { get; set; }

        public IssueAssignmentType Type { get; set; }
    }
}