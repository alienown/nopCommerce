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
using Microsoft.AspNetCore.Mvc.Rendering;

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
                    BasicInfoPanelModel = PrepareEditBasicInfoPanelModel(null, entity),
                };
            }
            else
            {
                model.BasicInfoPanelModel = PrepareEditBasicInfoPanelModel(model.BasicInfoPanelModel, null);
            }

            model.PersonsInvolvedPanelModel = PrepareEditIssuePersonsInvolvedPanelModel(model);
            model.AssignmentsPanelModel = PrepareEditIssueAssignmentsPanelModel(model);

            return model;
        }

        public EditBasicInfoPanelModel PrepareEditBasicInfoPanelModel(EditBasicInfoPanelModel model, Issue entity)
        {
            if (model == null)
            {
                model = new EditBasicInfoPanelModel
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

        private EditIssuePersonsInvolvedPanelModel PrepareEditIssuePersonsInvolvedPanelModel(EditIssueModel model)
        {
            var panelModel = new EditIssuePersonsInvolvedPanelModel();
            panelModel.PersonsInvolvedSearchModel = PrepareIssuePersonsInvolvedSearchModel(model);
            return panelModel;
        }

        private IssuePersonsInvolvedSearchModel PrepareIssuePersonsInvolvedSearchModel(EditIssueModel model)
        {
            var searchModel = new IssuePersonsInvolvedSearchModel { IssueId = model.Id };
            return searchModel;
        }

        public IssuePersonInvolvedListModel PrepareIssuePersonInvolvedListModel(IssuePersonsInvolvedSearchModel searchModel)
        {
            var personInvolvedList = _issueService.GetPersonInvolvedList(searchModel.IssueId, searchModel.Page - 1, searchModel.PageSize);
            var customersIds = personInvolvedList.Select(x => x.CustomerId);
            var customers = _customerService.GetCustomersByIds(customersIds.ToArray());
            var customerIdToCustomer = customers.ToDictionary(x => x.Id);

            var model = new IssuePersonInvolvedListModel().PrepareToGrid(searchModel, personInvolvedList, () =>
            {
                var items = personInvolvedList.Select(x =>
                {
                    var item = new IssuePersonsInvolvedGridItem();

                    var customerFullName = _customerService.GetCustomerFullName(new Customer
                    {
                        Id = x.CustomerId,
                    });

                    item.Id = x.Id;
                    item.Name = customerFullName;
                    item.Email = customerIdToCustomer[x.CustomerId].Email;

                    return item;
                });

                return items;
            });

            return model;
        }

        private EditIssueAssignmentsPanelModel PrepareEditIssueAssignmentsPanelModel(EditIssueModel model)
        {
            var panelModel = new EditIssueAssignmentsPanelModel();
            panelModel.AssignmentTypesSelectList = IssueAssignmentType.Product.ToSelectList();
            panelModel.AssignmentsSearchModel = PrepareIssueAssignmentsSearchModel(model);
            return panelModel;
        }

        private IssueAssignmentsSearchModel PrepareIssueAssignmentsSearchModel(EditIssueModel model)
        {
            var searchModel = new IssueAssignmentsSearchModel { IssueId = model.Id };
            return searchModel;
        }

        public IssueAssignmentListModel PrepareIssueAssignmentListModel(IssueAssignmentsSearchModel searchModel)
        {
            var assignmentList = _issueService.GetAssignmentList(searchModel.IssueId, null, searchModel.Page - 1, searchModel.PageSize);
            var preload = GetAssignmentsInfoPreload(assignmentList.ToList());

            var model = new IssueAssignmentListModel().PrepareToGrid(searchModel, assignmentList, () =>
            {
                var items = assignmentList.Select(x =>
                {
                    var item = new IssueAssignmentsGridItem();

                    item.Id = x.Id;
                    item.ObjectId = x.ObjectId;

                    switch (item.AssignmentType)
                    {
                        case IssueAssignmentType.Product:
                            if (preload.Products.TryGetValue(item.ObjectId.Value, out var productInfo))
                            {
                                item.ProductInfo = productInfo;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    return item;
                }).OrderBy(x => x.AssignmentType).ToList();

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

        public List<SelectListItem> GetPersonsInvolvedForAddComboBox(string text, int excludePersonsFromIssueId)
        {
            var existingPersonsInvolved = _issueService.GetPersonInvolvedList(excludePersonsFromIssueId);
            var existingPersonsInvolvedIds = existingPersonsInvolved.Select(x => x.CustomerId).ToList();
            var personsInvolved = _issueService.QuickSearchCustomers(text, existingPersonsInvolvedIds);

            var list = personsInvolved.Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = $"{x.FirstName} {x.LastName} ({x.Email})",
            }).ToList();

            return list;
        }

        public List<SelectListItem> GetAssignmentsForAddComboBox(string text, IssueAssignmentType issueAssignmentType, int excludeAssignmentsFromIssueId)
        {
            var existingAssignmentsOfType = _issueService.GetAssignmentList(excludeAssignmentsFromIssueId, issueAssignmentType);
            var existingAssignmentsOfTypeIds = existingAssignmentsOfType.Select(x => x.Id).ToList();
            var assignments = _issueService.QuickSearchAssignments(text, issueAssignmentType, existingAssignmentsOfTypeIds);

            var list = assignments.Select(x => new SelectListItem
            {
                Value = x.ObjectId.HasValue ? x.ObjectId.ToString() : string.Empty,
                Text = x.Name,
            }).ToList();

            return list;
        }
    }
}
