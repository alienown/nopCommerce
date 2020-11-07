using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class TaskAssignmentBuilder : NopEntityBuilder<TaskAssignment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TaskAssignment.ObjectId)).AsInt32().Nullable()
                .WithColumn(nameof(TaskAssignment.TaskId)).AsInt32().ForeignKey<Task>(onDelete: Rule.None)
                .WithColumn(nameof(TaskAssignment.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(TaskAssignment.CreatedAt)).AsDateTime2();
        }
    }
}