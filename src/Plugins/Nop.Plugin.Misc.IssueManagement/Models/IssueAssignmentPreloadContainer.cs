using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueAssignmentPreloadContainer
    {
        public Dictionary<int, IssueProductAssignmentInfo> Products { get; set; }
    }
}
