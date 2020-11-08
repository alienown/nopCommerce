using System;
using Nop.Core;
using Nop.Services;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public class IssueModelFactory : IIssueModelFactory
    {
        private readonly IIssueService _issueService;

        public IssueModelFactory(IIssueService issueService)
        {
            _issueService = issueService;
        }

        public AddIssueModel PrepareAddIssueModel(AddIssueModel model)
        {
            if (model == null)
            {
                model = new AddIssueModel();
            }

            model.PrioritySelectList = IssuePriority.Normal.ToSelectList();
            model.StatusSelectList = IssueStatus.New.ToSelectList();

            return model;
        }
    }
}
