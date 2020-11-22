using System;
using Nop.Core;
using Nop.Core.Domain.Common;

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
