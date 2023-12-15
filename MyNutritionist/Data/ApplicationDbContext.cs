using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Models;
using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace MyNutritionist.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<RegisteredUser> RegisteredUser { get; set; }
        public virtual DbSet<PremiumUser> PremiumUser { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<DietPlan> DietPlan { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<Progress> Progress { get; set; }
        public virtual DbSet<Nutritionist> Nutritionist { get; set; }
        public virtual DbSet<NutritionTipsAndQuotes> NutritionTipsAndQuotes { get; set;}
        public virtual DbSet<Training> Training{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.Entity<RegisteredUser>().ToTable("RegisteredUser");
            builder.Entity<PremiumUser>().ToTable("PremiumUser");
            builder.Entity<Admin>().ToTable("Admin");
            builder.Entity<Card>().ToTable("Card");
            builder.Entity<DietPlan>().ToTable("DietPlan");
            builder.Entity<Ingredient>().ToTable("Ingredient");
            builder.Entity<Recipe>().ToTable("Recipe");
            builder.Entity<Progress>().ToTable("Progress");
            builder.Entity<Nutritionist>().ToTable("Nutritionist");
            builder.Entity<NutritionTipsAndQuotes>().ToTable("NutritionTipsAndQuotes");
            builder.Entity<Training>().ToTable("Training");

            builder.Entity<ApplicationUser>()
                .Property(e => e.FullName);
            builder.Entity<ApplicationUser>()
                .Property(e => e.Id);
            builder.Entity<ApplicationUser>()
                .Property(e => e.EmailAddress);
            builder.Entity<ApplicationUser>()
                .Property(e => e.NutriPassword);
            builder.Entity<ApplicationUser>()
                .Property(e => e.NutriUsername);

            builder.Entity<DietPlan>()
            .HasMany(s => s.Recipes)
            .WithMany(c => c.DietPlans)
            .UsingEntity(j => j
                .ToTable("DietPlanRecipe")
                .Property<int>("Id")
                .ValueGeneratedOnAdd()
                .IsRequired()
                
            );
            builder.Entity("DietPlanRecipe").HasKey("Id");

            base.OnModelCreating(builder);

        }

    }
}