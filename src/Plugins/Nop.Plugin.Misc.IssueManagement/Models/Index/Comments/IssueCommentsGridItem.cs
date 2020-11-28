using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueCommentsGridItem : BaseNopEntityModel
    {
        public string Content { get; set; }

        public string CreatedByFullName { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool CanDelete { get; set; }
    }
}