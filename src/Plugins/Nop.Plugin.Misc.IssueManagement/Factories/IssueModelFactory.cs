using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.IssueManagement.Factories
{
    public class IssueModelFactory : IIssueModelFactory
    {
        private readonly IIssueService _issueService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;

        public IssueModelFactory(IIssueService issueService, ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, IProductService productService, IVendorService vendorService,
            IWorkContext workContext, IPermissionService permissionService)
        {
            _issueService = issueService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _productService = productService;
            _vendorService = vendorService;
            _workContext = workContext;
            _permissionService = permissionService;
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

            var canEdit = _issueService.CanEditIssue(model.Id);
            model.CanEdit = canEdit;

            model.PersonsInvolvedPanelModel = PrepareEditIssuePersonsInvolvedPanelModel(model);
            model.AssignmentsPanelModel = PrepareEditIssueAssignmentsPanelModel(model);
            model.HistoryPanelModel = PrepareIssueHistoryPanelModel(model);
            model.CommentsPanelModel = PrepareIssueCommentsPanelModel(model);

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
                    Deadline = entity.Deadline.HasValue ? _dateTimeHelper.ConvertToUserTime(entity.Deadline.Value, DateTimeKind.Utc) : (DateTime?)null,
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
            panelModel.CanEdit = model.CanEdit && (_workContext.CurrentVendor != null || _permissionService.Authorize(StandardPermissionProvider.ManageCustomers));
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

            var canManageProducts = _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            var canManageVendors = _permissionService.Authorize(StandardPermissionProvider.ManageVendors);

            var assignmentTypesToExclude = new List<int>();
            if (!canManageProducts)
            {
                assignmentTypesToExclude.Add((int)IssueAssignmentType.Product);
            }
            if (!canManageVendors)
            {
                assignmentTypesToExclude.Add((int)IssueAssignmentType.Vendor);
            }

            panelModel.AssignmentTypesSelectList = IssueAssignmentType.Product.ToSelectList(valuesToExclude: assignmentTypesToExclude.ToArray());
            panelModel.AssignmentsSearchModel = PrepareIssueAssignmentsSearchModel(model);
            panelModel.CanEdit = model.CanEdit && (canManageProducts || canManageVendors);
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
            var assignmentsDetails = GetAssignmentsIdsToDetails(assignmentList.ToList());

            var model = new IssueAssignmentListModel().PrepareToGrid(searchModel, assignmentList, () =>
            {
                var items = assignmentList.Select(x =>
                {
                    var item = x.ToModel<IssueAssignmentsGridItem>();

                    if (assignmentsDetails.TryGetValue(item.Id, out var details))
                    {
                        item.Details = details;
                    }

                    return item;
                }).OrderBy(x => x.AssignmentType).ToList();

                return items;
            });

            return model;
        }

        private Dictionary<int, IssueAssignmentDetails> GetAssignmentsIdsToDetails(List<IssueAssignment> assignments)
        {
            var assignmentTypeToAssignments = assignments.ToLookup(x => x.AssignmentType);

            var products = assignmentTypeToAssignments[IssueAssignmentType.Product].ToList();
            var vendors = assignmentTypeToAssignments[IssueAssignmentType.Vendor].ToList();

            var productsInfo = GetAssignmentsIdsToProductDetails(products).Cast<IssueAssignmentDetails>();
            var vendorsInfo = GetAssignmentsIdsToVendorDetails(vendors).Cast<IssueAssignmentDetails>();

            var result = MergeAssignmentsDetailsToDictionary(productsInfo, vendorsInfo);
            return result;
        }

        private Dictionary<int, IssueAssignmentDetails> MergeAssignmentsDetailsToDictionary(params IEnumerable<IssueAssignmentDetails>[] details)
        {
            var concat = Enumerable.Empty<IssueAssignmentDetails>();

            foreach (var list in details)
            {
                concat = concat.Concat(list);
            }

            return concat.ToDictionary(x => x.IssueAssignmentId);
        }

        private List<IssueProductAssignmentDetails> GetAssignmentsIdsToProductDetails(List<IssueAssignment> productAssignments)
        {
            var canManageProducts = _permissionService.Authorize(StandardPermissionProvider.ManageProducts);

            var productsIds = productAssignments.Select(x => x.ObjectId);
            var productsInfo = _productService.GetProductsByIds(productsIds.ToArray());
            var productIdsToProductDetails = productsInfo.ToDictionary(x => x.Id);

            var details = productAssignments.Select(x =>
            {
                var productAssignmentDetails = new IssueProductAssignmentDetails
                {
                    IssueAssignmentId = x.Id,
                };

                if (productIdsToProductDetails.TryGetValue(x.ObjectId, out var product))
                {
                    productAssignmentDetails.Name = product.Name;

                    if (canManageProducts && _workContext.CurrentVendor != null && product.VendorId == _workContext.CurrentVendor.Id
                        || canManageProducts && _workContext.CurrentVendor == null)
                    {
                        productAssignmentDetails.CanViewEditPage = true;
                    }
                }

                return productAssignmentDetails;
            }).ToList();

            return details;
        }

        private List<IssueVendorAssignmentDetails> GetAssignmentsIdsToVendorDetails(List<IssueAssignment> vendorAssignments)
        {
            var canManageVendors = _permissionService.Authorize(StandardPermissionProvider.ManageVendors);

            var vendorsIds = vendorAssignments.Select(x => x.ObjectId);
            var vendorsInfo = _vendorService.GetVendorsByIds(vendorsIds.ToArray());
            var vendorsIdsToProductDetails = vendorsInfo.ToDictionary(x => x.Id);

            var details = vendorAssignments.Select(x =>
            {
                var vendorAssignmentDetails = new IssueVendorAssignmentDetails
                {
                    IssueAssignmentId = x.Id,
                };

                if (vendorsIdsToProductDetails.TryGetValue(x.ObjectId, out var vendor))
                {
                    vendorAssignmentDetails.Name = vendor.Name;
                    vendorAssignmentDetails.Email = vendor.Email;
                    
                    if (canManageVendors)
                    {
                        vendorAssignmentDetails.CanViewEditPage = true;
                    }
                }

                return vendorAssignmentDetails;
            }).ToList();

            return details;
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
            var isVendorCustomer = _workContext.CurrentVendor != null;

            var issueStatuses = searchModel.SearchIssueStatus.Select(x => (IssueStatus)x).ToList();
            var issueProrities = searchModel.SearchIssuePriority.Select(x => (IssuePriority)x).ToList();

            var issueList = _issueService.GetIssueList(searchModel.SearchIssueName, issueProrities, issueStatuses,
                searchModel.SearchDeadlineFrom, searchModel.SearchDeadlineTo, searchModel.Page - 1, searchModel.PageSize);

            var createdByIds = issueList.Select(x => x.CreatedBy).Distinct();
            var customers = _customerService.GetCustomersByIds(createdByIds.ToArray()).ToDictionary(x => x.Id);

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
                    item.Deadline = item.Deadline.HasValue ? _dateTimeHelper.ConvertToUserTime(item.Deadline.Value, DateTimeKind.Utc) : (DateTime?)null;
                    item.CreatedAt = _dateTimeHelper.ConvertToUserTime(x.CreatedAt, DateTimeKind.Utc);
                    item.LastModified = _dateTimeHelper.ConvertToUserTime(x.LastModified, DateTimeKind.Utc);

                    if (customers.TryGetValue(x.CreatedBy, out var customer))
                    {
                        item.CreatedBy = $"{customerFullName} ({customer.Email})";
                    }
                    else
                    {
                        item.CreatedBy = customerFullName;
                    }

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
            var existingAssignmentsOfTypeObjectIds = existingAssignmentsOfType.Select(x => x.ObjectId).ToList();

            var assignments = _issueService.QuickSearchAssignments(text, issueAssignmentType, existingAssignmentsOfTypeObjectIds);

            var list = assignments.Select(x => new SelectListItem
            {
                Value = x.ObjectId.HasValue ? x.ObjectId.ToString() : string.Empty,
                Text = x.Name,
            }).ToList();

            return list;
        }

        private IssueHistoryPanelModel PrepareIssueHistoryPanelModel(EditIssueModel model)
        {
            var panelModel = new IssueHistoryPanelModel();
            panelModel.HistorySearchModel = PrepareIssueHistorySearchModel(model);
            return panelModel;
        }

        private IssueHistorySearchModel PrepareIssueHistorySearchModel(EditIssueModel model)
        {
            var searchModel = new IssueHistorySearchModel { IssueId = model.Id };
            return searchModel;
        }

        public IssueHistoryListModel PrepareIssueHistoryListModel(IssueHistorySearchModel searchModel)
        {
            var changes = _issueService.GetHistoryList(searchModel.IssueId, searchModel.Page - 1, searchModel.PageSize);
            var changeDetails = GetHistoryIdsToDetails(changes.Select(x => x).ToList());

            var createdByIds = changes.Select(x => x.ModifiedBy).Distinct();
            var customers = _customerService.GetCustomersByIds(createdByIds.ToArray()).ToDictionary(x => x.Id);

            var model = new IssueHistoryListModel().PrepareToGrid(searchModel, changes, () =>
            {
                var items = changes.Select(x =>
                {
                    var item = x.ToModel<IssueHistoryGridItem>();
                    item.ModifiedAt = _dateTimeHelper.ConvertToUserTime(x.ModifiedAt, DateTimeKind.Utc);

                    var customerFullName = _customerService.GetCustomerFullName(new Customer { Id = x.ModifiedBy });

                    if (customers.TryGetValue(x.ModifiedBy, out var customer))
                    {
                        item.ModifiedByFullName = $"{customerFullName} ({customer.Email})";
                    }
                    else
                    {
                        item.ModifiedByFullName = customerFullName;
                    }

                    if (changeDetails.TryGetValue(item.Id, out var details))
                    {
                        item.ChangeDetails = details;
                    }

                    return item;
                });

                return items;
            });

            return model;
        }

        private Dictionary<int, IssueHistoryChangeDetails> GetHistoryIdsToDetails(List<IssueHistory> changes)
        {
            var changeTypesToIssueHistory = changes.ToLookup(x => x.ChangeType);

            var assignmentChanges = changeTypesToIssueHistory[IssueChangeType.Assignment].ToList();
            var personsInvolvedChanges = changeTypesToIssueHistory[IssueChangeType.PersonInvolved].ToList();

            var changeIdsToAssignmentsChanges = GetAssignmentsChangeDetails(assignmentChanges).Cast<IssueHistoryChangeDetails>();
            var changeIdsToPersonsInvolvedChanges = GetPersonsInvolvedChangeDetails(personsInvolvedChanges).Cast<IssueHistoryChangeDetails>();
            var changeIdsToBasicInfoChanges = GetBasicInfoChangesDetails(changeTypesToIssueHistory);

            var changeIdsToDetails = MergeHistoryChangeDetailsToDictionary(changeIdsToAssignmentsChanges,
                changeIdsToPersonsInvolvedChanges,
                changeIdsToBasicInfoChanges);
            return changeIdsToDetails;
        }

        private Dictionary<int, IssueHistoryChangeDetails> MergeHistoryChangeDetailsToDictionary(params IEnumerable<IssueHistoryChangeDetails>[] details)
        {
            var concat = Enumerable.Empty<IssueHistoryChangeDetails>();

            foreach (var list in details)
            {
                concat = concat.Concat(list);
            }

            return concat.ToDictionary(x => x.IssueHistoryId);
        }

        private List<IssueHistoryAssignmentDetails> GetAssignmentsChangeDetails(List<IssueHistory> assignmentsChanges)
        {
            var issueHistoryAssignmentDetails = new List<IssueHistoryAssignmentDetails>();

            var issueHistoryIdsToAssignmentChangeValues = GetIssueHistoryEntriesToAssignmentValues(assignmentsChanges);
            var issueHistoryIdsToAssignmentChangeValuesGroupedByAssignmentType = issueHistoryIdsToAssignmentChangeValues
                .ToLookup(x => x.Value.AssignmentType, x => x.Key);

            var productChanges = issueHistoryIdsToAssignmentChangeValuesGroupedByAssignmentType[IssueAssignmentType.Product].ToList();
            var vendorChanges = issueHistoryIdsToAssignmentChangeValuesGroupedByAssignmentType[IssueAssignmentType.Vendor].ToList();

            var productAssignmentChangeDetails = GetProductAssignmentChangeDetails(productChanges, issueHistoryIdsToAssignmentChangeValues)
                .Cast<IssueHistoryAssignmentDetails>().ToList();
            var vendorAssignmentChangeDetails = GetVendorAssignmentChangeDetails(vendorChanges, issueHistoryIdsToAssignmentChangeValues)
                .Cast<IssueHistoryAssignmentDetails>().ToList();

            issueHistoryAssignmentDetails.AddRange(productAssignmentChangeDetails);
            issueHistoryAssignmentDetails.AddRange(vendorAssignmentChangeDetails);
            return issueHistoryAssignmentDetails;
        }

        private Dictionary<IssueHistory, IssueHistoryAssignmentValue> GetIssueHistoryEntriesToAssignmentValues(List<IssueHistory> assignmentsChanges)
        {
            var issueHistoryIdToAssignmentChangeValue = new Dictionary<IssueHistory, IssueHistoryAssignmentValue>();

            foreach (var change in assignmentsChanges)
            {
                var value = change.NewValue ?? change.OldValue;
                var deserializedValue = JsonConvert.DeserializeObject<IssueHistoryAssignmentValue>(value);
                issueHistoryIdToAssignmentChangeValue.Add(change, deserializedValue);
            }

            return issueHistoryIdToAssignmentChangeValue;
        }

        private List<IssueHistoryProductAssignmentDetails> GetProductAssignmentChangeDetails(List<IssueHistory> productChanges,
            Dictionary<IssueHistory, IssueHistoryAssignmentValue> issueHistoryIdToProductChangeValue)
        {
            var productsIds = issueHistoryIdToProductChangeValue.Select(x => x.Value.ObjectId).Distinct();
            var productsIdsToProducts = _productService.GetProductsByIds(productsIds.ToArray()).ToDictionary(x => x.Id);
            var result = new List<IssueHistoryProductAssignmentDetails>();
            foreach (var change in productChanges)
            {
                var details = new IssueHistoryProductAssignmentDetails
                {
                    IssueHistoryId = change.Id,
                    NewValue = change.NewValue,
                    OldValue = change.OldValue,
                    AssignmentType = IssueAssignmentType.Product,
                };

                var value = issueHistoryIdToProductChangeValue[change];
                if (productsIdsToProducts.TryGetValue(value.ObjectId, out var product))
                {
                    details.Name = product.Name;
                }
                else
                {
                    details.Name = value.Name;
                }

                result.Add(details);
            }

            return result;
        }

        private List<IssueHistoryVendorAssignmentDetails> GetVendorAssignmentChangeDetails(List<IssueHistory> vendorChanges,
            Dictionary<IssueHistory, IssueHistoryAssignmentValue> issueHistoryIdToVendorChangeValue)
        {
            var vendorsIds = issueHistoryIdToVendorChangeValue.Select(x => x.Value.ObjectId).Distinct();
            var vendorIdsToVendors = _vendorService.GetVendorsByIds(vendorsIds.ToArray()).ToDictionary(x => x.Id);
            var result = new List<IssueHistoryVendorAssignmentDetails>();
            foreach (var change in vendorChanges)
            {
                var details = new IssueHistoryVendorAssignmentDetails
                {
                    IssueHistoryId = change.Id,
                    NewValue = change.NewValue,
                    OldValue = change.OldValue,
                    AssignmentType = IssueAssignmentType.Vendor,
                };

                var value = issueHistoryIdToVendorChangeValue[change];
                if (vendorIdsToVendors.TryGetValue(value.ObjectId, out var vendor))
                {
                    details.Name = vendor.Name;
                    details.Email = vendor.Email;
                }
                else
                {
                    details.Name = value.Name;
                }

                result.Add(details);
            }

            return result;
        }

        private List<IssueHistoryPersonInvolvedDetails> GetPersonsInvolvedChangeDetails(List<IssueHistory> personsInvolvedChanges)
        {
            var details = personsInvolvedChanges.Select(x =>
            {
                var details = new IssueHistoryPersonInvolvedDetails
                {
                    IssueHistoryId = x.Id,
                    NewValue = x.NewValue,
                    OldValue = x.OldValue,
                };

                int customerId = x.NewValue != null ? int.Parse(x.NewValue) : int.Parse(x.OldValue);

                details.CustomerId = customerId;

                return details;
            }).ToList();

            var customersIds = details.Select(x => x.CustomerId).Distinct();
            var customers = _customerService.GetCustomersByIds(customersIds.ToArray());
            var customerIdToCustomer = customers.ToDictionary(x => x.Id);

            foreach (var item in details)
            {
                if (customerIdToCustomer.TryGetValue(item.CustomerId, out var customer))
                {
                    item.Email = customer.Email;
                    item.FullName = _customerService.GetCustomerFullName(new Customer { Id = item.CustomerId });
                }
            }

            return details.ToList();
        }

        private List<IssueHistoryChangeDetails> GetBasicInfoChangesDetails(ILookup<IssueChangeType, IssueHistory> changeTypesToIssueHistory)
        {
            var simpleChangesList = new List<IssueChangeType>
            {
                IssueChangeType.Deadline,
                IssueChangeType.Description,
                IssueChangeType.Name,
                IssueChangeType.Priority,
                IssueChangeType.Status,
            };

            var simpleChanges = changeTypesToIssueHistory.Where(x => simpleChangesList.Contains(x.Key)).SelectMany(x => x);
            var details = new List<IssueHistoryChangeDetails>();
            foreach (var change in simpleChanges)
            {
                if (change.ChangeType == IssueChangeType.Deadline)
                {
                    details.Add(new IssueHistoryChangeDetails
                    {
                        IssueHistoryId = change.Id,
                        NewValue = ParseAndConvertDateTimeHistoryValue(change.NewValue),
                        OldValue = ParseAndConvertDateTimeHistoryValue(change.OldValue),
                    });
                }
                else
                {
                    details.Add(new IssueHistoryChangeDetails
                    {
                        IssueHistoryId = change.Id,
                        NewValue = change.NewValue,
                        OldValue = change.OldValue,
                    });
                }
            }

            return details;
        }

        private string ParseAndConvertDateTimeHistoryValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            DateTime.TryParse(value, out var parsedValue);

            if (parsedValue == null)
            {
                return string.Empty;
            }

            var convertedValue = _dateTimeHelper.ConvertToUserTime(parsedValue, DateTimeKind.Utc).ToString();
            return convertedValue;
        }

        private IssueCommentsPanelModel PrepareIssueCommentsPanelModel(EditIssueModel model)
        {
            var panelModel = new IssueCommentsPanelModel();
            panelModel.CommentsSearchModel = PrepareIssueCommentsSearchModel(model);
            panelModel.CanEdit = model.CanEdit;
            return panelModel;
        }

        private IssueCommentsSearchModel PrepareIssueCommentsSearchModel(EditIssueModel model)
        {
            var searchModel = new IssueCommentsSearchModel { IssueId = model.Id };
            return searchModel;
        }

        public IssueCommentListModel PrepareIssueCommentListModel(IssueCommentsSearchModel searchModel)
        {
            var changes = _issueService.GetCommentList(searchModel.IssueId, searchModel.Page - 1, searchModel.PageSize);
            var currentUserId = _workContext.CurrentCustomer.Id;

            var createdByIds = changes.Select(x => x.CreatedBy).Distinct();
            var customers = _customerService.GetCustomersByIds(createdByIds.ToArray()).ToDictionary(x => x.Id);
            var isAdmin = _customerService.IsInCustomerRole(_workContext.CurrentCustomer, NopCustomerDefaults.AdministratorsRoleName);

            var model = new IssueCommentListModel().PrepareToGrid(searchModel, changes, () =>
            {
                var items = changes.Select(x =>
                {
                    var item = x.ToModel<IssueCommentsGridItem>();
                    item.CreatedAt = _dateTimeHelper.ConvertToUserTime(x.CreatedAt, DateTimeKind.Utc);

                    var customerFullName = _customerService.GetCustomerFullName(new Customer { Id = x.CreatedBy });

                    if (customers.TryGetValue(x.CreatedBy, out var customer))
                    {
                        item.CreatedByFullName = $"{customerFullName} ({customer.Email})";
                    }
                    else
                    {
                        item.CreatedByFullName = customerFullName;
                    }

                    if (isAdmin || x.CreatedBy == currentUserId)
                    {
                        item.CanDelete = true;
                    }

                    return item;
                });

                return items;
            });

            return model;
        }
    }
}
