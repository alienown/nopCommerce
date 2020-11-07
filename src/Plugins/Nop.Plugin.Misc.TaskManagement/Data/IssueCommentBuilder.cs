using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class IssueCommentBuilder : NopEntityBuilder<IssueComment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(IssueComment.IssueId)).AsInt32().ForeignKey<Issue>(onDelete: Rule.None)
                .WithColumn(nameof(IssueComment.Content)).AsString(256)
                .WithColumn(nameof(IssueComment.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssueComment.CreatedAt)).AsDateTime2();
        }
    }
}