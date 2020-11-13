using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Services.Catalog;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.IssueManagement.Services
{
    public class IssueService : IIssueService
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        private readonly IRepository<Issue> _issueRepository;
        private readonly IRepository<IssueHistory> _issueHistoryRepository;
        private readonly IRepository<IssuePersonInvolved> _issuePersonsInvolvedRepository;
        private readonly IRepository<IssueAssignment> _issueAssignmentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        private readonly byte _quickSearchTextFilterMinLength = 3;
        private readonly byte _quickSearchTake = 50;

        public IssueService(IWorkContext workContext, ICustomerService customerService, IRepository<Issue> issueRepository,
            IRepository<IssueHistory> issueHistoryRepository, IRepository<IssuePersonInvolved> issuePersonsInvolvedRepository,
            IRepository<IssueAssignment> issueAssignmentRepository, IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository, IProductService productService)
        {
            _workContext = workContext;
            _customerService = customerService;
            _issueRepository = issueRepository;
            _issueHistoryRepository = issueHistoryRepository;
            _issuePersonsInvolvedRepository = issuePersonsInvolvedRepository;
            _issueAssignmentRepository = issueAssignmentRepository;
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _productService = productService;
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

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return list;
        }

        public void InsertIssue(Issue issue)
        {
            var now = DateTime.UtcNow;
            issue.CreatedBy = _workContext.CurrentCustomer.Id;
            issue.CreatedAt = now;
            issue.LastModified = now;
            _issueRepository.Insert(issue);
        }

        public void UpdateIssue(Issue issue)
        {
            var originalIssue = _issueRepository.GetById(issue.Id);
            if (originalIssue != null)
            {
                issue.CreatedAt = originalIssue.CreatedAt;
                issue.CreatedBy = originalIssue.CreatedBy;
                issue.LastModified = DateTime.UtcNow;
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
                var query = (from customer in _customerRepository.Table
                             from gaFirstName in _genericAttributeRepository.Table.Where(x => x.EntityId == customer.Id &&
                                  x.Key == NopCustomerDefaults.FirstNameAttribute && x.KeyGroup == nameof(Customer))
                             from gaLastName in _genericAttributeRepository.Table.Where(x => x.EntityId == customer.Id &&
                                  x.Key == NopCustomerDefaults.LastNameAttribute && x.KeyGroup == nameof(Customer))
                             where gaFirstName.Value.Contains(text) || gaLastName.Value.Contains(text) || customer.Email.Contains(text)
                             select new
                             {
                                 CustomerId = customer.Id,
                                 customer.Email,
                                 FirstName = gaFirstName.Value,
                                 LastName = gaLastName.Value,
                             });

                if (idsToExclude != null && idsToExclude.Any())
                {
                    query = query.Where(x => !idsToExclude.Contains(x.CustomerId));
                }

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

                var historyEntry = CreateIssueHistoryEntry(issueAssignment.IssueId, issueAssignment.ObjectId.ToString(), null,
                    IssueChangeType.Assignment, now, _workContext.CurrentCustomer.Id);
                _issueHistoryRepository.Insert(historyEntry);
            }
        }

        public void DeleteAssignment(int id)
        {
            var issueAssignment = _issueAssignmentRepository.GetById(id);
            if (issueAssignment != null)
            {
                _issueAssignmentRepository.Delete(issueAssignment);
                var historyEntry = CreateIssueHistoryEntry(issueAssignment.IssueId, null, issueAssignment.ObjectId.ToString(),
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
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Priority.ToString(), originalIssue.Priority.ToString(),
                    IssueChangeType.Priority, now, customerId));
            }

            if (modifiedIssue.Status != originalIssue.Status)
            {
                result.Add(CreateIssueHistoryEntry(modifiedIssue.Id, modifiedIssue.Status.ToString(), originalIssue.Status.ToString(),
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
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private List<QuickSearchAssignmentInfo> QuickSearchProductAssignments(string text, List<int> idsToExclude = null)
        {
            var products = _productService.SearchProducts(pageSize: _quickSearchTake, keywords: text, searchSku: true);

            var productsIds = products.Select(x => x.Id).Except(idsToExclude).ToList();
            var filteredProducts = products.Where(x => productsIds.Contains(x.Id)).ToList();

            var list = filteredProducts.Select(x => new QuickSearchAssignmentInfo
            {
                ObjectId = x.Id,
                Name = x.Name,
            }).ToList();

            return list;
        }
    }
}
