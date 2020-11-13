using FluentValidation;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Manual.Validators
{
    public partial class AddIssueModelValidator : BaseNopValidator<AddIssueModel>
    {
        public AddIssueModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.NameNotEmptyValidationMessage"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.DescriptionNotEmptyValidationMessage"));
            RuleFor(x => x.Priority).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.PriorityNotEmptyValidationMessage"));
            RuleFor(x => x.Status).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Add.BasicInfoPanel.StatusNotEmptyValidationMessage"));

            SetDatabaseValidationRules<Issue>(dataProvider);
        }
    }
}