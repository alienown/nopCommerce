using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.TaskManagement.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/11/07 16:30:00", "Misc.TaskManagement base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<Task>(Create);
            _migrationManager.BuildTable<TaskPersonInvolved>(Create);
            _migrationManager.BuildTable<TaskAssignment>(Create);
            _migrationManager.BuildTable<TaskComment>(Create);
            _migrationManager.BuildTable<TaskHistory>(Create);
        }
    }
}