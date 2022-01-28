using FluentMigrator.Runner;
using FluentMigrator.Postgres;
using FluentMigrator;

namespace Migrator.Migrations
{
    [Migration(20220128164600)]
    public class Migration_20200601100000 : Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Create.Table("Citys")
                .WithColumn("CityId").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString().NotNullable();

            Create.Table("UserPasswords")
                .WithColumn("UserPasswordId").AsGuid().Identity().PrimaryKey()
                .WithColumn("\"Password\"").AsString().NotNullable();

            Create.Table("Users")
                .WithColumn("UserId").AsGuid().Identity().PrimaryKey()
                .WithColumn("Surname").AsString().NotNullable()
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("PhoneNumber").AsString().NotNullable()
                .WithColumn("VkAddress").AsString().Nullable()
                .WithColumn("Rating").AsInt32()
                .WithColumn("CityId").AsInt32().NotNullable().ForeignKey("Citys", "CityId")
                .WithColumn("IsDeleted").AsBoolean().NotNullable()
                .WithColumn("PasswordId").AsGuid().NotNullable().Unique().ForeignKey("UserPasswords", "UserPasswordId");

            Create.Index("IX_Passwords_Users")
                .OnTable("Users")
                .OnColumn("PasswordId")
                .Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Citys_Users")
                .OnTable("Citys")
                .OnColumn("CityId")
                .Ascending()
                .WithOptions().NonClustered();

            Create.Table("Categorys")
                .WithColumn("CategoryId ").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("CategoryParent").AsInt32().NotNullable();

            Create.Table("Products")
                .WithColumn("ProductId").AsGuid().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("FirstPhoto").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("CityId").AsInt32().NotNullable().ForeignKey("Citys", "CityId")
                .WithColumn("IsActive").AsInt32().NotNullable()
                .WithColumn("CategoryId").AsInt32().NotNullable().ForeignKey("Categorys", "CategoryId")
                .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("Users", "UserId");

            Create.Index("IX_Citys_Products")
                .OnTable("Citys")
                .OnColumn("CityId")
                .Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Categorys_Products")
                .OnTable("Categorys")
                .OnColumn("CategoryId")
                .Ascending()
                .WithOptions().NonClustered();

            Create.Table("ProductPhotos")
                .WithColumn("PhotoId").AsGuid().Identity().PrimaryKey()
                .WithColumn("\"Link\"").AsString().NotNullable()
                .WithColumn("ProductId").AsGuid().NotNullable().ForeignKey("Products", "ProductId");

            Create.Index("IX_Products_Photos")
                .OnTable("Products")
                .OnColumn("ProductId")
                .Ascending()
                .WithOptions().NonClustered();
        }
    }
}
