using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Services
{
    public interface IIssueService
    {
        Issue GetIssue(int id);
    }
}
