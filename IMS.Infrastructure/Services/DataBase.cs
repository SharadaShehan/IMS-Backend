using IMS.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Services
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<Equipment> equipments { get; set; }
        public DbSet<ItemReservation> itemReservations { get; set; }
        public DbSet<Lab> labs { get; set; }
        public DbSet<Maintenance> maintenances { get; set; }

        // Use this constructor for Presentation Layer

        public DataBaseContext(DbContextOptions<DataBaseContext> option)
            : base(option) { }

        // Use this constructor to apply migrations to the database, changing the connection string
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LabTracker;Trusted_Connection=True;TrustServerCertificate=True");
        }
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(k => k.UserId);
            });
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(k => k.EquipmentId);
            });
            modelBuilder.Entity<ItemReservation>(entity =>
            {
                entity.HasKey(k => k.ItemReservationId);
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(k => k.ItemId);
            });
            modelBuilder.Entity<Lab>(entity =>
            {
                entity.HasKey(k => k.LabId);
            });
            modelBuilder.Entity<Maintenance>(entity =>
            {
                entity.HasKey(k => k.MaintenanceId);
            });

            // user table collections from maintenance table
            modelBuilder
                .Entity<Maintenance>()
                .HasOne(m => m.Technician)
                .WithMany(u => u.MaintenancesAssignedTo)
                .HasForeignKey(m => m.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Maintenance>()
                .HasOne(m => m.CreatedClerk)
                .WithMany(u => u.MaintenancesCreatedBy)
                .HasForeignKey(m => m.CreatedClerkId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Maintenance>()
                .HasOne(m => m.ReviewedClerk)
                .WithMany(u => u.MaintenancesReviewedBy)
                .HasForeignKey(m => m.ReviewedClerkId)
                .OnDelete(DeleteBehavior.Restrict);

            // user table collections from item reservations table
            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.ReservedUser)
                .WithMany(u => u.ItemsReservedBy)
                .HasForeignKey(ir => ir.ReservedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.RespondedClerk)
                .WithMany(u => u.ReservationsRespondedTo)
                .HasForeignKey(ir => ir.RespondedClerkId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.LentClerk)
                .WithMany(u => u.ItemsBorrowedFrom)
                .HasForeignKey(ir => ir.LentClerkId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.ReturnAcceptedClerk)
                .WithMany(u => u.ItemsReturnedTo)
                .HasForeignKey(ir => ir.ReturnAcceptedClerkId)
                .OnDelete(DeleteBehavior.Restrict);

            // item table collections from maintenance table
            modelBuilder
                .Entity<Maintenance>()
                .HasOne(m => m.Item)
                .WithMany(i => i.Maintenances)
                .HasForeignKey(m => m.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // item table collections from item reservations table
            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.Item)
                .WithMany(i => i.Reservations)
                .HasForeignKey(ir => ir.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ItemReservation>()
                .HasOne(ir => ir.Equipment)
                .WithMany(i => i.ItemReservations)
                .HasForeignKey(ir => ir.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // equipment table collections from item table
            modelBuilder
                .Entity<Item>()
                .HasOne(i => i.Equipment)
                .WithMany(e => e.Items)
                .HasForeignKey(i => i.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // lab table collections from equipment table
            modelBuilder
                .Entity<Equipment>()
                .HasOne(e => e.Lab)
                .WithMany(l => l.Equipments)
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
