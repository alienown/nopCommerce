using System;
using Nop.Core;
using Nop.Services;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Customers;
using Nop.Services.Vendors;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using System.Collections.Generic;
using Nop.Services.Helpers;
using Nop.Services.Localization;

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

        public IssueSearchModel PrepareIssueSearchModel()
        {
            var model = new IssueSearchModel();
            model.SetGridPageSize();
            return model;
        }

        public IssueListModel PrepareIssueListModel(IssueSearchModel searchModel)
        {
            var issueList = _issueService.GetIssueList(searchModel.Page - 1, searchModel.PageSize);

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
