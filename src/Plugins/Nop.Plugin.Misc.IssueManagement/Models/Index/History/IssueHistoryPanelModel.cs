using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryPanelModel
    {
        public IssueHistorySearchModel HistorySearchModel { get; set; }
    }
}
