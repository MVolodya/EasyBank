using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class ConnectionContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        //public DbSet<CurrencyOperation> CurrencyOperations { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AccountStatus> AccountStatuses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<DepositOperation> DepositHistory { get; set; }
        public DbSet<TransferOperation> TransferHistory { get; set; }
        public DbSet<WithdrawOperation> WithdrawHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Operation>().HasRequired(u => u.FromAccount).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Operation>().HasRequired(u => u.ToAccount).WithMany().WillCascadeOnDelete(false);

            /*modelBuilder.Entity<AccountType>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<AccountStatus>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Currency>().HasRequired(d => d.Account).WithMany().WillCascadeOnDelete(false);*/

            modelBuilder.Entity<Account>()
    .HasMany(u => u.OperationsHistory)
    .WithRequired(f => f.FromAccount)
    .HasForeignKey(f => f.FromAccountId);
        }

        public override int SaveChanges()
        {
            Random rand = new Random();
            foreach (var entry in ChangeTracker.Entries<Account>()
                .Where(e => e.State == EntityState.Added))
            {
                /*DateTime now = DateTime.Now;
                int clientId = entry.Entity.ClientId + 1964;
                string clientIdString = clientId.ToString();
                if (clientIdString.Length < 7) clientIdString = clientIdString.PadLeft(7, '1');
                
                string[] separatedClientId = new string[2];
                separatedClientId[0] = clientIdString.Substring(0, 4);
                separatedClientId[1] = clientIdString.Substring(4, 3);

                string accCount;
                if (entry.Entity.Client.Accounts != null)
                    accCount = entry.Entity.Client.Accounts.Count.ToString().PadLeft(2, '8');
                else
                    accCount = "00";

                string randNumber = rand.Next(0, 10).ToString();

                string accountNumber = separatedClientId[0] + accCount + randNumber + separatedClientId[1];
                Debug.WriteLine("--------------------" + accountNumber + "-----------------------------");*/

                int first, second;
                first = rand.Next(11111,99999);
                second = rand.Next(11111,99999);
                entry.Entity.AccountNumber = first.ToString() + second.ToString();
            }
            return base.SaveChanges();
        }
    }
}