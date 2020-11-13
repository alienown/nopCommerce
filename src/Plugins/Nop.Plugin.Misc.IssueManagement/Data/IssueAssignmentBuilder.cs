using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Data
{
    public class IssueAssignmentBuilder : NopEntityBuilder<IssueAssignment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(IssueAssignment.ObjectId)).AsInt32().Nullable()
                .WithColumn(nameof(IssueAssignment.AssignmentType)).AsByte()
                .WithColumn(nameof(IssueAssignment.IssueId)).AsInt32().ForeignKey<Issue>(onDelete: Rule.None)
                .WithColumn(nameof(IssueAssignment.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssueAssignment.CreatedAt)).AsDateTime2();
        }
    }
}