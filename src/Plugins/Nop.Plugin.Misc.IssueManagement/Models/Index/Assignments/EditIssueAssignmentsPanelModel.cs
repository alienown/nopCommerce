using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class EditIssueAssignmentsPanelModel
    {
        public IssueAssignmentsSearchModel AssignmentsSearchModel { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.Assignment")]
        public int? ObjectId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.AssignmentsPanel.AddAssignmentsSubPanel.AssignmentType")]
        public IssueAssignmentType AssignmentType { get; set; }

        public SelectList AssignmentTypesSelectList { get; set; }

        public bool CanEdit { get; set; }
    }
}
