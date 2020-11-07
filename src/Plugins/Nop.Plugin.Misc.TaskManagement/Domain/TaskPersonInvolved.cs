using System;
using Nop.Core;

namespace Nop.Plugin.Misc.TaskManagement.Domain
{
    public class TaskPersonInvolved : BaseEntity
    {
        public int UserId { get; set; }

        public int TaskId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
