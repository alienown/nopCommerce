using FluentValidation;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Manual.Validators
{
    public partial class EditBasicInfoModelValidator : BaseNopValidator<EditBasicInfoPanelModel>
    {
        public EditBasicInfoModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.NameNotEmptyValidationMessage"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.DescriptionNotEmptyValidationMessage"));
            RuleFor(x => x.Priority).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.PriorityNotEmptyValidationMessage"));
            RuleFor(x => x.Status).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.BasicInfoPanel.StatusNotEmptyValidationMessage"));

            SetDatabaseValidationRules<Issue>(dataProvider);
        }
    }
}