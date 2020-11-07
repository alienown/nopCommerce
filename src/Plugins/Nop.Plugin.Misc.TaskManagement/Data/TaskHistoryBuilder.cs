using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class TaskHistoryBuilder : NopEntityBuilder<TaskHistory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TaskHistory.TaskId)).AsInt32().ForeignKey<Task>(onDelete: Rule.None)
                .WithColumn(nameof(TaskHistory.ChangeType)).AsByte()
                .WithColumn(nameof(TaskHistory.NewValue)).AsString().Nullable()
                .WithColumn(nameof(TaskHistory.OldValue)).AsString().Nullable()
                .WithColumn(nameof(TaskHistory.ModifiedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(TaskHistory.ModifiedAt)).AsDateTime2();
        }
    }
}