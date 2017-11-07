﻿// <auto-generated />
using _02.SocialNetwork.Data;
using _02.SocialNetwork.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace _02.SocialNetwork.Data.Migrations
{
    [DbContext(typeof(SocialNetworkDbContext))]
    [Migration("20170928071415_RolesToAlbumUserTableAdded")]
    partial class RolesToAlbumUserTableAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("_02.SocialNetwork.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BackgroundColor")
                        .IsRequired();

                    b.Property<int>("CreatorId");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumPicture", b =>
                {
                    b.Property<int>("AlbumId");

                    b.Property<int>("PictureId");

                    b.HasKey("AlbumId", "PictureId");

                    b.HasIndex("PictureId");

                    b.ToTable("AlbumPicture");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumTag", b =>
                {
                    b.Property<int>("AlbumId");

                    b.Property<int>("TagId");

                    b.HasKey("AlbumId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("AlbumTag");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumUser", b =>
                {
                    b.Property<int>("AlbumId");

                    b.Property<int>("UserId");

                    b.Property<int>("Role");

                    b.HasKey("AlbumId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("AlbumUser");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.Friendship", b =>
                {
                    b.Property<int>("FromUserId");

                    b.Property<int>("ToUserId");

                    b.HasKey("FromUserId", "ToUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.Picture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Caption")
                        .IsRequired();

                    b.Property<string>("Path")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Age");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("LastTimeLoggedIn");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("ProfilePicture")
                        .HasMaxLength(1024);

                    b.Property<DateTime>("RegisteredOn");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.Album", b =>
                {
                    b.HasOne("_02.SocialNetwork.Models.User", "Creator")
                        .WithMany("Albums")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumPicture", b =>
                {
                    b.HasOne("_02.SocialNetwork.Models.Album", "Album")
                        .WithMany("Pictures")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("_02.SocialNetwork.Models.Picture", "Picture")
                        .WithMany("Albums")
                        .HasForeignKey("PictureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumTag", b =>
                {
                    b.HasOne("_02.SocialNetwork.Models.Album", "Album")
                        .WithMany("Tags")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("_02.SocialNetwork.Models.Tag", "Tag")
                        .WithMany("Albums")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.AlbumUser", b =>
                {
                    b.HasOne("_02.SocialNetwork.Models.Album", "Album")
                        .WithMany("SharedAlbums")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("_02.SocialNetwork.Models.User", "User")
                        .WithMany("SharedAlbums")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("_02.SocialNetwork.Models.Friendship", b =>
                {
                    b.HasOne("_02.SocialNetwork.Models.User", "FromUser")
                        .WithMany("FromFriends")
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("_02.SocialNetwork.Models.User", "ToUser")
                        .WithMany("ToFriends")
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
