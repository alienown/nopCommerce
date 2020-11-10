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
using System.Collections.Generic;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public class IssueModelFactory : IIssueModelFactory
    {
        private readonly IIssueService _issueService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;

        public IssueModelFactory(IIssueService issueService, ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, IProductService productService)
        {
            _issueService = issueService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _productService = productService;
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
            model.PersonInvolvedSearchModel = PrepareIssuePersonInvolvedSearchModel();
            model.AssignmentSearchModel = PrepareIssueAssignmentSearchModel();

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

        public IssuePersonInvolvedSearchModel PrepareIssuePersonInvolvedSearchModel()
        {
            var model = new IssuePersonInvolvedSearchModel();
            return model;
        }

        public IssuePersonInvolvedListModel PrepareIssuePersonInvolvedListModel(IssuePersonInvolvedSearchModel searchModel)
        {
            var personInvolvedList = _issueService.GetPersonInvolvedList(searchModel.Page - 1, searchModel.PageSize);
            var customersIds = personInvolvedList.Select(x => x.CustomerId);
            var customers = _customerService.GetCustomersByIds(customersIds.ToArray());
            var customerIdToCustomer = customers.ToDictionary(x => x.Id);

            var model = new IssuePersonInvolvedListModel().PrepareToGrid(searchModel, personInvolvedList, () =>
            {
                var items = personInvolvedList.Select(x =>
                {
                    var item = new IssuePersonInvolvedGridItem();

                    var customerFullName = _customerService.GetCustomerFullName(new Customer
                    {
                        Id = x.CreatedBy,
                    });

                    item.Id = x.Id;
                    item.Name = customerFullName;
                    item.Email = customerIdToCustomer[x.CreatedBy].Email;

                    return item;
                });

                return items;
            });

            return model;
        }

        public IssueAssignmentSearchModel PrepareIssueAssignmentSearchModel()
        {
            var model = new IssueAssignmentSearchModel();
            return model;
        }

        public IssueAssignmentListModel PrepareIssueAssignmentListModel(IssueAssignmentSearchModel searchModel)
        {
            var assignmentList = _issueService.GetAssignmentList(searchModel.Page - 1, searchModel.PageSize);
            var preload = GetAssignmentsInfoPreload(assignmentList.ToList());

            var model = new IssueAssignmentListModel().PrepareToGrid(searchModel, assignmentList, () =>
            {
                var items = assignmentList.Select(x =>
                {
                    var item = new IssueAssignmentGridItem();

                    item.Id = x.Id;
                    item.ObjectId = x.ObjectId;

                    switch (item.Type)
                    {
                        case IssueAssignmentType.Product:
                            if (preload.Products.TryGetValue(item.Id, out var productInfo))
                            {
                                item.ProductInfo = productInfo;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    return item;
                });

                return items;
            });

            return model;
        }

        private IssueAssignmentPreloadContainer GetAssignmentsInfoPreload(List<IssueAssignment> assignments)
        {
            var assignmentTypeToAssignments = assignments.ToLookup(x => x.AssignmentType);

            var productsIds = assignmentTypeToAssignments[IssueAssignmentType.Product].Select(x => x.ObjectId.Value).ToList();
            var productsInfo = GetProductAssignmentsInfo(productsIds);

            var container = new IssueAssignmentPreloadContainer
            {
                Products = productsInfo,
            };

            return container;
        }

        private Dictionary<int, IssueProductAssignmentInfo> GetProductAssignmentsInfo(List<int> productsIds)
        {
            var productIdToProductInfo = _productService.GetProductsByIds(productsIds.ToArray()).ToDictionary(x => x.Id, y => new IssueProductAssignmentInfo
            {
                Name = y.Name,
            });

            return productIdToProductInfo;
        }
    }
}
