namespace FastTrade.Data
{
    public class ProductDbContext : DbContext
    {
        public DbSet<ProductSpecialCode> ProductSpecialCode { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer($"Data Source={PublicSettings.MSSQLServer};Initial Catalog={PublicSettings.MSSQLDATABASE};Persist Security Info=False;User ID={PublicSettings.MSSQLUSERNAME};Password={PublicSettings.MSSQLPASSWORD};Encrypt=True;Trust Server Certificate=True;Connection Timeout=5;Command Timeout=300;Pooling=True;Application Name=FastTrade-{AppInfo.VersionString};Language=Turkish;");
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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntries = ChangeTracker.Entries<Product>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                entry.Entity.ModifiedDate = DateTime.Now;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
