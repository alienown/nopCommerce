using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.IssueManagement.Models
{
    public class EditIssuePersonsInvolvedPanelModel
    {
        public IssuePersonsInvolvedSearchModel PersonsInvolvedSearchModel { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IssueManagement.Edit.PersonsInvolvedPanel.AddPersonsInvolvedSubPanel.Person")]
        public int? CustomerId { get; set; }

        public bool CanEdit { get; set; }
    }
}
