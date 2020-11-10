using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Data
{
    public class IssuePersonInvolvedBuilder : NopEntityBuilder<IssuePersonInvolved>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(IssuePersonInvolved.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssuePersonInvolved.IssueId)).AsInt32().ForeignKey<Issue>(onDelete: Rule.None)
                .WithColumn(nameof(IssuePersonInvolved.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssuePersonInvolved.CreatedAt)).AsDateTime2();
        }
    }
}