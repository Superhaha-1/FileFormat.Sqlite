﻿// <auto-generated />
using System;
using FileFormat.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FileFormat.Sqlite.Migrations
{
    [DbContext(typeof(FileFormatContext))]
    [Migration("20181017085001_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("FileFormat.Sqlite.Models.Data", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Value");

                    b.HasKey("Key");

                    b.ToTable("Datas");
                });

            modelBuilder.Entity("FileFormat.Sqlite.Models.Parameter", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("Parameters");
                });
#pragma warning restore 612, 618
        }
    }
}
