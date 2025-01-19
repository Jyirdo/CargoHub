using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Contexts
{
    public class CargoHubDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public CargoHubDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("CargoHubDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Global query filter for soft delete
            modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Location>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<Inventory>().HasQueryFilter(i => !i.IsDeleted);
            modelBuilder.Entity<Transfer>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Item>().HasQueryFilter(it => !it.IsDeleted);
            modelBuilder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Supplier>().HasQueryFilter(su => !su.IsDeleted);
            modelBuilder.Entity<Item_Group>().HasQueryFilter(ig => !ig.IsDeleted);
            modelBuilder.Entity<Item_Line>().HasQueryFilter(il => !il.IsDeleted);
            modelBuilder.Entity<Item_Type>().HasQueryFilter(it => !it.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<Shipment>().HasQueryFilter(sh => !sh.IsDeleted);


            // Configure one-to-many relationship between Order and StockOfItems
            modelBuilder.Entity<Warehouse>()
                .OwnsOne(w => w.Contact);
            modelBuilder.Entity<Stock>()
            .ToTable("Stocks")
            .HasDiscriminator<string>("StockType")
            .HasValue<OrderStock>("Order")
            .HasValue<ShipmentStock>("Shipment")
            .HasValue<TransferStock>("Transfer");
        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Item_Group> Items_Groups { get; set; }
        public DbSet<Item_Line> Items_Lines { get; set; }
        public DbSet<Item_Type> Items_Types { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Shipment> Shipments { get; set; }


    }
}
