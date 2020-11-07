﻿using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Misc.TaskManagement.Data
{
    public class IssueAssignmentBuilder : NopEntityBuilder<IssueAssignment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(IssueAssignment.ObjectId)).AsInt32().Nullable()
                .WithColumn(nameof(IssueAssignment.TaskId)).AsInt32().ForeignKey<Issue>(onDelete: Rule.None)
                .WithColumn(nameof(IssueAssignment.CreatedBy)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(IssueAssignment.CreatedAt)).AsDateTime2();
        }
    }
}