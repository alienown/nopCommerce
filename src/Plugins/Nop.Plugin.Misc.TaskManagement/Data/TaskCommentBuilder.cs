using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class TaskCommentBuilder : NopEntityBuilder<TaskComment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TaskComment.TaskId)).AsInt32().ForeignKey<Task>(onDelete: Rule.None)
                .WithColumn(nameof(TaskComment.Content)).AsString(256)
                .WithColumn(nameof(TaskComment.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(TaskComment.CreatedAt)).AsDateTime2();
        }
    }
}