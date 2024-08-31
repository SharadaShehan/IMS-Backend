using Microsoft.EntityFrameworkCore;
using IMS.ApplicationCore.Model;

namespace IMS.Infrastructure.Services
{

	public class DataBaseContext : DbContext
	{
		public DbSet<User> users { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Equipment> Equipments { get; set; }
		public DbSet<ItemReservation> ItemReservations { get; set; }
		public DbSet<Lab> Labs { get; set; }
		public DbSet<Maintenance> Maintenances { get; set; }
		public DataBaseContext(DbContextOptions<DataBaseContext> option) : base(option) { }
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

			modelBuilder.Entity<Maintenance>()
				.HasOne(m => m.Technician)
				.WithMany(u => u.MaintenancesAssignedTechnician)
				.HasForeignKey(m => m.AssignedTechnician)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Maintenance>()
				.HasOne(m => m.Assigner)
				.WithMany(u => u.MaintenancesAssignedBy)
				.HasForeignKey(m => m.AssignedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Maintenance>()
				.HasOne(m => m.Reviewer)
				.WithMany(u => u.MaintenancesReviewedBy)
				.HasForeignKey(m => m.ReviwedBy)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<ItemReservation>()
				.HasOne(ir => ir.ReservedByUser)
				.WithMany(u => u.ReservedItems)
				.HasForeignKey(ir => ir.ReservedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ItemReservation>()
				.HasOne(ir => ir.ResponseedByUser)
				.WithMany(u => u.ResponseItems)
				.HasForeignKey(ir => ir.ResponseedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ItemReservation>()
				.HasOne(ir => ir.BorrowedFromUser)
				.WithMany(u => u.BorrowedItems)
				.HasForeignKey(ir => ir.BorrowedFrom)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ItemReservation>()
				.HasOne(ir => ir.ReturnedToUser)
				.WithMany(u => u.ReturnedItems)
				.HasForeignKey(ir => ir.ReturnedTo)
				.OnDelete(DeleteBehavior.Restrict);
			base.OnModelCreating(modelBuilder);
		}
	}
}
