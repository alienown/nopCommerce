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

        public IssueService()
        {

        }

        public Issue GetIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            return issue;
        }
    }
}
