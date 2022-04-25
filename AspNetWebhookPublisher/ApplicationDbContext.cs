using System;
using System.IO;
using AspNetWebhookPublisher.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AspNetWebhookPublisher
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
        public DbSet<WebhookSubscriptionAllowedEvent> WebhookSubscriptionAllowedEvents { get; set; }
        public DbSet<WebhookSubscriptionContentType> WebhookSubscriptionContentTypes { get; set; }
        public DbSet<WebhookSubscriptionType> WebhookSubscriptionTypes { get; set; }
        public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
        public DbSet<Person> Persons { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<WebhookEvent>().HasData(
                new WebhookEvent() { Id=Guid.NewGuid(), Name = "person.created", DisplayName = "Person Created", Description = "Triggered when personel is created.", Created = DateTime.Now},
                new WebhookEvent() { Id = Guid.NewGuid(), Name = "person.updated", DisplayName = "Person Updated", Description = "Triggered when personel is updated.", Created = DateTime.Now },
                new WebhookEvent() { Id = Guid.NewGuid(), Name = "person.deleted", DisplayName = "Person Deleted", Description = "Triggered when personel is deleted.", Created = DateTime.Now }
            );
            var webhookSubscriptionContentType1 =  new WebhookSubscriptionContentType() { Id = Guid.NewGuid(), Name = "application/json", Created = DateTime.Now };
            var webhookSubscriptionContentType2 = new WebhookSubscriptionContentType() { Id = Guid.NewGuid(), Name = "application/x-www-form-urlencoded", Created = DateTime.Now };
            builder.Entity<WebhookSubscriptionContentType>().HasData(
                webhookSubscriptionContentType1,
                 webhookSubscriptionContentType2
                );
            var webhookSubscriptionType1 = new WebhookSubscriptionType() { Id = Guid.NewGuid(), Name = "All", Created = DateTime.Now };
            var webhookSubscriptionType2 = new WebhookSubscriptionType() { Id = Guid.NewGuid(), Name = "Specific", Created = DateTime.Now };
            builder.Entity<WebhookSubscriptionType>().HasData(
                webhookSubscriptionType1,
                 webhookSubscriptionType2
                );
            builder.Entity<WebhookSubscription>().HasData(
                new WebhookSubscription() {  Id = Guid.NewGuid(), PayloadUrl= "http://localhost:5045/webhook-json-data-test", Secret = "secret", WebhookSubscriptionContentTypeId = webhookSubscriptionContentType1.Id, WebhookSubscriptionTypeId = webhookSubscriptionType1.Id, Created = DateTime.Now, IsActive = true },
                 new WebhookSubscription() { Id = Guid.NewGuid(), PayloadUrl = "http://localhost:5045/webhook-form-data-test", Secret = "secret", WebhookSubscriptionContentTypeId = webhookSubscriptionContentType2.Id, WebhookSubscriptionTypeId = webhookSubscriptionType1.Id, Created = DateTime.Now, IsActive = true }
                );
        }
    }
}
