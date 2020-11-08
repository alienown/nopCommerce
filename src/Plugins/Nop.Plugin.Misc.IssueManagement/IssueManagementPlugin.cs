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
                ["Plugins.Misc.IssueManagement.Issue.Add.AddNewIssue"] = "Add new issue",
                ["Plugins.Misc.IssueManagement.Issue.Add.ToIssueList"] = "to issue list",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Title"] = "Basic info",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Description"] = "Description",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Deadline"] = "Deadline",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Priority"] = "Priority",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Status"] = "Status",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.NameNotEmptyValidationMessage"] = "Name is required",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.DescriptionNotEmptyValidationMessage"] = "Description is required",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.PriorityNotEmptyValidationMessage"] = "Priority is required",
                ["Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.StatusNotEmptyValidationMessage"] = "Status is required",
                ["Plugins.Misc.IssueManagement.Issue.List.Title"] = "All issues",
                ["Plugins.Misc.IssueManagement.Issue.List.Issues"] = "Issues",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.Name"] = "Name",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.Status"] = "Status",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.Priority"] = "Priority",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.Deadline"] = "Deadline",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.CreatedBy"] = "Created by",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.CreatedAt"] = "Created at",
                ["Plugins.Misc.IssueManagement.Issue.List.Grid.Columns.LastModified"] = "Last activity",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Low"] = "Low",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Normal"] = "Normal",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Important"] = "Important",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Urgent"] = "Urgent",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssuePriority.Critical"] = "Critical",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.New"] = "New",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.InProgress"] = "In progress",
                ["Enums.Nop.Plugin.Misc.IssueManagement.Domain.IssueStatus.Closed"] = "Closed",
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
