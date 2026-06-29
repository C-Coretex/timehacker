#pragma warning disable EF1001 // Internal EF Core API usage.

using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;
using TimeHacker.Infrastructure.Interceptors;

namespace TimeHacker.Migrations.Configuration;


/// <summary>
/// Custom EF Core model differ that auto-generates PostgreSQL Row-Level-Security DDL into migrations.
/// When an entity gains/loses the "Rls:Enabled" annotation (stamped by
/// <see cref="TimeHacker.Infrastructure.Configuration.UserScopedEntityConfigurationBase{T}"/>), it appends
/// ENABLE/DISABLE RLS + CREATE/DROP POLICY statements so the user-isolation policy ships with the schema
/// instead of being maintained by hand. Wired in via .ReplaceService&lt;IMigrationsModelDiffer, ...&gt;().
/// </summary>
internal sealed class RlsMigrationsModelDiffer : MigrationsModelDiffer
{
    //RLS is configured onlt for provided user. DB owner is not restricted by RLS at all.
    public const string RlsRole = "application_user";

    public RlsMigrationsModelDiffer(
        IRelationalTypeMappingSource typeMappingSource,
        IMigrationsAnnotationProvider migrationsAnnotationProvider,
        IRelationalAnnotationProvider relationalAnnotationProvider,
        IRowIdentityMapFactory rowIdentityMapFactory,
        CommandBatchPreparerDependencies commandBatchPreparerDependencies)
        : base(typeMappingSource, migrationsAnnotationProvider, relationalAnnotationProvider,
               rowIdentityMapFactory, commandBatchPreparerDependencies)
    { }

    public override IReadOnlyList<MigrationOperation> GetDifferences(
        IRelationalModel? source,
        IRelationalModel? target)
    {
        // Start from the normal schema diff, then layer RLS changes on top by comparing the RLS annotations
        // on each table between the source (previous) and target (current) models.
        var operations = base.GetDifferences(source, target).ToList();

        var targetEntities = target?.Model.GetEntityTypes() ?? [];
        var sourceEntities = source?.Model.GetEntityTypes() ?? [];

        foreach (var targetEntity in targetEntities)
        {
            var tableName = targetEntity.GetTableName();
            if (tableName is null) continue;

            var sourceEntity = sourceEntities
                .FirstOrDefault(e => e.GetTableName() == tableName);

            var rlsEnabled = targetEntity.FindAnnotation("Rls:Enabled")?.Value as bool?;
            var sourceRlsEnabled = sourceEntity?.FindAnnotation("Rls:Enabled")?.Value as bool?;

            if (rlsEnabled == true && sourceRlsEnabled != true)
            {
                // Annotation was added - enable RLS
                var tenantColumn = targetEntity.FindAnnotation("Rls:TenantColumn")?.Value as string 
                    ?? throw new ArgumentException("Tenant column annotation is required for RLS-enabled entities");

                operations.Add(BuildEnableRlsOperation(tableName, tenantColumn));
            }
            else if (rlsEnabled != true && sourceRlsEnabled == true)
            {
                // Annotation was removed - disable RLS
                operations.Add(BuildDisableRlsOperation(tableName));
            }
        }

        return operations;
    }

    // Builds the policy that scopes every row to the current user: the tenant column must equal the
    // session variable that UserSessionInterceptor sets per connection (app.user_id).
    private static SqlOperation BuildEnableRlsOperation(
        string tableName,
        string tenantColumn) => new()
        {
            Sql = $"""
            ALTER TABLE "{tableName}" ENABLE ROW LEVEL SECURITY;
            DROP POLICY IF EXISTS "{tableName}_rls_policy" ON "{tableName}";
            CREATE POLICY "{tableName}_rls_policy" ON "{tableName}"
                FOR ALL
                TO {RlsRole}
                USING ("{tenantColumn}" = current_setting('{UserSessionInterceptor.SessionUserIdParameterName}', true)::uuid);
            """
        };

    private static SqlOperation BuildDisableRlsOperation(string tableName) => new()
    {
        Sql = $"""
            DROP POLICY IF EXISTS "{tableName}_rls_policy" ON "{tableName}";
            ALTER TABLE "{tableName}" DISABLE ROW LEVEL SECURITY;
            """
    };
}
