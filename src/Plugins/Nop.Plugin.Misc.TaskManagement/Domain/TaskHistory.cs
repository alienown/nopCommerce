using System;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.TaskManagement.Domain
{
    public class TaskHistory : BaseEntity
    {
        public int TaskId { get; set; }

        public TaskChangeType ChangeType { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
