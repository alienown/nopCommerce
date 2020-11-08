using System;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.IssueManagement.Domain
{
    public class Issue : BaseEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        public IssuePriority Priority { get; set; }

        public IssueStatus Status { get; set; }

        public bool Deleted { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }
    }
}
