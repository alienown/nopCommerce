using System;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.TaskManagement.Domain
{
    public class TaskAssignment : BaseEntity
    {
        public int? ObjectId { get; set; }

        public int TaskId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
