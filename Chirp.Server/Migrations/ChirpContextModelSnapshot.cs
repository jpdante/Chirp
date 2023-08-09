﻿// <auto-generated />
using System;
using Chirp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chirp.Server.Migrations
{
    [DbContext(typeof(ChirpContext))]
    partial class ChirpContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chirp.Entities.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("account_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AccountId"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<DateTime?>("LastConfirmedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_confirmed_at");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.HasKey("AccountId")
                        .HasName("pk_accounts");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_accounts_email");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Chirp.Entities.Attachment", b =>
                {
                    b.Property<long>("AttachmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("attachment_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AttachmentId"));

                    b.Property<string>("AttachmentData")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("attachment_data");

                    b.Property<byte>("AttachmentType")
                        .HasColumnType("smallint")
                        .HasColumnName("attachment_type");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("PostId")
                        .HasColumnType("bigint")
                        .HasColumnName("post_id");

                    b.HasKey("AttachmentId")
                        .HasName("pk_attachments");

                    b.HasIndex("PostId")
                        .HasDatabaseName("ix_attachments_post_id");

                    b.ToTable("attachments", (string)null);
                });

            modelBuilder.Entity("Chirp.Entities.Post", b =>
                {
                    b.Property<long>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("post_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("PostId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<long>("ProfileId")
                        .HasColumnType("bigint")
                        .HasColumnName("profile_id");

                    b.HasKey("PostId")
                        .HasName("pk_posts");

                    b.HasIndex("ProfileId")
                        .HasDatabaseName("ix_posts_profile_id");

                    b.ToTable("posts", (string)null);
                });

            modelBuilder.Entity("Chirp.Entities.Profile", b =>
                {
                    b.Property<long>("ProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("profile_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ProfileId"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_id");

                    b.Property<string>("BackgroundPicture")
                        .HasColumnType("text")
                        .HasColumnName("background_picture");

                    b.Property<string>("Biography")
                        .HasColumnType("text")
                        .HasColumnName("biography");

                    b.Property<string>("Handle")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("handle");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("text")
                        .HasColumnName("profile_picture");

                    b.HasKey("ProfileId")
                        .HasName("pk_profiles");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_profiles_account_id");

                    b.HasIndex("Handle")
                        .IsUnique()
                        .HasDatabaseName("ix_profiles_handle");

                    b.ToTable("profiles", (string)null);
                });

            modelBuilder.Entity("Chirp.Entities.Attachment", b =>
                {
                    b.HasOne("Chirp.Entities.Post", "Post")
                        .WithMany("Attachments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_attachments_posts_post_id");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Chirp.Entities.Post", b =>
                {
                    b.HasOne("Chirp.Entities.Profile", "Profile")
                        .WithMany("Posts")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_posts_profiles_profile_id");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("Chirp.Entities.Profile", b =>
                {
                    b.HasOne("Chirp.Entities.Account", "Account")
                        .WithMany("Profiles")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_profiles_accounts_account_id");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Chirp.Entities.Account", b =>
                {
                    b.Navigation("Profiles");
                });

            modelBuilder.Entity("Chirp.Entities.Post", b =>
                {
                    b.Navigation("Attachments");
                });

            modelBuilder.Entity("Chirp.Entities.Profile", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
