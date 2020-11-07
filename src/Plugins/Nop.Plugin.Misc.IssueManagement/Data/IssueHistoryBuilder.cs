using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Data
{
    public class IssueHistoryBuilder : NopEntityBuilder<IssueHistory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(IssueHistory.IssueId)).AsInt32().ForeignKey<Issue>(onDelete: Rule.None)
                .WithColumn(nameof(IssueHistory.ChangeType)).AsByte()
                .WithColumn(nameof(IssueHistory.NewValue)).AsString().Nullable()
                .WithColumn(nameof(IssueHistory.OldValue)).AsString().Nullable()
                .WithColumn(nameof(IssueHistory.ModifiedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssueHistory.ModifiedAt)).AsDateTime2();
        }
    }
}