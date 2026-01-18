using Domain.Billing;
using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public sealed class NextGymDbContext : DbContext
    {

        public NextGymDbContext(DbContextOptions<NextGymDbContext> options) : base(options) { }

        public DbSet<Member> Members => Set<Member>();
        public DbSet<HealthProfile> HealthProfiles => Set<HealthProfile>();
        public DbSet<WeightLog> WeightLogs => Set<WeightLog>();

        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Payment> Payments => Set<Payment>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // members
            modelBuilder.Entity<Member>(e =>
            {
                e.ToTable("members");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");

                e.Property(x => x.MemberCode).HasColumnName("member_code").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.MemberCode).IsUnique();

                e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
                e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100);

                e.Property(x => x.Mobile).HasColumnName("mobile").HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Mobile).IsUnique();

                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(191);

                e.Property(x => x.Dob).HasColumnName("dob");
                e.Property(x => x.Gender).HasColumnName("gender").HasMaxLength(50);
                e.Property(x => x.Address).HasColumnName("address");

                e.Property(x => x.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(255);
                e.Property(x => x.EmergencyContactPhone).HasColumnName("emergency_contact_phone").HasMaxLength(50);

                e.Property(x => x.JoinDate).HasColumnName("join_date");
                e.Property(x => x.Status)
                    .HasColumnName("status")
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<MemberStatus>(v, true)
                    );

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasOne(x => x.HealthProfile)
                    .WithOne(h => h.Member)
                    .HasForeignKey<HealthProfile>(h => h.MemberId);

                e.HasMany(x => x.WeightLogs)
                    .WithOne(w => w.Member)
                    .HasForeignKey(w => w.MemberId);
            });

            // health_profiles
            modelBuilder.Entity<HealthProfile>(e =>
            {
                e.ToTable("health_profiles");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.MemberId).HasColumnName("member_id");

                e.Property(x => x.HeightCm).HasColumnName("height_cm");
                e.Property(x => x.RestingHr).HasColumnName("resting_hr");
                e.Property(x => x.BloodPressure).HasColumnName("blood_pressure").HasMaxLength(50);

                // JSON stored as string (MySQL JSON column)
                e.Property(x => x.ConditionsJson).HasColumnName("conditions_json").HasColumnType("json");
                e.Property(x => x.Notes).HasColumnName("notes");

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasIndex(x => x.MemberId).IsUnique();
            });

            // weight_logs
            modelBuilder.Entity<WeightLog>(e =>
            {
                e.ToTable("weight_logs");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.MemberId).HasColumnName("member_id");

                e.Property(x => x.LogDate).HasColumnName("log_date");
                e.Property(x => x.WeightKg).HasColumnName("weight_kg");

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasIndex(x => new { x.MemberId, x.LogDate }).HasDatabaseName("idx_weight_logs_member_date");
            });

            modelBuilder.Entity<Package>(e =>
            {
                e.ToTable("packages");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.DurationDays).HasColumnName("duration_days").IsRequired();
                e.Property(x => x.Price).HasColumnName("price").HasPrecision(12, 2);
                e.Property(x => x.SessionLimit).HasColumnName("session_limit");
                e.Property(x => x.IsActive).HasColumnName("is_active");

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasIndex(x => x.Name).IsUnique().HasDatabaseName("uq_packages_name");
            });

            // subscriptions
            modelBuilder.Entity<Subscription>(e =>
            {
                e.ToTable("subscriptions");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.MemberId).HasColumnName("member_id");
                e.Property(x => x.PackageId).HasColumnName("package_id");

                e.Property(x => x.StartDate).HasColumnName("start_date");
                e.Property(x => x.EndDate).HasColumnName("end_date");

                e.Property(x => x.Status)
                    .HasColumnName("status")
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<SubscriptionStatus>(v, true)
                    );

                e.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
                e.Property(x => x.Discount).HasColumnName("discount").HasPrecision(12, 2);
                e.Property(x => x.Taxes).HasColumnName("taxes").HasPrecision(12, 2);
                e.Property(x => x.TotalPayable).HasColumnName("total_payable").HasPrecision(12, 2);

                e.Property(x => x.Notes).HasColumnName("notes");

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasIndex(x => x.MemberId).HasDatabaseName("idx_subscriptions_member");
                e.HasIndex(x => x.Status).HasDatabaseName("idx_subscriptions_status");
                e.HasIndex(x => new { x.StartDate, x.EndDate }).HasDatabaseName("idx_subscriptions_dates");

                e.HasOne(x => x.Member)
                    .WithMany()
                    .HasForeignKey(x => x.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Package)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(x => x.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Payments)
                    .WithOne(p => p.Subscription)
                    .HasForeignKey(p => p.SubscriptionId);
            });

            // payments
            modelBuilder.Entity<Payment>(e =>
            {
                e.ToTable("payments");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.SubscriptionId).HasColumnName("subscription_id");

                e.Property(x => x.PaidAt).HasColumnName("paid_at");

                e.Property(x => x.Method)
                    .HasColumnName("method")
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<PaymentMethod>(v, true)
                    );

                e.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
                e.Property(x => x.Reference).HasColumnName("reference").HasMaxLength(255);

                e.Property(x => x.Status)
                    .HasColumnName("status")
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<PaymentStatus>(v, true)
                    );

                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                // Important: don't let EF try to send values for these
                e.Property(x => x.CreatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.CreatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.Property(x => x.UpdatedAt).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(x => x.UpdatedAt).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                e.HasIndex(x => x.SubscriptionId).HasDatabaseName("idx_payments_subscription");
                e.HasIndex(x => x.PaidAt).HasDatabaseName("idx_payments_paid_at");
                e.HasIndex(x => x.Method).HasDatabaseName("idx_payments_method");
                e.HasIndex(x => x.Status).HasDatabaseName("idx_payments_status");
            });
        }
    }
}
