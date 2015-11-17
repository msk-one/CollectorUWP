using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace Collector_local_db
{
    public class CollectorContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Object> Objects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = "CollectorDB.db";
            try
            {
                databaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch (InvalidOperationException)
            { }

            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make Blog.Url required
            //modelBuilder.Entity<Blog>()
             //   .Property(b => b.Url)
              //  .IsRequired();


        }
    }

    public class Entry
    {
        public int EntryId { get; set; }

        public Type Type { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Desc { get; set; }
        public Object Object { get; set; }
        public string Who { get; set; }
        public float Amount { get; set; }
        public int Priority { get; set; }
        public DateTime Deadline { get; set; }

        public Currency Currency { get; set; }
    }

    public class Object
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public int Quantity { get; set; }
        public Category Category { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Cname { get; set; }
    }

    public class Type
    {
        public int TypeId { get; set; }
        public string Typename { get; set; }
    }

    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Cursi { get; set; }
        public string Cursn { get; set; }
        public string Curln { get; set; }
    }
}