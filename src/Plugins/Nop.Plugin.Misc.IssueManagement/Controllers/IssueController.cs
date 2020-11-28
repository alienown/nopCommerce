using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Factories;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Plugin.Misc.IssueManagement.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
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
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public IssueController(IIssueService issueService, IIssueModelFactory issueModelFactory, ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _issueService = issueService;
            _issueModelFactory = issueModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            return View("~/Plugins/Misc.IssueManagement/Views/Configure.cshtml", model);
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
            var model = _issueModelFactory.PrepareAddIssueModel(null);
            return View("~/Plugins/Misc.IssueManagement/Views/Add/Add.cshtml", model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("saveContinueButton", "continueEditing")]
        public IActionResult Add(AddIssueModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var issue = model.ToEntity<Issue>();
                _issueService.InsertIssue(issue);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Misc.IssueManagement.Add.AddSuccess"));

                if (continueEditing)
                {
                    return RedirectToAction("Index", new { id = issue.Id });
                }

                return RedirectToAction("List");
            }

            model = _issueModelFactory.PrepareAddIssueModel(model);
            return View("~/Plugins/Misc.IssueManagement/Views/Add/Add.cshtml", model);
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var issue = _issueService.GetIssue(id);
            if (issue == null || issue.Deleted)
                return RedirectToAction("List");

            var model = _issueModelFactory.PrepareEditIssueModel(null, issue);
            return View("~/Plugins/Misc.IssueManagement/Views/Index/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveBasicInfo(EditBasicInfoPanelModel model)
        {
            if (ModelState.IsValid)
            {
                var issue = model.ToEntity<Issue>();
                _issueService.UpdateIssue(issue);
                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.UpdateSuccess"));
                return Json(new { Result = true, id = issue.Id });
            }

            model = _issueModelFactory.PrepareEditBasicInfoPanelModel(model, null);
            return View("~/Plugins/Misc.IssueManagement/Views/Index/_BasicInfo.cshtml", model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _issueService.DeleteIssue(id);
            _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Misc.IssueManagement.Delete.DeleteSuccess"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult IssuePersonInvolvedList(IssuePersonsInvolvedSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssuePersonInvolvedListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public IActionResult IssueAssignmentList(IssueAssignmentsSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssueAssignmentListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public IActionResult GetPersonsInvolvedForAddComboBox(string text, int issueId)
        {
            var list = _issueModelFactory.GetPersonsInvolvedForAddComboBox(text, issueId);
            return Json(list);
        }

        [HttpPost]
        public IActionResult AddPersonInvolved(int customerId, int issueId)
        {
            var personInvolved = new IssuePersonInvolved
            {
                CustomerId = customerId,
                IssueId = issueId,
            };

            _issueService.InsertPersonInvolved(personInvolved);
            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult DeletePersonInvolved(int id)
        {
            _issueService.DeletePersonInvolved(id);
            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult GetAssignmentsForAddComboBox(string text, IssueAssignmentType assignmentType, int issueId)
        {
            var list = _issueModelFactory.GetAssignmentsForAddComboBox(text, assignmentType, issueId);
            return Json(list);
        }

        [HttpPost]
        public IActionResult AddAssignment(int objectId, IssueAssignmentType assignmentType, int issueId)
        {
            var assignment = new IssueAssignment
            {
                ObjectId = objectId,
                IssueId = issueId,
                AssignmentType = assignmentType,
            };

            _issueService.InsertAssignment(assignment);
            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult DeleteAssignment(int id)
        {
            _issueService.DeleteAssignment(id);
            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult IssueHistoryList(IssueHistorySearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssueHistoryListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public IActionResult AddComment(AddIssueCommentModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = model.ToEntity<IssueComment>();
                _issueService.InsertComment(comment);
                return Json(new { Result = true });
            }

            return View("~/Plugins/Misc.IssueManagement/Views/Index/_AddComment.cshtml", model);
        }

        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            _issueService.DeleteComment(id);
            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult IssueCommentList(IssueCommentsSearchModel searchModel)
        {
            var model = _issueModelFactory.PrepareIssueCommentListModel(searchModel);
            return Json(model);
        }
    }
}