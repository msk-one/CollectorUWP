using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Collector_local_db;

namespace Collector_local_db.Migrations
{
    [DbContext(typeof(CollectorContext))]
    [Migration("20151115130258_InitMigration")]
    partial class InitMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964");

            modelBuilder.Entity("Collector_local_db.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Cname");

                    b.HasKey("CategoryId");
                });

            modelBuilder.Entity("Collector_local_db.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Curln");

                    b.Property<string>("Cursi");

                    b.Property<string>("Cursn");

                    b.HasKey("CurrencyId");
                });

            modelBuilder.Entity("Collector_local_db.Entry", b =>
                {
                    b.Property<int>("EntryId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("Amount");

                    b.Property<int?>("CurrencyCurrencyId");

                    b.Property<DateTime>("Date");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Desc");

                    b.Property<int?>("ObjectObjectId");

                    b.Property<int>("Priority");

                    b.Property<string>("Title");

                    b.Property<int?>("TypeTypeId");

                    b.Property<string>("Who");

                    b.HasKey("EntryId");
                });

            modelBuilder.Entity("Collector_local_db.Object", b =>
                {
                    b.Property<int>("ObjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryCategoryId");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.Property<int>("Quantity");

                    b.HasKey("ObjectId");
                });

            modelBuilder.Entity("Collector_local_db.Type", b =>
                {
                    b.Property<int>("TypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Typename");

                    b.HasKey("TypeId");
                });

            modelBuilder.Entity("Collector_local_db.Entry", b =>
                {
                    b.HasOne("Collector_local_db.Currency")
                        .WithMany()
                        .ForeignKey("CurrencyCurrencyId");

                    b.HasOne("Collector_local_db.Object")
                        .WithMany()
                        .ForeignKey("ObjectObjectId");

                    b.HasOne("Collector_local_db.Type")
                        .WithMany()
                        .ForeignKey("TypeTypeId");
                });

            modelBuilder.Entity("Collector_local_db.Object", b =>
                {
                    b.HasOne("Collector_local_db.Category")
                        .WithMany()
                        .ForeignKey("CategoryCategoryId");
                });
        }
    }
}
