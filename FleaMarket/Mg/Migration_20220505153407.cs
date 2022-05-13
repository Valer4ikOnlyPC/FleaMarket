using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220505153407)]
    public class Migration_20220505153407 : FluentMigrator.Migration
    {
        public override void Down()
        {
            Delete.Column("\"Location\"").FromTable("\"ProductPhotos\"");
        }

        public override void Up()
        {
            Alter.Table("\"ProductPhotos\"")
                .AddColumn("\"Latitude\"").AsDecimal()
                .AddColumn("\"Longitude\"").AsDecimal();
        }
    }
}
