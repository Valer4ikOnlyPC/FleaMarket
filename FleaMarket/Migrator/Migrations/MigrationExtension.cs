using FluentMigrator.Runner;
using FluentMigrator;
using FluentMigrator.Postgres;
using FluentMigrator.Exceptions;

namespace Migrator.Migrations
{
    public static class MigrationExtension
    {
        public static IApplicationBuilder Migrate(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp(20220128164600);
            return app;
        }
    }
}
