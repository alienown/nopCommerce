using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public interface IIssueService
    {
        Issue GetIssue(int id);

        void InsertIssue(Issue issue);
    }
}
