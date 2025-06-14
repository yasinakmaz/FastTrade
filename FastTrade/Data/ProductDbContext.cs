namespace FastTrade.Data
{
    public class ProductDbContext : DbContext
    {
        public DbSet<ProductSpecialCode> ProductSpecialCode { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer($"Data Source={PublicSettings.MSSQLServer};Initial Catalog={PublicSettings.MSSQLDATABASE};Persist Security Info=True;User ID={PublicSettings.MSSQLUSERNAME};Password={PublicSettings.MSSQLPASSWORD};Encrypt=True;Trust Server Certificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
            .Property(p => p.IND)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProductSpecialCode>()
            .Property(p => p.IND)
            .ValueGeneratedOnAdd();
        }
    }
}
