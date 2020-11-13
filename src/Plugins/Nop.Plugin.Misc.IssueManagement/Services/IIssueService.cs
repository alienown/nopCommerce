using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        IPagedList<IssuePersonInvolved> GetPersonInvolvedList(int issueId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        List<QuickSearchCustomerInfo> QuickSearchCustomers(string text, List<int> idsToExclude = null);

        void InsertPersonInvolved(IssuePersonInvolved issuePersonInvolved);

        void DeletePersonInvolved(int id);

        IPagedList<IssueAssignment> GetAssignmentList(int issueId, IssueAssignmentType? assignmentType, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        List<QuickSearchAssignmentInfo> QuickSearchAssignments(string text, IssueAssignmentType assignmentType, List<int> idsToExclude = null);

        void InsertAssignment(IssueAssignment issueAssignment);

        void DeleteAssignment(int id);
    }
}
