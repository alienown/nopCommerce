using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.TaskManagement.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.SendinBlue.Controllers
{
    public class TaskController : BasePluginController
    {
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            return View("~/Plugins/Misc.TaskManagement/Views/Configure.cshtml", model);
        }
    }
}