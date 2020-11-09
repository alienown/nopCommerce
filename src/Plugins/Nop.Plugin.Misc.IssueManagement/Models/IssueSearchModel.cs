using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Plugin.Payments.Manual.Validators;
using iTextSharp.text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueSearchModel : BaseSearchModel
    {
        public IssueSearchModel()
        {
            SearchIssuePriority = new List<int>();
            SearchIssueStatus = new List<int>();
        }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.List.Filters.SearchIssueName")]
        public string SearchIssueName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.List.Filters.SearchIssuePriority")]
        public IList<int> SearchIssuePriority { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.List.Filters.SearchIssueStatus")]
        public IList<int> SearchIssueStatus { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.List.Filters.SearchDeadlineFrom")]
        [UIHint("DateNullable")]
        public DateTime? SearchDeadlineFrom { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.List.Filters.SearchDeadlineTo")]
        [UIHint("DateNullable")]
        public DateTime? SearchDeadlineTo { get; set; }

        public IList<SelectListItem> StatusSelectList { get; set; }

        public IList<SelectListItem> PrioritySelectList { get; set; }
    }
}