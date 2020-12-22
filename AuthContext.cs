using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AppServer
{
    public partial class AuthContext : DbContext
    {
        public AuthContext()
        {
        }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=771G\\SQLEXPRESS;Database=Auth;Trusted_Connection=True;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_100_CI_AI_SC_UTF8");

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Device");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .UseCollation("Latin1_General_100_CI_AI_SC_UTF8");

                entity.HasOne(d => d.Player)
                    .WithMany()
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Device_Player");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Player");

                entity.Property(e => e.AllowDid)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Token).UseCollation("Latin1_General_100_CI_AI_SC_UTF8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
