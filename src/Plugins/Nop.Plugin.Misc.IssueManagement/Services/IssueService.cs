using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public class IssueService : IIssueService
    {
        private readonly IRepository<Issue> _issueRepository;

        public IssueService(IRepository<Issue> issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public Issue GetIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            return issue;
        }

        public IPagedList<Issue> GetIssueList(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueRepository.GetAllPaged(query =>
            {
                query = query.Where(x => !x.Deleted);
                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return list;
        }

        public void InsertIssue(Issue issue)
        {
            _issueRepository.Insert(issue);
        }
    }
}
