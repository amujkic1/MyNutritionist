using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Models;
using System;
using System.Diagnostics;

namespace MyNutritionist.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Person { get; set; }
        public DbSet<RegisteredUser> RegisteredUser { get; set; }
        public DbSet<PremiumUser> PremiumUser { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<DietPlan> DietPlan { get; set; }
        public DbSet<DailyDiet> DailyDiet { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<Recipe_Ingredient> RecipeIngredient { get; set; }
        public DbSet<Nutritionist> Nutritionist { get; set; }
        public DbSet<Leaderboard> Leaderboard { get; set; }
        public DbSet<PhysicalActivity> PhysicalActivity { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>().ToTable("Person");
            builder.Entity<RegisteredUser>().ToTable("RegisteredUser");
            builder.Entity<PremiumUser>().ToTable("PremiumUser");
            builder.Entity<Admin>().ToTable("Admin");
            builder.Entity<Card>().ToTable("Card");
            builder.Entity<DietPlan>().ToTable("DietPlan");
            builder.Entity<DailyDiet>().ToTable("DailyDiet");
            builder.Entity<Ingredient>().ToTable("Ingredient");
            builder.Entity<Recipe>().ToTable("Recipe");
            builder.Entity<Progress>().ToTable("Progress");
            builder.Entity<Recipe_Ingredient>().ToTable("RecipeIngredient");
            builder.Entity<Leaderboard>().ToTable("Leaderboard");
            builder.Entity<PhysicalActivity>().ToTable("PhysicalActivity");
            builder.Entity<Nutritionist>().ToTable("Nutritionist");
            base.OnModelCreating(builder);

        }

    }
}