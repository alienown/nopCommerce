using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssuePersonInvolvedGridItem : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }
}