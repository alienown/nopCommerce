using System;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.TaskManagement.Domain
{
    public class IssueHistory : BaseEntity
    {
        public int IssueId { get; set; }

        public IssueChangeType ChangeType { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
