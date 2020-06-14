﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sidekick.Database;

namespace Sidekick.Database.Migrations
{
    [DbContext(typeof(SidekickContext))]
    partial class SidekickContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("sidekick")
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("Sidekick.Database.Windows.Window", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<double>("Height")
                        .HasColumnType("REAL");

                    b.Property<double>("Width")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Windows");
                });
#pragma warning restore 612, 618
        }
    }
}
