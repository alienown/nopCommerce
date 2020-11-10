using System;
using Nop.Core;

namespace Nop.Plugin.Misc.IssueManagement.Domain
{
    public class IssuePersonInvolved : BaseEntity
    {
        public int CustomerId { get; set; }

        public int IssueId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
