using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class TaskPersonInvolvedBuilder : NopEntityBuilder<TaskPersonInvolved>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TaskPersonInvolved.UserId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(TaskPersonInvolved.TaskId)).AsInt32().ForeignKey<Task>(onDelete: Rule.None)
                .WithColumn(nameof(TaskPersonInvolved.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(TaskPersonInvolved.CreatedAt)).AsDateTime2();
        }
    }
}