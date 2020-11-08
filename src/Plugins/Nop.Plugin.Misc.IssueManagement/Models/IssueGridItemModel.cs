using System;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueGridItemModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public DateTime? Deadline { get; set; }

        public IssuePriority Priority { get; set; }

        public string PriorityDisplay { get; set; }

        public IssueStatus Status { get; set; }

        public string StatusDisplay { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }
    }
}