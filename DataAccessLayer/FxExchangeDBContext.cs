using Microsoft.EntityFrameworkCore;
using Models.Data;
using System.Reflection.Metadata;

namespace DataAccessLayer
{
    public class FxExchangeDBContext: DbContext
    {
        public FxExchangeDBContext(DbContextOptions<FxExchangeDBContext> options) : base(options)
        {
            
        }       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var CurrencyEUR = new Currency() { Id = 1, ISOCode = "EUR", Name = "EURO" };
            var CurrencyUSD = new Currency() { Id = 2, ISOCode = "USD", Name = "American Dollar" };
            var CurrencyCAD = new Currency() { Id = 3, ISOCode = "CAD", Name = "Canadian Dollar" };
            var CurrencyEGP = new Currency() { Id = 4, ISOCode = "EGP", Name = "Egyptian Pound" };
           
            modelBuilder.Entity<Currency>(Entity =>
            {
                Entity.Property(p => p.Id).ValueGeneratedOnAdd();
                Entity.HasData(CurrencyEUR, CurrencyUSD, CurrencyCAD, CurrencyEGP);
            });
            modelBuilder.Entity<FxTransactionDetailType>(Entity =>
            {
                Entity.Property(p => p.Id).ValueGeneratedOnAdd();
                Entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
                Entity.HasData(new FxTransactionDetailType()
                {
                    Id = 1,
                    Name = "Sell"
                }, new FxTransactionDetailType()
                {
                    Id = 2,
                    Name = "Buy"
                });
            });
            var Holder = new Holder()
            {
                Id = 1,
                FirstName = "Yasser",
                LastName = "Moaly",
                Address = "Zayed, Egypt",
                PassportId = "A27344511"
            };
            modelBuilder.Entity<Holder>(Entity =>
            {
                Entity.Property(p => p.Id).ValueGeneratedOnAdd();
                Entity.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
                Entity.Property(p => p.LastName).IsRequired().HasMaxLength(50);
                Entity.Property(p => p.PassportId).IsRequired().HasMaxLength(25);

                Entity.HasData(Holder);

            });
           
            modelBuilder.Entity<Account>(Entity =>
            {
                Entity.Property(p => p.Id).ValueGeneratedOnAdd();
                Entity.Property(p => p.RowVersion).IsRowVersion();
                Entity.ToTable(t => t.HasCheckConstraint("AccountBalanceGreaterThanOrEqualZero", "[Balance] >= 0"));
                Entity.HasData(new Account(){Id = 1,HolderId = Holder.Id,CurrencyId = CurrencyEUR.Id,Balance = 1000},
                    new Account(){Id = 2,HolderId = Holder.Id,CurrencyId = CurrencyUSD.Id,Balance = 1000},
                    new Account(){Id = 3,HolderId = Holder.Id,CurrencyId = CurrencyCAD.Id,Balance = 1000},
                    new Account(){Id = 4,HolderId = Holder.Id,CurrencyId = CurrencyEGP.Id,Balance = 1000}
                );
            });
            modelBuilder.Entity<FxTransactionFixedSide>(Entity =>
            {
                Entity.HasData(new FxTransactionFixedSide() { Id = 1, Name = "Buy" }, new FxTransactionFixedSide() { Id = 2, Name = "Sell" });                
            });

            modelBuilder.Entity<FxTransaction>(Entity =>
            {
                Entity.HasMany(r=>r.FxTransactionDetails).WithOne(r=>r.FxTransaction).HasForeignKey(r=>r.FxTransactionId).OnDelete(DeleteBehavior.NoAction);
            });
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Holder> Holders { get; set; }
        public DbSet<FxTransactionDetailType> FxTransactionDetailTypes { get; set; }
        public DbSet<FxTransaction> FxTransactions { get; set; }
        public DbSet<FxTransactionDetail> FxTransactionDetails { get; set; }
        public DbSet<FxTransactionFixedSide> FxTransactionFixedSides { get; set; }
        
    }
}
