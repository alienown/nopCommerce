using System;
using System.Collections.Generic;
using System.Text;
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

        public void InsertIssue(Issue issue)
        {
            _issueRepository.Insert(issue);
        }
    }
}
