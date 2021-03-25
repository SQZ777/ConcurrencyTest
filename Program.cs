using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // #region ConcurrencyHandlingCode
            using var context = new PersonContext();
            context.Database.EnsureCreated();
            // Fetch a person from database and change phone number
            var person = context.People.Single(p => p.PersonId == 1);
            person.PhoneNumber = "555-555-5555";
            // Change the person's name in the database to simulate a concurrency conflict
            context.Database.ExecuteSqlRaw("UPDATE dbo.People SET FirstName = 'Darren321' WHERE PersonId = 1");
            context.SaveChanges();

            // var saved = false;
            // while (!saved)
            // {
            //     try
            //     {
            //         // Attempt to save changes to the database
            //         context.SaveChanges();
            //         saved = true;
            //     }
            //     catch (DbUpdateConcurrencyException ex)
            //     {
            //         foreach (var entry in ex.Entries)
            //         {
            //             if (entry.Entity is Person)
            //             {
            //                 var proposedValues = entry.CurrentValues;
            //                 var databaseValues = entry.GetDatabaseValues();
            //
            //                 foreach (var property in proposedValues.Properties)
            //                 {
            //                     var proposedValue = proposedValues[property];
            //                     var databaseValue = databaseValues[property];
            //
            //                     // TODO: decide which value should be written to database
            //                     // proposedValues[property] = <value to be saved>;
            //                 }
            //
            //                 // Refresh original values to bypass next concurrency check
            //                 entry.OriginalValues.SetValues(databaseValues);
            //             }
            //             else
            //             {
            //                 throw new NotSupportedException(
            //                     "Don't know how to handle concurrency conflicts for "
            //                     + entry.Metadata.Name);
            //             }
            //         }
            //     }
            // }
            // #endregion
        }
    }

    public class PersonContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Requires NuGet package Microsoft.EntityFrameworkCore.SqlServer
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;");
        }
    }

    public class Person
    {
        public int PersonId { get; set; }


        [ConcurrencyCheck]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [ConcurrencyCheck]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(50)")] public string PhoneNumber { get; set; }
    }
}