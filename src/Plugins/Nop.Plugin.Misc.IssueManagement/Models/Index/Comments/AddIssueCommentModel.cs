using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class AddIssueCommentModel : BaseNopEntityModel
    {
        public int IssueId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.CommentsPanel.AddCommentSubPanel.Comment")]
        public string Content { get; set; }
    }
}
