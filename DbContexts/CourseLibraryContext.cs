using System;
using CourseLibrary.Entites;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.DbContexts
{
    public class CourseLibraryContext: DbContext
    {
     public CourseLibraryContext(DbContextOptions<CourseLibraryContext> options) : base(options)
     {} 
    public DbSet<Authors> Authors {get;set;}
    public DbSet<Courses> Courses {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Authors>().HasData(
                new Authors()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Joao",
                    LastName = "Lima",
                    DateOfBirth = new DateTime(1650, 7, 23),
                    MainCategory = "Ships"
                },
                new Authors()
                {
                    Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    FirstName = "Nancy",
                    LastName = "Ferrari",
                    DateOfBirth = new DateTime(1668, 5, 21),
                    MainCategory = "Rum"
                },
                new Authors()
                {
                    Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    FirstName = "Eli",
                    LastName = "Ivory Bones Sweet",
                    DateOfBirth = new DateTime(1701, 12, 16),
                    MainCategory = "Singing"
                },
                new Authors()
                {
                    Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                    FirstName = "Neymar",
                    LastName = "Junior",
                    DateOfBirth = new DateTime(1702, 3, 6),
                    MainCategory = "Singing"
                },
                new Authors()
                {
                    Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                    FirstName = "Elano",
                    LastName = "Jogador do Santos",
                    DateOfBirth = new DateTime(1690, 11, 23),
                    MainCategory = "Maps"
                },
                new Authors()
                {
                    Id = Guid.Parse("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                    FirstName = "Rutherford",
                    LastName = "Lima Junior Pereira",
                    DateOfBirth = new DateTime(1723, 4, 5),
                    MainCategory = "General debauchery"
                },
                new Authors()
                {
                    Id = Guid.Parse("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                    FirstName = "Atielis",
                    LastName = "Silva",
                    DateOfBirth = new DateTime(1721, 10, 11),
                    MainCategory = "Rum"
                }
                );

            modelBuilder.Entity<Courses>().HasData(
               new Courses
               {
                   Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Commandeering a Ship Without Getting Caught",
                   Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
               },
               new Courses
               {
                   Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Overthrowing Mutiny",
                   Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
               },
               new Courses
               {
                   Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                   AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                   Title = "Avoiding Brawls While Drinking as Much Rum as You Desire",
                   Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk."
               },
               new Courses
               {
                   Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                   AuthorId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                   Title = "Singalong Pirate Hits",
                   Description = "In this course you'll learn how to sing all-time favourite pirate songs without sounding like you actually know the words or how to hold a note."
               }
               );
            base.OnModelCreating(modelBuilder);
        }
    }
}
