using Microsoft.EntityFrameworkCore;
using BMC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMC.StreamHub.Data;

namespace BMC.StreamHub.Data
{
    public class CloudIoTDB : DbContext
    {

        public CloudIoTDB()
        {
        }

        public CloudIoTDB(DbContextOptions<CloudIoTDB> options)
            : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<MqttTopic> MqttTopics { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<MessageStream> MessageStreams { get; set; }
     
      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*
            builder.Entity<DataEventRecord>().HasKey(m => m.DataEventRecordId);
            builder.Entity<SourceInfo>().HasKey(m => m.SourceInfoId);

            // shadow properties
            builder.Entity<DataEventRecord>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<SourceInfo>().Property<DateTime>("UpdatedTimestamp");
            */
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            /*
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<SourceInfo>();
            updateUpdatedProperty<DataEventRecord>();
            */
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(AppConstants.SQLConn, ServerVersion.AutoDetect(AppConstants.SQLConn));
            }
        }
        private void updateUpdatedProperty<T>() where T : class
        {
            /*
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
            */
        }

    }
}
