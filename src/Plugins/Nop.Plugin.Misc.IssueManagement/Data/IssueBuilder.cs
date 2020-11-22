using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Data
{
    public class IssueBuilder : NopEntityBuilder<Issue>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Issue.Name)).AsString(50)
                .WithColumn(nameof(Issue.Description)).AsString(1000)
                .WithColumn(nameof(Issue.Deadline)).AsDateTime2().Nullable()
                .WithColumn(nameof(Issue.Priority)).AsByte()
                .WithColumn(nameof(Issue.Status)).AsByte()
                .WithColumn(nameof(Issue.CreatedBy)).AsInt32().Nullable()
                .WithColumn(nameof(Issue.CreatedAt)).AsDateTime2()
                .WithColumn(nameof(Issue.LastModified)).AsDateTime2();
        }
    }
}