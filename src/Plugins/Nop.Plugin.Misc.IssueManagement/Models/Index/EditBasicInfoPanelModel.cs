using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class EditBasicInfoPanelModel : BaseNopEntityModel
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
    }
}
