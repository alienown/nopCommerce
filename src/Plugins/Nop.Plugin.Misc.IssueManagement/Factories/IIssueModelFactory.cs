using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public interface IIssueModelFactory
    {
        AddIssueModel PrepareAddIssueModel(AddIssueModel model);

        EditIssueModel PrepareEditIssueModel(EditIssueModel model, Issue entity);

        EditBasicInfoPanelModel PrepareEditBasicInfoPanelModel(EditBasicInfoPanelModel model, Issue entity);

        IssueSearchModel PrepareIssueSearchModel();

        IssueListModel PrepareIssueListModel(IssueSearchModel model);

        IssuePersonInvolvedListModel PrepareIssuePersonInvolvedListModel(IssuePersonsInvolvedSearchModel searchModel);

        IssueAssignmentListModel PrepareIssueAssignmentListModel(IssueAssignmentsSearchModel searchModel);

        List<SelectListItem> GetPersonsInvolvedForAddComboBox(string text, int excludePersonsFromIssueId);

        List<SelectListItem> GetAssignmentsForAddComboBox(string text, IssueAssignmentType issueAssignmentType, int excludeAssignmentsFromIssueId);

        IssueHistoryListModel PrepareIssueHistoryListModel(IssueHistorySearchModel searchModel);

        IssueCommentListModel PrepareIssueCommentListModel(IssueCommentsSearchModel searchModel);
    }
}
