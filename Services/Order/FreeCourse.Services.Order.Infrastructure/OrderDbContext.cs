using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Services.Order.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        //Şema belirledik.
        public const string DEFAULT_SCHEMA = "ordering";
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }

        public DbSet<Domain.OrderAggreagate.Order> Orders { get; set; }
        //Value Object'i tanımladık veritabanında bir karşılığı olsun istemediğimiz için.
        public DbSet<Domain.OrderAggreagate.OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.OrderAggreagate.Order>().ToTable("Orders", DEFAULT_SCHEMA);
            modelBuilder.Entity<Domain.OrderAggreagate.OrderItem>().ToTable("Orders", DEFAULT_SCHEMA);
            modelBuilder.Entity<Domain.OrderAggreagate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

            //Owned Types //Veriler Order sınıfı üzerinden eklenecek kontrollü şekilde oluşturuyoruz bu sayede.
            modelBuilder.Entity<Domain.OrderAggreagate.Order>().OwnsOne(o => o.Address).WithOwner();

            base.OnModelCreating(modelBuilder);
        }



    }
}
