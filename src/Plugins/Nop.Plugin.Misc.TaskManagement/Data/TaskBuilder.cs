using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class TaskBuilder : NopEntityBuilder<Task>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Task.Name)).AsString(50)
                .WithColumn(nameof(Task.Description)).AsString(1000)
                .WithColumn(nameof(Task.Deadline)).AsDateTime2().Nullable()
                .WithColumn(nameof(Task.Priority)).AsByte()
                .WithColumn(nameof(Task.Status)).AsByte()
                .WithColumn(nameof(Task.Deleted)).AsBinary()
                .WithColumn(nameof(Task.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(Task.CreatedAt)).AsDateTime2()
                .WithColumn(nameof(Task.LastModified)).AsDateTime2();
        }
    }
}