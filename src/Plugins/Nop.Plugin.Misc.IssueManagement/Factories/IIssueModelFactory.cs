using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public interface IIssueModelFactory
    {
        AddIssueModel PrepareAddIssueModel(AddIssueModel model);

        EditIssueModel PrepareEditIssueModel(EditIssueModel model, Issue entity);

        IssueSearchModel PrepareIssueSearchModel();

        IssueListModel PrepareIssueListModel(IssueSearchModel model);

        IssuePersonInvolvedListModel PrepareIssuePersonInvolvedListModel(IssuePersonInvolvedSearchModel searchModel);

        IssuePersonInvolvedSearchModel PrepareIssuePersonInvolvedSearchModel();

        IssueAssignmentListModel PrepareIssueAssignmentListModel(IssueAssignmentSearchModel searchModel);

        IssueAssignmentSearchModel PrepareIssueAssignmentSearchModel();
    }
}
