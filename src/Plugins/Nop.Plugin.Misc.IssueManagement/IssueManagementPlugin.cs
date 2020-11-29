using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.IssueManagement
{
    public class IssueManagementPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public IssueManagementPlugin(IWebHelper webHelper, ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Issue/Configure";
        }

        public override void Install()
        {
            _localizationService.AddLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.IssueManagement.Menu.IssueManagement"] = "Issue management",
                ["Plugins.Misc.IssueManagement.Menu.NewIssue"] = "New issue",
                ["Plugins.Misc.IssueManagement.Menu.AllIssues"] = "All issues",
                ["Plugins.Misc.IssueManagement.Add.Title"] = "New issue",
                ["Plugins.Misc.IssueManagement.Add.AddSuccess"] = "The issue has been successfully added",
                ["Plugins.Misc.IssueManagement.Add.AddNewIssue"] = "Add new issue",
                ["Plugins.Misc.IssueManagement.Add.ToIssueList"] = "to issue list",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Title"] = "Basic info",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Description"] = "Description",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Deadline"] = "Deadline",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Priority"] = "Priority",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Status"] = "Status",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.NameNotEmptyValidationMessage"] = "Name is required",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.DescriptionNotEmptyValidationMessage"] = "Description is required",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.PriorityNotEmptyValidationMessage"] = "Priority is required",
                ["Plugins.Misc.IssueManagement.Add.BasicInfoPanel.StatusNotEmptyValidationMessage"] = "Status is required",
                ["Plugins.Misc.IssueManagement.Delete.DeleteSuccess"] = "The issue has been successfully deleted",
                ["Plugins.Misc.IssueManagement.Edit.Title"] = "Issue",
                ["Plugins.Misc.IssueManagement.Edit.UpdateSuccess"] = "The issue has been successfully updated",
                ["Plugins.Misc.IssueManagement.Edit.ToIssueList"] = "to issue list",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Title"] = "Basic info",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Description"] = "Description",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Deadline"] = "Deadline",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Priority"] = "Priority",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Status"] = "Status",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.NameNotEmptyValidationMessage"] = "Name is required",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.DescriptionNotEmptyValidationMessage"] = "Description is required",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.PriorityNotEmptyValidationMessage"] = "Priority is required",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.StatusNotEmptyValidationMessage"] = "Status is required",
                ["Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.SaveButton"] = "Save",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.Title"] = "Persons involved",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.Grid.Columns.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.Grid.Columns.Email"] = "Email",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.AddPersonsInvolvedSubPanel.Title"] = "Add person",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.AddPersonsInvolvedSubPanel.Person"] = "Person",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.AddPersonsInvolvedSubPanel.AddPersonButton"] = "Add person",
                ["Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.AddPersonsInvolvedSubPanel.AddPersonComboBox.Placeholder"] = "Please type in at least 3 letters...",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.Title"] = "Assignments",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.Grid.Columns.Assignment"] = "Assignment",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.Title"] = "Add assignment",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.Assignment"] = "Assignment",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.AssignmentType"] = "Type",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.AddAssignmentButton"] = "Add assignment",
                ["Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.AddAssignmentComboBox.Placeholder"] = "Please type in at least 3 letters...",
                ["Plugins.Misc.IssueManagement.Edit.HistoryPanel.Title"] = "History",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change"] = "Change",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.ChangedBy"] = "Changed by",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.ChangedOn"] = "Changed on",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change.Added"] = "Added",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change.Removed"] = "Removed",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change.Changed"] = "Changed",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change.From"] = "from",
                ["Plugins.Misc.IssueManagement.Edit.Historypanel.Grid.Columns.Change.To"] = "to",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.Title"] = "Comments",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.Grid.Columns.Comment"] = "Comment",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.Grid.Columns.CreatedBy"] = "Created by",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.Grid.Columns.CreatedAt"] = "Created at",
                ["Plugins.Misc.Issuemanagement.Edit.CommentsPanel.AddCommentSubPanel.Title"] = "Add comment",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.AddCommentSubPanel.Comment"] = "Comment",
                ["Plugins.Misc.Issuemanagement.Edit.CommentsPanel.AddCommentSubPanel.AddCommentButton"] = "Add comment",
                ["Plugins.Misc.IssueManagement.Edit.CommentsPanel.AddCommentsSubPanel.CommentRequiredValidationMessage"] = "Comment can't be empty",
                ["Plugins.Misc.IssueManagement.List.Title"] = "All issues",
                ["Plugins.Misc.IssueManagement.List.Issues"] = "Issues",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.Status"] = "Status",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.Priority"] = "Priority",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.Deadline"] = "Deadline",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.CreatedBy"] = "Created by",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.CreatedAt"] = "Created at",
                ["Plugins.Misc.IssueManagement.List.Grid.Columns.LastModified"] = "Last activity",
                ["Plugins.Misc.IssueManagement.List.Filters.SearchIssueName"] = "Name",
                ["Plugins.Misc.IssueManagement.List.Filters.SearchIssuePriority"] = "Priority",
                ["Plugins.Misc.IssueManagement.List.Filters.SearchIssueStatus"] = "Status",
                ["Plugins.Misc.IssueManagement.List.Filters.SearchDeadlineFrom"] = "Deadline from",
                ["Plugins.Misc.IssueManagement.List.Filters.SearchDeadlineTo"] = "Deadline to",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Low"] = "Low",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Normal"] = "Normal",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Important"] = "Important",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Urgent"] = "Urgent",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Critical"] = "Critical",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.New"] = "New",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.InProgress"] = "In progress",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.Closed"] = "Closed",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueAssignmentType.Product"] = "Product",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueAssignmentType.Vendor"] = "Vendor",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueChangeType.Name"] = "Name",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueChangeType.Description"] = "Description",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueChangeType.Deadline"] = "Deadline",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueChangeType.Priority"] = "Priority",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueChangeType.Status"] = "Status",
            });

            base.Install();
        }

        public override void Uninstall()
        {
            _localizationService.DeleteLocaleResources("Plugins.Misc.IssueManagement");
            _localizationService.DeleteLocaleResources("Enums.Nop.Plugin.Misc.IssueManagement");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var issueManagementRootMenuItem = new SiteMapNode()
            {
                SystemName = "Issue management",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.Menu.IssueManagement"),
                IconClass = "fa-check-square",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var allIssuesMenuItem = new SiteMapNode()
            {
                SystemName = "All issues",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.Menu.AllIssues"),
                IconClass = "fa-list",
                ControllerName = "Issue",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var newIssueMenuItem = new SiteMapNode()
            {
                SystemName = "New issue",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.Menu.NewIssue"),
                IconClass = "fa-plus",
                ControllerName = "Issue",
                ActionName = "Add",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var customersMenuItem = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Customers");
            if (customersMenuItem != null)
            {
                rootNode.ChildNodes.Insert(rootNode.ChildNodes.IndexOf(customersMenuItem) + 1, issueManagementRootMenuItem);
            }
            else
            {
                rootNode.ChildNodes.Add(issueManagementRootMenuItem);
            } 

            issueManagementRootMenuItem.ChildNodes.Add(allIssuesMenuItem);
            issueManagementRootMenuItem.ChildNodes.Add(newIssueMenuItem);
        }
    }
}
