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

        IPagedList<Issue> GetIssueList(string name, List<IssuePriority> priorities, List<IssueStatus> statuses, DateTime? deadlineFrom, DateTime? deadlineTo,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        void InsertIssue(Issue issue);

        void UpdateIssue(Issue issue);

        void DeleteIssue(int id);

        IPagedList<IssuePersonInvolved> GetPersonInvolvedList(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        void InsertPersonInvolved(IssuePersonInvolved issuePersonInvolved);

        void DeletePersonInvolved(int id);

        IPagedList<IssueAssignment> GetAssignmentList(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        void InsertAssignment(IssueAssignment issueAssignment);

        void DeleteAssignment(int id);
    }
}
