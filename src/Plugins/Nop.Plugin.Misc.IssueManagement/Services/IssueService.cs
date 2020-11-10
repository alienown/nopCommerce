using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public class IssueService : IIssueService
    {
        private readonly IWorkContext _workContext;
        private readonly IRepository<Issue> _issueRepository;
        private readonly IRepository<IssueHistory> _issueHistoryRepository;

        public IssueService(IWorkContext workContext, IRepository<Issue> issueRepository, IRepository<IssueHistory> issueHistoryRepository)
        {
            _workContext = workContext;
            _issueRepository = issueRepository;
            _issueHistoryRepository = issueHistoryRepository;
        }

        public Issue GetIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            return issue;
        }

        public IPagedList<Issue> GetIssueList(string name, List<IssuePriority> priorities, List<IssueStatus> statuses, DateTime? deadlineFrom, DateTime? deadlineTo,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueRepository.GetAllPaged(query =>
            {
                query = query.Where(x => !x.Deleted);
                
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(x => name.Contains(x.Name));
                }

                if (priorities != null && priorities.Any())
                {
                    query = query.Where(x => priorities.Contains(x.Priority));
                }

                if (statuses != null && statuses.Any())
                {
                    query = query.Where(x => statuses.Contains(x.Status));
                }

                if (deadlineFrom.HasValue)
                {
                    query = query.Where(x => x.Deadline >= deadlineFrom.Value);
                }

                if (deadlineTo.HasValue)
                {
                    query = query.Where(x => x.Deadline <= deadlineTo.Value);
                }

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return list;
        }

        public void InsertIssue(Issue issue)
        {
            var now = DateTime.UtcNow;
            issue.CreatedBy = _workContext.CurrentCustomer.Id;
            issue.CreatedAt = now;
            issue.LastModified = now;
            _issueRepository.Insert(issue);
        }

        public void UpdateIssue(Issue issue)
        {
            var originalIssue = _issueRepository.GetById(issue.Id);
            issue.CreatedAt = originalIssue.CreatedAt;
            issue.CreatedBy = originalIssue.CreatedBy;
            issue.LastModified = DateTime.UtcNow;
            var changes = GenerateIssueHistoryEntiresForIssueEntity(issue);
            _issueRepository.Update(issue);
            _issueHistoryRepository.Insert(changes);
        }

        private List<IssueHistory> GenerateIssueHistoryEntiresForIssueEntity(Issue modifiedIssue)
        {
            var result = new List<IssueHistory>();
            var now = modifiedIssue.LastModified;
            var customerId = _workContext.CurrentCustomer.Id;
            var originalIssue = _issueRepository.GetById(modifiedIssue.Id);

            if (modifiedIssue.Name != originalIssue.Name)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Name, originalIssue.Name,
                    IssueChangeType.Name, now, customerId));
            }

            if (modifiedIssue.Description != originalIssue.Description)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Description, originalIssue.Description,
                    IssueChangeType.Description, now, customerId));
            }

            if (modifiedIssue.Priority != originalIssue.Priority)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Priority.ToString(), originalIssue.Priority.ToString(),
                    IssueChangeType.Priority, now, customerId));
            }

            if (modifiedIssue.Status != originalIssue.Status)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Status.ToString(), originalIssue.Status.ToString(),
                    IssueChangeType.Status, now, customerId));
            }

            if (modifiedIssue.Deadline != originalIssue.Deadline)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Deadline.ToString(), originalIssue.Deadline.ToString(),
                    IssueChangeType.Deadline, now, customerId));
            }

            return result;
        }

        private IssueHistory CreateIssueHistoryEntry(int issueId, string newValue, string oldValue, IssueChangeType changeType, DateTime modifiedAt, int modifiedBy)
        {
            return new IssueHistory
            {
                IssueId = issueId,
                NewValue = newValue,
                OldValue = oldValue,
                ChangeType = changeType,
                ModifiedAt = modifiedAt,
                ModifiedBy = modifiedBy,
            };
        }

        public void DeleteIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            if (issue != null)
            {
                _issueRepository.Delete(issue);
            }
        }
    }
}
