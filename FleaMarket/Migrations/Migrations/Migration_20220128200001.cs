﻿using Dapper;
using FluentMigrator;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    [Migration(20220128200001)]
    public class Migration_20220128200001 : Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {


            Create.Table("citys")
                .WithColumn("cityid").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString().NotNullable();

            Create.Table("userpasswords")
                .WithColumn("userpasswordid").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("\"Password\"").AsString().NotNullable();

            Create.Table("users")
                .WithColumn("userid").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("surname").AsString().NotNullable()
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("phonenumber").AsString().NotNullable()
                .WithColumn("vkaddress").AsString().Nullable()
                .WithColumn("rating").AsInt32()
                .WithColumn("cityid").AsInt32().NotNullable()
                .WithColumn("isdeleted").AsBoolean().NotNullable()
                .WithColumn("passwordid").AsGuid().NotNullable();

            Create.ForeignKey()
                .FromTable("users").ForeignColumn("passwordid")
                .ToTable("userpasswords").PrimaryColumn("userpasswordid");

            Create.ForeignKey()
                .FromTable("users").ForeignColumn("cityid")
                .ToTable("citys").PrimaryColumn("cityid");

            Create.Table("categorys")
                .WithColumn("categoryid").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("categoryparent").AsInt32().NotNullable();

            Create.Table("products")
                .WithColumn("productid").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("\"Name\"").AsString().NotNullable()
                .WithColumn("firstphoto").AsString().NotNullable()
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
                .WithColumn("photoid").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("\"Link\"").AsString().NotNullable()
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