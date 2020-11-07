using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.IssueManagement.Controllers
{
    [Area(AreaNames.Admin)]
    public class IssueController : BasePluginController
    {
        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            return View("~/Plugins/Misc.IssueManagement/Views/Configure.cshtml", model);
        }

        public IActionResult List()
        {
            return View("~/Plugins/Misc.IssueManagement/Views/List.cshtml");
        }

        public IActionResult New()
        {
            return View("~/Plugins/Misc.IssueManagement/Views/New.cshtml");
        }
    }
}