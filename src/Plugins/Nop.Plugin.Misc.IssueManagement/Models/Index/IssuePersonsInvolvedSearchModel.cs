using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssuePersonsInvolvedSearchModel : BaseSearchModel
    {
        public int IssueId { get; set; }
    }
}