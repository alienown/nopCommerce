using System;
using Nop.Services;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using NUglify.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public class IssueModelFactory : IIssueModelFactory
    {
        private readonly IIssueService _issueService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        public IssueModelFactory(IIssueService issueService, ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            _issueService = issueService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
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

        public EditIssueModel PrepareEditIssueModel(EditIssueModel model, Issue entity)
        {
            if (model == null)
            {
                model = new EditIssueModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Priority = entity.Priority,
                    Status = entity.Status,
                    Deadline = entity.Deadline,
                };
            }           

            model.PrioritySelectList = IssuePriority.Normal.ToSelectList();
            model.StatusSelectList = IssueStatus.New.ToSelectList();

            return model;
        }

        public IssueSearchModel PrepareIssueSearchModel()
        {
            var model = new IssueSearchModel();

            model.PrioritySelectList = IssuePriority.Normal.ToSelectList(markCurrentAsSelected: false).Select(x => x).ToList();
            model.StatusSelectList = IssueStatus.New.ToSelectList(markCurrentAsSelected: false).Select(x => x).ToList();
            model.SetGridPageSize();

            return model;
        }

        public IssueListModel PrepareIssueListModel(IssueSearchModel searchModel)
        {
            var issueStatuses = searchModel.SearchIssueStatus.Select(x => (IssueStatus)x).ToList();
            var issueProrities = searchModel.SearchIssuePriority.Select(x => (IssuePriority)x).ToList();

            var issueList = _issueService.GetIssueList(searchModel.SearchIssueName, issueProrities, issueStatuses,
                searchModel.SearchDeadlineFrom, searchModel.SearchDeadlineTo, searchModel.Page - 1, searchModel.PageSize);

            var model = new IssueListModel().PrepareToGrid(searchModel, issueList, () =>
            {
                var items = issueList.Select(x =>
                {
                    var item = x.ToModel<IssueGridItemModel>();

                    var customerFullName = _customerService.GetCustomerFullName(new Customer
                    {
                        Id = x.CreatedBy,
                    });

                    item.StatusDisplay = _localizationService.GetLocalizedEnum(x.Status);
                    item.PriorityDisplay = _localizationService.GetLocalizedEnum(x.Priority);
                    item.CreatedBy = customerFullName;
                    item.CreatedAt = _dateTimeHelper.ConvertToUserTime(x.CreatedAt, DateTimeKind.Utc);
                    item.LastModified = _dateTimeHelper.ConvertToUserTime(x.LastModified, DateTimeKind.Utc);

                    return item;
                });

                return items;
            });

            return model;
        }
    }
}
