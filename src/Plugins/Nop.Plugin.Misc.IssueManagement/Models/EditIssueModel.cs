using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class EditIssueModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Deadline")]
        [UIHint("DateNullable")]
        public DateTime? Deadline { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Priority")]
        public IssuePriority? Priority { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.Status")]
        public IssueStatus? Status { get; set; }

        public SelectList PrioritySelectList { get; set; }

        public SelectList StatusSelectList { get; set; }

        public IssuePersonInvolvedSearchModel PersonInvolvedSearchModel { get; set; }

        public IssueAssignmentSearchModel AssignmentSearchModel { get; set; }
    }
}