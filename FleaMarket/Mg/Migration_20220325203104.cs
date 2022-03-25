using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220325203104)]
    public class Migration_20220325203104 : FluentMigrator.Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.Sql("CREATE INDEX products_idx ON \"Products\" USING GIN (to_tsvector('russian', \"Name\"||' '||\"Description\"));");

            Create.Table("\"Dialogues\"")
                .WithColumn("\"DialogId\"").AsGuid().PrimaryKey()
                .WithColumn("\"User1\"").AsGuid().NotNullable()
                .WithColumn("\"User2\"").AsGuid().NotNullable()
                .WithColumn("\"Path\"").AsString(250).NotNullable()
                .WithColumn("\"Date\"").AsDateTime().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Dialogues\"").ForeignColumn("\"User1\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");
            Create.ForeignKey()
                .FromTable("\"Dialogues\"").ForeignColumn("\"User2\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");
        }
    }
}
