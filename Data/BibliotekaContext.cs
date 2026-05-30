using DrugiKlkPrimer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugiKlkPrimer.Data
{
    internal class BibliotekaContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=BibliotekaDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>(entiry =>
            { 
                entiry.HasKey(g=>g.Id);

                entiry.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

                entiry.HasMany(g => g.Books)
                .WithOne(b => b.Genre)
                .HasForeignKey(b => b.GenreId);


            }
            );


            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.PublicationYear).IsRequired();
                entity.Property(g=>g.Title).IsRequired().HasMaxLength(150);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g=> g.FirstName).IsRequired();
                entity.Property(g=>g.LastName).IsRequired();
            });
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(g=>g.Id);

                entity.HasOne(g=>g.Book)
                    .WithMany(b=>b.Loans)
                    .HasForeignKey(g=>g.BookId);

                entity.HasOne(g=>g.Member)
                    .WithMany(m=>m.Loans)
                    .HasForeignKey(g=>g.MemberId);
            });
        }

    }
}
