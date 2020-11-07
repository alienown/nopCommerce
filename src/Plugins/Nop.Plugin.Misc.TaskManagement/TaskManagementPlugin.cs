using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.TaskManagement
{
    public class TaskManagementPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public TaskManagementPlugin(IWebHelper webHelper, ILocalizationService localizationService)
        {
            this._webHelper = webHelper;
            this._localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Task/Configure";
        }

        public override void Install()
        {
            _localizationService.AddLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.TaskManagement.Task"] = "Task",
            });

            base.Install();
        }

        public override void Uninstall()
        {
            _localizationService.DeleteLocaleResources("Plugins.Misc.TaskManagement");

            base.Uninstall();
        }
    }
}
