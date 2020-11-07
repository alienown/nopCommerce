using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Services
{
    public class IssueService : IIssueService
    {
        private readonly IRepository<Issue> _issueRepository;

        public IssueService()
        {

        }

        public Issue GetIssue(int id)
        {
            throw new NotImplementedException();
        }
    }
}
