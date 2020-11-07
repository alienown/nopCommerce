using System;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.TaskManagement.Domain
{
    public class TaskComment : BaseEntity
    {
        public int TaskId { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
