using System;
using Nop.Core;
using Nop.Plugin.Misc.IssueManagement.Models;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public interface IIssueModelFactory
    {
        AddIssueModel PrepareAddIssueModel(AddIssueModel model);

        IssueSearchModel PrepareIssueSearchModel();

        IssueListModel PrepareIssueListModel(IssueSearchModel model);
    }
}
