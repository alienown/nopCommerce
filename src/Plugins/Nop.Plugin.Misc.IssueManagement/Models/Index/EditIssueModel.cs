using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class EditIssueModel : BaseNopEntityModel
    {
        public EditBasicInfoPanelModel BasicInfoPanelModel { get; set; }

        public EditIssuePersonsInvolvedPanelModel PersonsInvolvedPanelModel { get; set; }

        public EditIssueAssignmentsPanelModel AssignmentsPanelModel { get; set; }
    }
}