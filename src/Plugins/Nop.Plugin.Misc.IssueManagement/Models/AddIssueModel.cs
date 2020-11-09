﻿using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class AddIssueModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Deadline")]
        [UIHint("DateNullable")]
        public DateTime? Deadline { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Priority")]
        public IssuePriority? Priority { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.Status")]
        public IssueStatus? Status { get; set; }

        public SelectList PrioritySelectList { get; set; }

        public SelectList StatusSelectList { get; set; }
    }
}