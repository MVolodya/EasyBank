using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class ConnectionContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CurrencyOperation> CurrencyOperations { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AccountStatus> AccountStatuses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Image> Images { get; set; }
        /*public DbSet<DepositOperation> DepositHistory { get; set; }
        public DbSet<TransferOperation> TransferHistory { get; set; }
        public DbSet<WithdrawOperation> WithdrawHistory { get; set; }*/

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<CurrencyOperation>().HasRequired(u => u.FromAccount).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<CurrencyOperation>().HasRequired(u => u.ToAccount).WithMany().WillCascadeOnDelete(false);
            /*modelBuilder.Entity<AccountType>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<AccountStatus>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Currency>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);*/

            modelBuilder.Entity<Account>()
    .HasMany(u => u.CurrencyOperations)
    .WithRequired(f => f.FromAccount)
    .HasForeignKey(f => f.FromAccountId);
            modelBuilder.Entity<CurrencyOperation>()
    .HasRequired(f => f.ToAccount)
    .WithMany()
    .HasForeignKey(f => f.ToAccountId);
        }
    }
}