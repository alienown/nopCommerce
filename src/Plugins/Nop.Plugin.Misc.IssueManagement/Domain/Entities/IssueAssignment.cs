using System;
using Nop.Core;

namespace Nop.Plugin.Misc.IssueManagement.Domain
{
    public class IssueAssignment : BaseEntity
    {
        public int ObjectId { get; set; }

        public int IssueId { get; set; }

        public IssueAssignmentType AssignmentType { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
