using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220325203104)]
    public class Migration_20220330172705 : FluentMigrator.Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Create.Table("\"Messages\"")
                .WithColumn("\"UserId\"").AsGuid().NotNullable()
                .WithColumn("\"User\"").AsString(50).NotNullable()
                .WithColumn("\"Text\"").AsString(250).NotNullable()
                .WithColumn("\"IsRead\"").AsBoolean().NotNullable()
                .WithColumn("\"DialogId\"").AsGuid().NotNullable()
                .WithColumn("\"Date\"").AsDateTime().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Messages\"").ForeignColumn("\"UserId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");
            Create.ForeignKey()
                .FromTable("\"Messages\"").ForeignColumn("\"DialogId\"")
                .ToTable("\"Dialogues\"").PrimaryColumn("\"DialogId\"");
        }
    }
}
