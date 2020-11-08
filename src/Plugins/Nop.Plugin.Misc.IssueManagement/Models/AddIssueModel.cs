using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Plugin.Payments.Manual.Validators;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public partial class AddIssueModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Deadline")]
        public DateTime? Deadline { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Priority")]
        public IssuePriority? Priority { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Issue.Add.BasicInfoPanel.Status")]
        public IssueStatus? Status { get; set; }

        public SelectList PrioritySelectList { get; set; }

        public SelectList StatusSelectList { get; set; }
    }
}