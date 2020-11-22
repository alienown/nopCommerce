using System;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class IssueHistoryGridItem : BaseNopEntityModel
    {
        public IssueChangeType ChangeType { get; set; }

        public string ModifiedByFullName { get; set; }

        public DateTime ModifiedAt { get; set; }

        public IssueHistoryChangeDetails ChangeDetails { get; set; }
    }
}