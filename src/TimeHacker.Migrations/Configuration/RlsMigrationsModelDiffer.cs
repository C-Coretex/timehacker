#pragma warning disable EF1001 // Internal EF Core API usage.

using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;
using TimeHacker.Infrastructure.Interceptors;

namespace TimeHacker.Migrations.Configuration;


internal sealed class RlsMigrationsModelDiffer : MigrationsModelDiffer
{
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
