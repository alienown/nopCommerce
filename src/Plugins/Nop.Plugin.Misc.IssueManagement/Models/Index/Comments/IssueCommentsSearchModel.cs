using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueCommentsSearchModel : BaseSearchModel
    {
        public int IssueId { get; set; }
    }
}