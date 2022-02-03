using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220203150801)]
    public class Migration_20220203150801 : FluentMigrator.Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Create.Table("citys")
                .WithColumn("cityid").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString(50).NotNullable();

            Create.Table("users")
                .WithColumn("userid").AsGuid().PrimaryKey()
                .WithColumn("surname").AsString(100).NotNullable()
                .WithColumn("\"Name\"").AsString(50).NotNullable()
                .WithColumn("phonenumber").AsString(18).NotNullable()
                .WithColumn("vkaddress").AsString(150).Nullable()
                .WithColumn("rating").AsInt32().WithDefaultValue("0")
                .WithColumn("cityid").AsInt32().NotNullable()
                .WithColumn("isdeleted").AsBoolean().NotNullable();

            Create.Table("userpasswords")
                .WithColumn("userpasswordid").AsGuid().PrimaryKey()
                .WithColumn("\"Password\"").AsString(250).NotNullable()
                .WithColumn("userid").AsGuid().NotNullable().Unique();

            Create.ForeignKey()
                .FromTable("userpasswords").ForeignColumn("userid")
                .ToTable("users").PrimaryColumn("userid");

            Create.ForeignKey()
                .FromTable("users").ForeignColumn("cityid")
                .ToTable("citys").PrimaryColumn("cityid");

            Create.Table("categorys")
                .WithColumn("categoryid").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString(250).NotNullable()
                .WithColumn("categoryparent").AsInt32().NotNullable();

            Create.Table("products")
                .WithColumn("productid").AsGuid().PrimaryKey()
                .WithColumn("\"Name\"").AsString(50).NotNullable()
                .WithColumn("firstphoto").AsString(250).NotNullable()
                .WithColumn("description").AsString().NotNullable()
                .WithColumn("cityid").AsInt32().NotNullable()
                .WithColumn("isactive").AsInt32().NotNullable()
                .WithColumn("categoryid").AsInt32().NotNullable()
                .WithColumn("userid").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("products").ForeignColumn("cityid")
                .ToTable("citys").PrimaryColumn("cityid");

            Create.ForeignKey()
                .FromTable("products").ForeignColumn("categoryid")
                .ToTable("categorys").PrimaryColumn("categoryid");

            Create.ForeignKey()
                .FromTable("products").ForeignColumn("userid")
                .ToTable("users").PrimaryColumn("userid");

            Create.Table("productphotos")
                .WithColumn("photoid").AsGuid().PrimaryKey()
                .WithColumn("\"Link\"").AsString(250).NotNullable()
                .WithColumn("productid").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("productphotos").ForeignColumn("productid")
                .ToTable("products").PrimaryColumn("productid");

            Insert.IntoTable("categorys")
                .Row(new { Name = "Товары для дома", categoryparent = "-1" });

            Insert.IntoTable("citys")
                .Row(new { Name = "Кострома" });
        }
    }
}
