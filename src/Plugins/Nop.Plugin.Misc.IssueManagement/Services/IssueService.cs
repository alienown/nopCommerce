using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Services.Vendors;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public class IssueService : IIssueService
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<Issue> _issueRepository;
        private readonly IRepository<IssueHistory> _issueHistoryRepository;
        private readonly IRepository<IssuePersonInvolved> _issuePersonsInvolvedRepository;
        private readonly IRepository<IssueAssignment> _issueAssignmentRepository;
        private readonly IRepository<IssueComment> _issueCommentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly byte _quickSearchTextFilterMinLength = 3;
        private readonly byte _quickSearchTake = 50;

        public IssueService(IWorkContext workContext, ICustomerService customerService, IRepository<Issue> issueRepository,
            IRepository<IssueHistory> issueHistoryRepository, IRepository<IssuePersonInvolved> issuePersonsInvolvedRepository,
            IRepository<IssueAssignment> issueAssignmentRepository, IRepository<IssueComment> issueCommentRepository,
            IRepository<Customer> customerRepository, IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<Vendor> vendorRepository, IProductService productService, IVendorService vendorService,
            IPermissionService permissionService, IDateTimeHelper dateTimeHelper)
        {
            _workContext = workContext;
            _customerService = customerService;
            _issueRepository = issueRepository;
            _issueHistoryRepository = issueHistoryRepository;
            _issuePersonsInvolvedRepository = issuePersonsInvolvedRepository;
            _issueAssignmentRepository = issueAssignmentRepository;
            _issueCommentRepository = issueCommentRepository;
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _vendorRepository = vendorRepository;
            _productService = productService;
            _vendorService = vendorService;
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
        }

        public Issue GetIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            return issue;
        }

        public IPagedList<Issue> GetIssueList(string name, List<IssuePriority> priorities, List<IssueStatus> statuses, DateTime? deadlineFrom, DateTime? deadlineTo,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueRepository.GetAllPaged(query =>
            {
                query = query.Where(x => !x.Deleted);
                
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (priorities != null && priorities.Any())
                {
                    query = query.Where(x => priorities.Contains(x.Priority));
                }

                if (statuses != null && statuses.Any())
                {
                    query = query.Where(x => statuses.Contains(x.Status));
                }

                if (deadlineFrom.HasValue)
                {
                    query = query.Where(x => x.Deadline >= deadlineFrom.Value);
                }

                if (deadlineTo.HasValue)
                {
                    query = query.Where(x => x.Deadline <= deadlineTo.Value);
                }

                if (_workContext.CurrentVendor != null)
                {
                    query = RestrictIssuesForVendorCustomer(query);
                }

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return list;
        }

        /// <summary>
        /// Used for limiting query to issues that vendor customer (vendor manager) can see.
        /// Vendor customers can access issues if they are creators or they are in persons involved or if there is vendor assignment that customer is assigned to.
        /// </summary>
        /// <param name="query">Query to limit</param>
        /// <returns>Query limited to issues that vendor customer can see</returns>
        private IQueryable<Issue> RestrictIssuesForVendorCustomer(IQueryable<Issue> query)
        {
            var customerId = _workContext.CurrentCustomer.Id;
            var vendorId = _workContext.CurrentVendor.Id;

            var inPersonsInvolvedQuery = from personInvolved in _issuePersonsInvolvedRepository.Table
                                         where personInvolved.CustomerId == customerId
                                         select personInvolved.IssueId;

            var issuesIdsWithVendorOfCustomerQuery = from assignment in _issueAssignmentRepository.Table
                                                     where assignment.ObjectId == vendorId && assignment.AssignmentType == IssueAssignmentType.Vendor
                                                     select assignment.IssueId;

            var issuesCreatedByCustomer = from issue in _issueRepository.Table
                                          where issue.CreatedBy == customerId
                                          select issue.Id;

            var issuesIds = inPersonsInvolvedQuery.Union(issuesIdsWithVendorOfCustomerQuery).Union(issuesCreatedByCustomer);

            query = from issue in query
                    from issueId in issuesIds.Where(x => x == issue.Id)
                    select issue;

            return query;
        }

        public void InsertIssue(Issue issue)
        {
            var now = DateTime.UtcNow;
            var currentUserId = _workContext.CurrentCustomer.Id;
            issue.CreatedBy = currentUserId;
            issue.CreatedAt = now;
            issue.LastModified = now;
            if (issue.Deadline.HasValue)
            {
                issue.Deadline = _dateTimeHelper.ConvertToUtcTime(issue.Deadline.Value, DateTimeKind.Local);
            }

            _issueRepository.Insert(issue);

            var personInvolved = new IssuePersonInvolved
            {
                IssueId = issue.Id,
                CustomerId = currentUserId,
                CreatedBy = currentUserId,
                CreatedAt = now,
            };
            InsertPersonInvolved(personInvolved);

            if (_workContext.CurrentVendor != null)
            {
                var vendorAssignment = new IssueAssignment
                {
                    IssueId = issue.Id,
                    ObjectId = _workContext.CurrentVendor.Id,
                    AssignmentType = IssueAssignmentType.Vendor,
                    CreatedBy = currentUserId,
                    CreatedAt = now,
                };
                InsertAssignment(vendorAssignment);
            }
        }

        public void UpdateIssue(Issue issue)
        {
            var originalIssue = _issueRepository.GetById(issue.Id);
            if (originalIssue != null)
            {
                issue.CreatedAt = originalIssue.CreatedAt;
                issue.CreatedBy = originalIssue.CreatedBy;
                issue.LastModified = DateTime.UtcNow;
                if (issue.Deadline.HasValue)
                {
                    issue.Deadline = _dateTimeHelper.ConvertToUtcTime(issue.Deadline.Value, DateTimeKind.Local);
                }
                var changes = GenerateIssueHistoryEntiresForIssueEntity(issue);
                _issueRepository.Update(issue);
                _issueHistoryRepository.Insert(changes);
            }
        }

        public void DeleteIssue(int id)
        {
            var issue = _issueRepository.GetById(id);
            if (issue != null)
            {
                _issueRepository.Delete(issue);
            }
        }

        public IPagedList<IssuePersonInvolved> GetPersonInvolvedList(int issueId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issuePersonsInvolvedRepository.GetAllPaged(query =>
            {
                query = query.Where(x => x.IssueId == issueId);
                return query;
            }, pageIndex: pageIndex, pageSize: pageSize, getOnlyTotalCount: getOnlyTotalCount);

            return list;
        }

        public List<QuickSearchCustomerInfo> QuickSearchCustomers(string text, List<int> idsToExclude = null)
        {
            if (!string.IsNullOrWhiteSpace(text) && text.Length >= _quickSearchTextFilterMinLength)
            {
                var baseQuery = from customer in _customerRepository.Table
                            select customer;

                if (idsToExclude != null && idsToExclude.Any())
                {
                    baseQuery = baseQuery.Where(x => !idsToExclude.Contains(x.Id));
                }

                if (_workContext.CurrentVendor != null && !_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                {
                    baseQuery = baseQuery.Where(x => x.VendorId == _workContext.CurrentVendor.Id);
                }

                var query = (from customer in baseQuery
                             from gaFirstName in _genericAttributeRepository.Table.Where(x => x.EntityId == customer.Id &&
                                  x.Key == NopCustomerDefaults.FirstNameAttribute && x.KeyGroup == nameof(Customer))
                             from gaLastName in _genericAttributeRepository.Table.Where(x => x.EntityId == customer.Id &&
                                  x.Key == NopCustomerDefaults.LastNameAttribute && x.KeyGroup == nameof(Customer))
                             where gaFirstName.Value.Contains(text) || gaLastName.Value.Contains(text) || customer.Email.Contains(text) && !customer.Deleted
                                   && customer.Active
                             select new
                             {
                                 CustomerId = customer.Id,
                                 customer.Email,
                                 FirstName = gaFirstName.Value,
                                 LastName = gaLastName.Value,
                             });

                var queryResult = query.Take(_quickSearchTake).ToList();

                var list = new List<QuickSearchCustomerInfo>();
                foreach (var customer in queryResult)
                {
                    list.Add(new QuickSearchCustomerInfo
                    {
                        CustomerId = customer.CustomerId,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email,
                    });
                }

                return list;
            }
            else
            {
                return new List<QuickSearchCustomerInfo>();
            }
        }

        public void InsertPersonInvolved(IssuePersonInvolved issuePersonInvolved)
        {
            var doesIssueExist = _issueRepository.Table.Any(x => x.Id == issuePersonInvolved.IssueId);
            if (doesIssueExist)
            {
                var now = DateTime.UtcNow;
                issuePersonInvolved.CreatedBy = _workContext.CurrentCustomer.Id;
                issuePersonInvolved.CreatedAt = now;
                _issuePersonsInvolvedRepository.Insert(issuePersonInvolved);

                var historyEntry = CreateIssueHistoryEntry(issuePersonInvolved.IssueId, issuePersonInvolved.CustomerId.ToString(), null,
                    IssueChangeType.PersonInvolved, now, _workContext.CurrentCustomer.Id);
                _issueHistoryRepository.Insert(historyEntry);
            }
        }

        public void DeletePersonInvolved(int id)
        {
            var personInvolved = _issuePersonsInvolvedRepository.GetById(id);
            if (personInvolved != null)
            {
                _issuePersonsInvolvedRepository.Delete(personInvolved);
                var historyEntry = CreateIssueHistoryEntry(personInvolved.IssueId, null, personInvolved.CustomerId.ToString(),
                    IssueChangeType.PersonInvolved, DateTime.UtcNow, _workContext.CurrentCustomer.Id);
                _issueHistoryRepository.Insert(historyEntry);
            }
        }

        public IPagedList<IssueAssignment> GetAssignmentList(int issueId, IssueAssignmentType? assignmentType, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueAssignmentRepository.GetAllPaged(query =>
            {
                query = query.Where(x => x.IssueId == issueId);

                if (assignmentType.HasValue)
                {
                    query = query.Where(x => x.AssignmentType == assignmentType.Value);
                }

                return query;
            }, pageIndex: pageIndex, pageSize: pageSize, getOnlyTotalCount: getOnlyTotalCount);

            return list;
        }

        public void InsertAssignment(IssueAssignment issueAssignment)
        {
            var doesIssueExist = _issueRepository.Table.Any(x => x.Id == issueAssignment.IssueId);
            if (doesIssueExist)
            {
                var now = DateTime.UtcNow;
                issueAssignment.CreatedBy = _workContext.CurrentCustomer.Id;
                issueAssignment.CreatedAt = now;
                _issueAssignmentRepository.Insert(issueAssignment);

                var newValue = JsonConvert.SerializeObject(GetIssueHistoryAssignmentValue(issueAssignment));
                var historyEntry = CreateIssueHistoryEntry(issueAssignment.IssueId, newValue, null,
                    IssueChangeType.Assignment, now, _workContext.CurrentCustomer.Id);
                _issueHistoryRepository.Insert(historyEntry);
            }
        }

        private IssueHistoryAssignmentValue GetIssueHistoryAssignmentValue(IssueAssignment issueAssignment)
        {
            var value = new IssueHistoryAssignmentValue
            {
                ObjectId = issueAssignment.ObjectId,
                Name = GetAssignmentObjectName(issueAssignment.ObjectId, issueAssignment.AssignmentType),
                AssignmentType = issueAssignment.AssignmentType,
            };

            return value;
        }

        private string GetAssignmentObjectName(int objectId, IssueAssignmentType assignmentType)
        {
            var name = string.Empty;

            switch (assignmentType)
            {
                case IssueAssignmentType.Product:
                    name = GetProductName(objectId);
                    break;
                case IssueAssignmentType.Vendor:
                    name = GetVendorName(objectId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return name;
        }

        private string GetProductName(int productId)
        {
            var product = _productService.GetProductById(productId);
            return product.Name;
        }

        private string GetVendorName(int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            return $"{vendor.Name} ({vendor.Email})";
        }

        public void DeleteAssignment(int id)
        {
            var issueAssignment = _issueAssignmentRepository.GetById(id);
            if (issueAssignment != null)
            {
                _issueAssignmentRepository.Delete(issueAssignment);
                var oldValue = JsonConvert.SerializeObject(GetIssueHistoryAssignmentValue(issueAssignment));
                var historyEntry = CreateIssueHistoryEntry(issueAssignment.IssueId, null, oldValue,
                    IssueChangeType.Assignment, DateTime.UtcNow, _workContext.CurrentCustomer.Id);
                _issueHistoryRepository.Insert(historyEntry);
            }
        }

        private List<IssueHistory> GenerateIssueHistoryEntiresForIssueEntity(Issue modifiedIssue)
        {
            var result = new List<IssueHistory>();
            var now = modifiedIssue.LastModified;
            var customerId = _workContext.CurrentCustomer.Id;
            var originalIssue = _issueRepository.GetById(modifiedIssue.Id);

            if (modifiedIssue.Name != originalIssue.Name)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Name, originalIssue.Name,
                    IssueChangeType.Name, now, customerId));
            }

            if (modifiedIssue.Description != originalIssue.Description)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Description, originalIssue.Description,
                    IssueChangeType.Description, now, customerId));
            }

            if (modifiedIssue.Priority != originalIssue.Priority)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, ((byte)modifiedIssue.Priority).ToString(), ((byte)originalIssue.Priority).ToString(),
                    IssueChangeType.Priority, now, customerId));
            }

            if (modifiedIssue.Status != originalIssue.Status)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, ((byte)modifiedIssue.Status).ToString(), ((byte)originalIssue.Status).ToString(),
                    IssueChangeType.Status, now, customerId));
            }

            if (modifiedIssue.Deadline != originalIssue.Deadline)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Deadline.ToString(), originalIssue.Deadline.ToString(),
                    IssueChangeType.Deadline, now, customerId));
            }

            return result;
        }

        private IssueHistory CreateIssueHistoryEntry(int issueId, string newValue, string oldValue, IssueChangeType changeType, DateTime modifiedAt, int modifiedBy)
        {
            return new IssueHistory
            {
                IssueId = issueId,
                NewValue = newValue,
                OldValue = oldValue,
                ChangeType = changeType,
                ModifiedAt = modifiedAt,
                ModifiedBy = modifiedBy,
            };
        }

        public List<QuickSearchAssignmentInfo> QuickSearchAssignments(string text, IssueAssignmentType issueAssignmentType, List<int> idsToExclude = null)
        {
            List<QuickSearchAssignmentInfo> result;

            switch (issueAssignmentType)
            {
                case IssueAssignmentType.Product:
                    result = QuickSearchProductAssignments(text, idsToExclude);
                    break;
                case IssueAssignmentType.Vendor:
                    result = QuickSearchVendorAssignments(text, idsToExclude);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private List<QuickSearchAssignmentInfo> QuickSearchProductAssignments(string text, List<int> idsToExclude = null)
        {
            if (!string.IsNullOrWhiteSpace(text) && text.Length >= _quickSearchTextFilterMinLength)
            {
                var vendorId = _workContext.CurrentVendor?.Id ?? 0;
                var products = _productService.SearchProducts(pageSize: _quickSearchTake, keywords: text, searchSku: true, vendorId: vendorId);

                var productsIds = products.Select(x => x.Id).Except(idsToExclude).ToList();
                var filteredProducts = products.Where(x => productsIds.Contains(x.Id)).ToList();

                var list = filteredProducts.Select(x => new QuickSearchAssignmentInfo
                {
                    ObjectId = x.Id,
                    Name = x.Name,
                }).ToList();

                return list;
            }

            return new List<QuickSearchAssignmentInfo>();
        }

        private List<QuickSearchAssignmentInfo> QuickSearchVendorAssignments(string text, List<int> idsToExclude = null)
        {
            if (!string.IsNullOrWhiteSpace(text) && text.Length >= _quickSearchTextFilterMinLength)
            {
                var query = from vendor in _vendorRepository.Table
                            where vendor.Name.Contains(text) || vendor.Email.Contains(text)
                            select new
                            {
                                Id = vendor.Id,
                                Name = vendor.Name,
                                Email = vendor.Email,
                            };

                if (idsToExclude != null && idsToExclude.Any())
                {
                    query = query.Where(x => !idsToExclude.Contains(x.Id));
                }

                if (_workContext.CurrentVendor != null && !_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                {
                    var customerVendorId = _workContext.CurrentVendor.Id;
                    query = query.Where(x => x.Id == customerVendorId);
                }

                var vendors = query.ToList();

                var list = vendors.Select(x => new QuickSearchAssignmentInfo
                {
                    ObjectId = x.Id,
                    Name = x.Name,
                }).ToList();

                return list;
            }

            return new List<QuickSearchAssignmentInfo>();
        }

        public IPagedList<IssueHistory> GetHistoryList(int issueId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueHistoryRepository.GetAllPaged(query =>
            {
                query = query.Where(x => x.IssueId == issueId).OrderByDescending(x => x.ModifiedAt);
                return query;
            }, pageIndex: pageIndex, pageSize: pageSize, getOnlyTotalCount: getOnlyTotalCount);

            return list;
        }

        public List<IssueAssignment> GetAssignmentsByIds(List<int> assignmentsIds)
        {
            var result = _issueAssignmentRepository.GetByIds(assignmentsIds).ToList();
            return result;
        }

        public void InsertComment(IssueComment comment)
        {
            var now = DateTime.UtcNow;
            var currentUserId = _workContext.CurrentCustomer.Id;
            comment.CreatedAt = now;
            comment.CreatedBy = currentUserId;
            _issueCommentRepository.Insert(comment);
        }

        public void DeleteComment(int id)
        {
            var comment = _issueCommentRepository.GetById(id);
            if (comment != null)
            {
                _issueCommentRepository.Delete(comment);
            }
        }

        public IPagedList<IssueComment> GetCommentList(int issueId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var list = _issueCommentRepository.GetAllPaged(query =>
            {
                query = query.Where(x => x.IssueId == issueId).OrderByDescending(x => x.CreatedAt);
                return query;
            }, pageIndex: pageIndex, pageSize: pageSize, getOnlyTotalCount: getOnlyTotalCount);

            return list;
        }

        public bool CanEditIssue(int issueId)
        {
            var issue = _issueRepository.GetById(issueId);
            var currentCustomerId = _workContext.CurrentCustomer.Id;

            var isPersonInvolved = _issuePersonsInvolvedRepository.Table.Any(x => x.CustomerId == currentCustomerId && x.IssueId == issueId);
            var isCreator = issue.CreatedBy == currentCustomerId;
            var isAdmin = _customerService.IsInCustomerRole(_workContext.CurrentCustomer, NopCustomerDefaults.AdministratorsRoleName);

            return isAdmin || isPersonInvolved || isCreator;
        }

        public bool CanViewIssue(int issueId)
        {
            var isAdmin = _customerService.IsInCustomerRole(_workContext.CurrentCustomer, NopCustomerDefaults.AdministratorsRoleName);
            if (isAdmin)
            {
                return true;
            }

            var customerId = _workContext.CurrentCustomer.Id;

            var isPersonInvolved = _issuePersonsInvolvedRepository.Table.Any(x => x.IssueId == issueId && x.CustomerId == customerId);
            if (isPersonInvolved)
            {
                return true;
            }

            var isCreator = _issueRepository.GetById(issueId).CreatedBy == customerId;
            if (isCreator)
            {
                return true;
            }

            return false;
        }

        public Dictionary<IssueStatus, int> GetIssueCountPerStatus()
        {
            var query = _issueRepository.Table;

            if (_workContext.CurrentVendor != null)
            {
                query = RestrictIssuesForVendorCustomer(query);
            }

            var result = query.GroupBy(x => x.Status)
                .Select(x => new
                {
                    Status = x.Key,
                    IssuesCount = x.Count(),
                }).ToDictionary(x => x.Status, y => y.IssuesCount);

            foreach (IssueStatus status in Enum.GetValues(typeof(IssueStatus)))
            {
                if (!result.ContainsKey(status))
                {
                    result.Add(status, 0);
                }
            }

            result = result.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            return result;
        }

        public Dictionary<IssuePriority, int> GetIssueCountPerPriority()
        {
            var query = _issueRepository.Table;

            if (_workContext.CurrentVendor != null)
            {
                query = RestrictIssuesForVendorCustomer(query);
            }

            var result = query.GroupBy(x => x.Priority)
                .Select(x => new
                {
                    Status = x.Key,
                    IssuesCount = x.Count(),
                }).ToDictionary(x => x.Status, y => y.IssuesCount);

            foreach (IssuePriority status in Enum.GetValues(typeof(IssuePriority)))
            {
                if (!result.ContainsKey(status))
                {
                    result.Add(status, 0);
                }
            }

            result = result.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            return result;
        }
    }
}
