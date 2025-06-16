namespace FastTrade.Data
{
    public class UsersDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer($"Data Source={PublicSettings.MSSQLServer};Initial Catalog={PublicSettings.MSSQLDATABASE};Persist Security Info=False;User ID={PublicSettings.MSSQLUSERNAME};Password={PublicSettings.MSSQLPASSWORD};Encrypt=True;Trust Server Certificate=True;Connection Timeout=5;Command Timeout=300;Pooling=True;Application Name=FastTrade-{AppInfo.VersionString};Language=Turkish;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
                .Property(e => e.IND)
                .ValueGeneratedOnAdd();
        }
    }
}
