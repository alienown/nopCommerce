using System;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.TaskManagement
{
    public class TaskManagementPlugin : BasePlugin, IMiscPlugin
    {
        private readonly string _configurationPageUrl = "Admin/Task/Configure";

        public TaskManagementPlugin()
        {

        }

        public override string GetConfigurationPageUrl()
        {
            return _configurationPageUrl;
        }
    }
}
