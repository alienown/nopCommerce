using System;
using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Manual.Validators
{
    public partial class EditIssueModelValidator : BaseNopValidator<EditIssueModel>
    {
        public EditIssueModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Issue.Edit.BasicInfoPanel.NameNotEmptyValidationMessage"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Issue.Edit.BasicInfoPanel.DescriptionNotEmptyValidationMessage"));
            RuleFor(x => x.Priority).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Issue.Edit.BasicInfoPanel.PriorityNotEmptyValidationMessage"));
            RuleFor(x => x.Status).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Issue.Edit.BasicInfoPanel.StatusNotEmptyValidationMessage"));

            SetDatabaseValidationRules<Issue>(dataProvider);
        }
    }
}