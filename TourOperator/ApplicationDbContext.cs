using System;
using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TourOperator.Entities;

namespace TourOperator
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString); 
            return new ApplicationDbContext(builder.Options);
        }
    }

    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<WebhookEvent> WebhookEvents { get; set; }
        public DbSet<WebhookPayload> WebhookPayloads { get; set; }
        public DbSet<WebhookResponse> WebhookResponses { get; set; }
        public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
        public DbSet<Reservation> Persons { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<WebhookEvent>().HasData(
                new WebhookEvent() { Id=Guid.NewGuid(), Name = "reservation.created", DisplayName = "Reservation Created", Description = "Triggered when reservation is created.", Created = DateTime.Now},
                new WebhookEvent() { Id = Guid.NewGuid(), Name = "reservation.canceled", DisplayName = "Reservation Canceled", Description = "Triggered when reservation is canceled.", Created = DateTime.Now },
                new WebhookEvent() { Id = Guid.NewGuid(), Name = "reservation.updated", DisplayName = "Reservation Updated", Description = "Triggered when reservation is updated.", Created = DateTime.Now }
            );
       
            builder.Entity<WebhookSubscription>().HasData(
                new WebhookSubscription() {  Id = Guid.NewGuid(), PayloadUrl= "http://45.85.146.19:999/webhook", Secret = "secret", Created = DateTime.Now, IsActive = true }
            );
        }
    }
}
