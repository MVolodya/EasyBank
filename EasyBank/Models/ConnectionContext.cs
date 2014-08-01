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
        public DbSet<ClientsImage> Images { get; set; }
        public DbSet<Operation> OperationHistory { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<DepositCreditModel> DepositCreditModels { get; set; }
        public DbSet<ErrorReport> ErrorReports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Operation>().HasOptional(u => u.FromAccount).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Operation>().HasOptional(u => u.ToAccount).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Operation>()
    .HasOptional(o => o.FromAccount)
    .WithMany(a => a.OperationsHistory);
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