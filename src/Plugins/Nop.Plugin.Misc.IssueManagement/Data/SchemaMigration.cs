using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.IssueManagement.Domain;

namespace Nop.Plugin.Misc.IssueManagement.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/11/07 16:30:00", "Misc.IssueManagement base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<Issue>(Create);
            _migrationManager.BuildTable<IssuePersonInvolved>(Create);
            _migrationManager.BuildTable<IssueAssignment>(Create);
            _migrationManager.BuildTable<IssueComment>(Create);
            _migrationManager.BuildTable<IssueHistory>(Create);
        }
    }
}