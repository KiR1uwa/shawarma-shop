﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShawarmaShop.Infrastructure.Data;

#nullable disable

namespace ShawarmaShop.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("Authentication.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@example.com",
                            PasswordHash = "21232f297a57a5a743894a0e4a801fc3",
                            Phone = "+48000000000",
                            Role = "admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("Client_.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "john@example.com",
                            Name = "John Doe",
                            Phone = "+48123456789"
                        },
                        new
                        {
                            Id = 2,
                            Email = "jane@example.com",
                            Name = "Jane Smith",
                            Phone = "+48987654321"
                        },
                        new
                        {
                            Id = 3,
                            Email = "mike@example.com",
                            Name = "Mike Johnson",
                            Phone = "+48555666777"
                        },
                        new
                        {
                            Id = 4,
                            Email = "anna@example.com",
                            Name = "Anna Kowalski",
                            Phone = "+48111222333"
                        });
                });

            modelBuilder.Entity("OrderItem_.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("OrderId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShawarmaId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ShawarmaId");

                    b.ToTable("OrderItems");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            OrderId = 1,
                            Quantity = 2,
                            ShawarmaId = 1
                        },
                        new
                        {
                            Id = 2,
                            OrderId = 1,
                            Quantity = 1,
                            ShawarmaId = 3
                        },
                        new
                        {
                            Id = 3,
                            OrderId = 2,
                            Quantity = 1,
                            ShawarmaId = 2
                        },
                        new
                        {
                            Id = 4,
                            OrderId = 3,
                            Quantity = 3,
                            ShawarmaId = 4
                        },
                        new
                        {
                            Id = 5,
                            OrderId = 3,
                            Quantity = 1,
                            ShawarmaId = 5
                        });
                });

            modelBuilder.Entity("Order_.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClientID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DeliveryAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientID");

                    b.ToTable("Orders");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClientID = 1,
                            Comment = "Extra sauce please",
                            CreatedAt = new DateTime(2024, 5, 28, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            DeliveryAt = new DateTime(2024, 5, 28, 12, 30, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = 2,
                            ClientID = 2,
                            Comment = "No onions",
                            CreatedAt = new DateTime(2024, 5, 29, 14, 30, 0, 0, DateTimeKind.Unspecified),
                            DeliveryAt = new DateTime(2024, 5, 29, 15, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = 3,
                            ClientID = 3,
                            CreatedAt = new DateTime(2024, 5, 30, 16, 45, 0, 0, DateTimeKind.Unspecified),
                            DeliveryAt = new DateTime(2024, 5, 30, 17, 15, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("Shawarma_.Models.Shawarma", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ingredients")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Shawarmas");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Ingredients = "Chicken, lettuce, tomato, cucumber, sauce",
                            Name = "Classic Chicken",
                            Price = 15.50m
                        },
                        new
                        {
                            Id = 2,
                            Ingredients = "Beef, onion, pepper, cheese, spicy sauce",
                            Name = "Beef Deluxe",
                            Price = 18.00m
                        },
                        new
                        {
                            Id = 3,
                            Ingredients = "Falafel, lettuce, tomato, cucumber, hummus",
                            Name = "Vegetarian",
                            Price = 12.50m
                        },
                        new
                        {
                            Id = 4,
                            Ingredients = "Lamb, hot pepper, onion, garlic sauce",
                            Name = "Spicy Lamb",
                            Price = 20.00m
                        },
                        new
                        {
                            Id = 5,
                            Ingredients = "Chicken, beef, vegetables, mixed sauce",
                            Name = "Mixed Meat",
                            Price = 22.00m
                        });
                });

            modelBuilder.Entity("OrderItem_.Models.OrderItem", b =>
                {
                    b.HasOne("Order_.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shawarma_.Models.Shawarma", "Shawarma")
                        .WithMany("OrderItems")
                        .HasForeignKey("ShawarmaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Shawarma");
                });

            modelBuilder.Entity("Order_.Models.Order", b =>
                {
                    b.HasOne("Client_.Models.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Client_.Models.Client", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Order_.Models.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Shawarma_.Models.Shawarma", b =>
                {
                    b.Navigation("OrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}
