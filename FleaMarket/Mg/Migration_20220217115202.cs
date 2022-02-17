using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mg
{
    [Migration(20220217115202)]
    public class Migration_20220217115202: FluentMigrator.Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            Create.Table("\"Cities\"")
                .WithColumn("\"CityId\"").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString(50).NotNullable();

            Create.Table("\"Users\"")
                .WithColumn("\"UserId\"").AsGuid().PrimaryKey()
                .WithColumn("\"Surname\"").AsString(100).NotNullable()
                .WithColumn("\"Name\"").AsString(50).NotNullable()
                .WithColumn("\"PhoneNumber\"").AsString(18).NotNullable()
                .WithColumn("\"VkAddress\"").AsString(150).Nullable()
                .WithColumn("\"Rating\"").AsFloat().WithDefaultValue("0")
                .WithColumn("\"CityId\"").AsInt32().NotNullable()
                .WithColumn("\"IsDeleted\"").AsBoolean().NotNullable();

            Create.Table("\"UserPasswords\"")
                .WithColumn("\"UserPasswordId\"").AsGuid().PrimaryKey()
                .WithColumn("\"Password\"").AsString(250).NotNullable()
                .WithColumn("\"UserId\"").AsGuid().NotNullable().Unique();

            Create.ForeignKey()
                .FromTable("\"UserPasswords\"").ForeignColumn("\"UserId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.ForeignKey()
                .FromTable("\"Users\"").ForeignColumn("\"CityId\"")
                .ToTable("\"Cities\"").PrimaryColumn("\"CityId\"");

            Create.Table("\"Categories\"")
                .WithColumn("\"CategoryId\"").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString(250).NotNullable()
                .WithColumn("\"CategoryParent\"").AsInt32().NotNullable();

            Create.Table("\"Products\"")
                .WithColumn("\"ProductId\"").AsGuid().PrimaryKey()
                .WithColumn("\"Name\"").AsString(50).NotNullable()
                .WithColumn("\"FirstPhoto\"").AsString(250).NotNullable()
                .WithColumn("\"Description\"").AsString().NotNullable()
                .WithColumn("\"CityId\"").AsInt32().NotNullable()
                .WithColumn("\"IsActive\"").AsInt32().NotNullable()
                .WithColumn("\"CategoryId\"").AsInt32().NotNullable()
                .WithColumn("\"UserId\"").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Products\"").ForeignColumn("\"CityId\"")
                .ToTable("\"Cities\"").PrimaryColumn("\"CityId\"");

            Create.ForeignKey()
                .FromTable("\"Products\"").ForeignColumn("\"CategoryId\"")
                .ToTable("\"Categories\"").PrimaryColumn("\"CategoryId\"");

            Create.ForeignKey()
                .FromTable("\"Products\"").ForeignColumn("\"UserId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.Table("\"ProductPhotos\"")
                .WithColumn("\"PhotoId\"").AsGuid().PrimaryKey()
                .WithColumn("\"Link\"").AsString(250).NotNullable()
                .WithColumn("\"ProductId\"").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("\"ProductPhotos\"").ForeignColumn("\"ProductId\"")
                .ToTable("\"Products\"").PrimaryColumn("\"ProductId\"");

            Create.Table("\"Deals\"")
                .WithColumn("\"DealId\"").AsGuid().PrimaryKey()
                .WithColumn("\"UserMaster\"").AsGuid().NotNullable()
                .WithColumn("\"ProductMaster\"").AsGuid().NotNullable()
                .WithColumn("\"UserRecipient\"").AsGuid().NotNullable()
                .WithColumn("\"ProductRecipient\"").AsGuid().NotNullable()
                .WithColumn("\"IsActive\"").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Deals\"").ForeignColumn("\"UserMaster\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.ForeignKey()
                .FromTable("\"Deals\"").ForeignColumn("\"UserRecipient\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.Table("\"Ratings\"")
                .WithColumn("\"RatingId\"").AsGuid().PrimaryKey()
                .WithColumn("\"Grade\"").AsInt32().NotNullable()
                .WithColumn("\"UserMasterId\"").AsGuid().NotNullable()
                .WithColumn("\"UserRecipientId\"").AsGuid().NotNullable()
                .WithColumn("\"DealId\"").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Ratings\"").ForeignColumn("\"UserMasterId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.ForeignKey()
                .FromTable("\"Ratings\"").ForeignColumn("\"UserRecipientId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Create.ForeignKey()
                .FromTable("\"Ratings\"").ForeignColumn("\"DealId\"")
                .ToTable("\"Deals\"").PrimaryColumn("\"DealId\"");

            Create.Table("\"Favorites\"")
                .WithColumn("\"FavoriteId\"").AsGuid().PrimaryKey()
                .WithColumn("\"ProductId\"").AsGuid().NotNullable()
                .WithColumn("\"UserId\"").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("\"Favorites\"").ForeignColumn("\"ProductId\"")
                .ToTable("\"Products\"").PrimaryColumn("\"ProductId\"");

            Create.ForeignKey()
                .FromTable("\"Favorites\"").ForeignColumn("\"UserId\"")
                .ToTable("\"Users\"").PrimaryColumn("\"UserId\"");

            Insert.IntoTable("\"Categories\"")
                .Row(new { Name = "Товары для дома", CategoryParent = "-1" })
                .Row(new { Name = "Электроника", CategoryParent = "-1" })
                .Row(new { Name = "Компьютерные комплектующие", CategoryParent = "-1" })
                .Row(new { Name = "Процессоры", CategoryParent = "3" })
                .Row(new { Name = "Видеокарты", CategoryParent = "3" })
                .Row(new { Name = "Intel", CategoryParent = "4" })
                .Row(new { Name = "AMD", CategoryParent = "4" });

            Insert.IntoTable("\"Cities\"")
                .Row(new { Name = "Кострома" })
                .Row(new { Name = "Москва" });
        }
    }
}
