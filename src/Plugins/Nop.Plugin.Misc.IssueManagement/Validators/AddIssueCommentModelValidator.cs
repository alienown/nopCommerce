using FluentValidation;
using Nop.Data;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Manual.Validators
{
    public partial class AddIssueCommentModelValidator : BaseNopValidator<AddIssueCommentModel>
    {
        public AddIssueCommentModelValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.IssueManagement.Edit.CommentsPanel.AddCommentsSubPanel.CommentRequiredValidationMessage"));

            SetDatabaseValidationRules<IssueComment>(dataProvider);
        }
    }
}