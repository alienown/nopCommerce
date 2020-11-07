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
            this._webHelper = webHelper;
            this._localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Issue/Configure";
        }

        public override void Install()
        {
            _localizationService.AddLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.IssueManagement.Issue"] = "Issue",
                ["Plugins.Misc.IssueManagement.IssueManagement"] = "Issue management",
                ["Plugins.Misc.IssueManagement.NewIssue"] = "New issue",
                ["Plugins.Misc.IssueManagement.AllIssues"] = "All issues",
            });

            base.Install();
        }

        public override void Uninstall()
        {
            _localizationService.DeleteLocaleResources("Plugins.Misc.IssueManagement");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var issueManagementRootMenuItem = new SiteMapNode()
            {
                SystemName = "Issue management",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.IssueManagement"),
                IconClass = "fa-check-square",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var allIssuesMenuItem = new SiteMapNode()
            {
                SystemName = "All issues",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.AllIssues"),
                IconClass = "fa-list",
                ControllerName = "Issue",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var newIssueMenuItem = new SiteMapNode()
            {
                SystemName = "New issue",
                Title = _localizationService.GetResource("Plugins.Misc.IssueManagement.NewIssue"),
                IconClass = "fa-plus",
                ControllerName = "Issue",
                ActionName = "New",
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
