using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using Dapper;
using Mg;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace Mg
{
    public class Migration
    {
        private static IConfiguration _configuration;
        private static string _connString;

        public Migration(string connString)
        {
            _connString = connString;

            var serviceProvider = CreateServices();
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter connection string: ");
            var conn = Console.ReadLine();
            var migration = new Migration(conn);
            Console.ReadKey();
        }
        private static IServiceProvider CreateServices()
        {
            return new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres92()
                    .WithGlobalConnectionString(_connString)
                    .ScanIn(typeof(Migration_20220325203104).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }
    }
}







