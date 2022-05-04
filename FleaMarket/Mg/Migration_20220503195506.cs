using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220503195506)]
    public class Migration_20220503195506 : FluentMigrator.Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Alter.Table("\"ProductPhotos\"")
                .AddColumn("\"Location\"").AsString(200);
        }
    }
}
