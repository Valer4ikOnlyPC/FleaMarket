using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Npgsql;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Migrations
{
    class Program
    {
        public static string constring = "Host=localhost;Port=5432;Database=testDB1;Username=postgres;Password=2077";
        static void Main(string[] args)
        {
            var serviceProvider = CreateServices();
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
            Console.ReadKey();
        }
        private static IServiceProvider CreateServices()
        {
            return new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres92()
                    .WithGlobalConnectionString(constring)
                    .ScanIn(typeof(Migration_20220128200001).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            NpgsqlConnection db = new NpgsqlConnection(constring);
            db.Execute("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
            
            runner.MigrateUp();
        }
    }
}
