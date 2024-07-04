using LabTracker.Model;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Data
{
	public class DataBaseContext : DbContext
    {
		public DbSet<User> users { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Equipment> Equipments { get; set; }
		public DbSet<ItemReservation> ItemReservations { get; set; }
		public DbSet<Lab> Labs { get; set; }
		public DbSet<Maintenance> Maintenances { get; set; }
		public DataBaseContext(DbContextOptions<DataBaseContext> option): base(option) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(k => k.Email);
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
		}
	}
}
