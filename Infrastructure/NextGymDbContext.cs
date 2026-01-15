using Domain.Members;
using Microsoft.EntityFrameworkCore;
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

                e.HasIndex(x => new { x.MemberId, x.LogDate }).HasDatabaseName("idx_weight_logs_member_date");
            });
        }
    }
}
