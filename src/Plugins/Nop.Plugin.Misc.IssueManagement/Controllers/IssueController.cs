using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Factories;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.IssueManagement.Controllers
{
    [Area(AreaNames.Admin)]
    public class IssueController : BasePluginController
    {
        private readonly IIssueService _issueService;
        private readonly IIssueModelFactory _issueModelFactory;

        public IssueController(IIssueService issueService, IIssueModelFactory issueModelFactory)
        {
            _issueService = issueService;
            _issueModelFactory = issueModelFactory;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            return View("~/Plugins/Misc.IssueManagement/Views/Configure.cshtml", model);
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            return View("~/Plugins/Misc.IssueManagement/Views/Index.cshtml");
        }

        [HttpGet]
        public IActionResult List()
        {
            var model = _issueModelFactory.PrepareIssueSearchModel();
            return View("~/Plugins/Misc.IssueManagement/Views/List/List.cshtml", model);
        }

        [HttpPost]
        public IActionResult IssueList(IssueSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssueListModel(searchModel);
            return Json(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = _issueModelFactory.PrepareAddIssueModel(new AddIssueModel());
            return View("~/Plugins/Misc.IssueManagement/Views/Add/Add.cshtml", model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Add(AddIssueModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var issue = model.ToEntity<Issue>();
                _issueService.InsertIssue(issue);

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = issue.Id });
                }

                return RedirectToAction("List");
            }

            model = _issueModelFactory.PrepareAddIssueModel(model);
            return View("~/Plugins/Misc.IssueManagement/Views/Add/Add.cshtml", model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var issue = _issueService.GetIssue(id);
            if (issue == null || issue.Deleted)
                return RedirectToAction("List");

            var model = _issueModelFactory.PrepareEditIssueModel(null, issue);
            return View("~/Plugins/Misc.IssueManagement/Views/Edit/Edit.cshtml", model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Edit(EditIssueModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var issue = model.ToEntity<Issue>();
                _issueService.UpdateIssue(issue);

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = issue.Id });
                }

                return RedirectToAction("List");
            }

            model = _issueModelFactory.PrepareEditIssueModel(model, null);
            return View("~/Plugins/Misc.IssueManagement/Views/Edit/Edit.cshtml", model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _issueService.DeleteIssue(id);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult IssuePersonInvolvedList(IssuePersonInvolvedSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssuePersonInvolvedListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public IActionResult IssueAssignmentList(IssueAssignmentSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssueAssignmentListModel(searchModel);
            return Json(model);
        }
    }
}