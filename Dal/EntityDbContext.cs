using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Relationships;
using static Dal.Utilities.ConnectionStringUtility;

namespace Dal
{
    public sealed class EntityDbContext : IdentityDbContext<User, IdentityRole<int>, int>,
        IDesignTimeDbContextFactory<EntityDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor that will be called by startup.cs
        /// </summary>
        /// <param name="optionsBuilderOptions"></param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasIndex(x => x.Name)
                .IsUnique();
            
            modelBuilder.Entity<UserVote>()
                .HasOne(x => x.User)
                .WithMany(x => x.Votes);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Projects)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Project);
            
            modelBuilder.Entity<ProjectCategoryRelationship>()
                .HasKey(x => new {x.ProjectId, x.CategoryId});

            modelBuilder.Entity<ProjectCategoryRelationship>()
                .HasOne(x => x.Project)
                .WithMany(x => x.ProjectCategoryRelationships)
                .HasForeignKey(x => x.ProjectId);
            
            modelBuilder.Entity<ProjectCategoryRelationship>()
                .HasOne(x => x.Category)
                .WithMany(x => x.ProjectCategoryRelationships)
                .HasForeignKey(x => x.CategoryId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            
            base.OnConfiguring(optionsBuilder);
        }

        
        /// <summary>
        ///     This is used for DB migration locally
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public EntityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var options = new DbContextOptionsBuilder<EntityDbContext>()
                .UseNpgsql(ConnectionStringUrlToPgResource(configuration.GetValue<string>("DATABASE_URL")))
                .Options;

            return new EntityDbContext(options);
        }
    }
}