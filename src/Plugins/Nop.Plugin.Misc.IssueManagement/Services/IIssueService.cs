using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public interface IIssueService
    {
        Issue GetIssue(int id);

        IPagedList<Issue> GetIssueList(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        void InsertIssue(Issue issue);
    }
}
