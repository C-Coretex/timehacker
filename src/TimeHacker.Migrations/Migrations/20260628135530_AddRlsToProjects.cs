using Microsoft.EntityFrameworkCore.Migrations;
using TimeHacker.Migrations.Configuration;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRlsToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Category\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"Category_rls_policy\" ON \"Category\";\r\nCREATE POLICY \"Category_rls_policy\" ON \"Category\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"ScheduleEntity\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"ScheduleEntity_rls_policy\" ON \"ScheduleEntity\";\r\nCREATE POLICY \"ScheduleEntity_rls_policy\" ON \"ScheduleEntity\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"ScheduleSnapshot\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"ScheduleSnapshot_rls_policy\" ON \"ScheduleSnapshot\";\r\nCREATE POLICY \"ScheduleSnapshot_rls_policy\" ON \"ScheduleSnapshot\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"ScheduledCategory\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"ScheduledCategory_rls_policy\" ON \"ScheduledCategory\";\r\nCREATE POLICY \"ScheduledCategory_rls_policy\" ON \"ScheduledCategory\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"ScheduledTask\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"ScheduledTask_rls_policy\" ON \"ScheduledTask\";\r\nCREATE POLICY \"ScheduledTask_rls_policy\" ON \"ScheduledTask\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"Tag\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"Tag_rls_policy\" ON \"Tag\";\r\nCREATE POLICY \"Tag_rls_policy\" ON \"Tag\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"DynamicTask\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"DynamicTask_rls_policy\" ON \"DynamicTask\";\r\nCREATE POLICY \"DynamicTask_rls_policy\" ON \"DynamicTask\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");

            migrationBuilder.Sql("ALTER TABLE \"FixedTask\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"FixedTask_rls_policy\" ON \"FixedTask\";\r\nCREATE POLICY \"FixedTask_rls_policy\" ON \"FixedTask\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"Category_rls_policy\" ON \"Category\";\r\nALTER TABLE \"Category\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"ScheduleEntity_rls_policy\" ON \"ScheduleEntity\";\r\nALTER TABLE \"ScheduleEntity\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"ScheduleSnapshot_rls_policy\" ON \"ScheduleSnapshot\";\r\nALTER TABLE \"ScheduleSnapshot\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"ScheduledCategory_rls_policy\" ON \"ScheduledCategory\";\r\nALTER TABLE \"ScheduledCategory\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"ScheduledTask_rls_policy\" ON \"ScheduledTask\";\r\nALTER TABLE \"ScheduledTask\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"Tag_rls_policy\" ON \"Tag\";\r\nALTER TABLE \"Tag\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"DynamicTask_rls_policy\" ON \"DynamicTask\";\r\nALTER TABLE \"DynamicTask\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("DROP POLICY IF EXISTS \"FixedTask_rls_policy\" ON \"FixedTask\";\r\nALTER TABLE \"FixedTask\" DISABLE ROW LEVEL SECURITY;");
        }
    }
}
